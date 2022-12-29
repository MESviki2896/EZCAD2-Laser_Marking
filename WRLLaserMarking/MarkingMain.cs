using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using FlagCheck;
using SuperSimpleTcp;

namespace WRLLaserMarking
{
    public partial class MarkingMain : Form
    {
        SimpleTcpServer server;
        MasterModel masterModel;
        public MarkingMain()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            //openFileDialog1.Filter = "EzCad2L files(*.ezd)| *.ezd | All files(*.*) | *.* ";
            // openFileDialog1.Filter = "EzCad2L files(*.txt)| *.txt | All files(*.*) | *.* ";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var FileName = openFileDialog1.FileName;
                textBox3.Text = openFileDialog1.FileName;
                if (string.IsNullOrEmpty(Appfunctions.GetConfiguration("FileName")))
                {
                    Appfunctions.AddConfiguration("FileName", textBox3.Text);
                 
                }
                else
                {
                    Appfunctions.SetConfiguration("FileName", textBox3.Text);
                }
                KeyboardFunction(textBox3.Text);
            }
        }
        private Process TaskRun(string FileName)    //Task Start with Safety
        {

            if (CheckRunningProcess(Appfunctions.GetConfiguration("ProcessName"))) return null;
            try
            {
                var value = Process.Start(FileName);
                value.WaitForInputIdle();
                return value;
            }
            catch(Win32Exception ex)
            {
                    Appfunctions.SetConfiguration("FileName", "");
                textBox3.Text = "";
                return null;
            }
            
        }
        private bool CheckRunningProcess(string ProcessName = "EzCAD2")
        {
            var processes = Process.GetProcessesByName(ProcessName);

            if (processes.Count() > 0)
            {
                MessageBox.Show(String.Concat("Process '", ProcessName, "' is already running"));
                label10.Text = Process.GetProcessesByName(ProcessName)[0].Id.ToString();
                return true;
            }
            return false;

        } //Check Existing Process
        private void KeyboardFunction(string FileName)
        {
            if (string.IsNullOrEmpty(FileName)) return;
            var ProcessID = TaskRun(FileName);
            if (ProcessID == null) return;
            label10.Text = ProcessID.Id.ToString();
            if (ProcessID == null) return;
            // System.Threading.Thread.Sleep(int.Parse(Appfunctions.GetConfiguration("Delay")));
            Interaction.AppActivate(ProcessID.Id);
            Keyboard keyboard = new Keyboard();
            // keyboard.SendKeys("{LEFT}", true);
            // keyboard.SendKeys("{F3}", true);
            // keyboard.SendKeys("^a", true);
            this.Activate();
        }   //Engage KeyBoardEvent

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox3.Text = Appfunctions.GetConfiguration("FileName");
            server = new SimpleTcpServer(Appfunctions.GetConfiguration("TCIPPort").Split(':')[0], int.Parse(Appfunctions.GetConfiguration("TCIPPort").Split(':')[1]));
            label12.Text = Appfunctions.GetConfiguration("TCIPPort").Split(':')[0];
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived1;
            this.TopMost = true;
            //  TCPIPWorker.RunWorkerAsync();

            KeyboardFunction(textBox3.Text);

        }

        private void Events_DataReceived1(object sender, SuperSimpleTcp.DataReceivedEventArgs e)
        {

            string request = Encoding.UTF8.GetString(e.Data.Array,0, masterModel.AssetVariable.Length);
          //  MessageBox.Show(request.Trim() +" and "+ masterModel.AssetVariable);

            if (request.Trim() == masterModel.AssetVariable)
            {
                server.Send(e.IpPort, textBox2.Text);
            //    MessageBox.Show($"Sent Response {textBox2.Text}");

            }
            else
            {
                server.Send(e.IpPort, " ");
              //  MessageBox.Show("Empty Data");
            }

            // throw new NotImplementedException();
        }

        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            // throw new NotImplementedException();
        }

        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    if (string.IsNullOrEmpty(textBox3.Text))
                    {
                        MessageBox.Show("Please select a Template File");
                        textBox1.SelectAll();
                        return;
                    }

                    if (textBox1.Text.Length == 0) return;

                    var Values = textBox1.Text.Split(';');
                    //Scope of Checking the elements, Future Task

                   //Getting Values
                    textBox2.Text = Values[5];
                    string ProductNumber = Values[4].Substring(1, 4);
                    var ProductDetails = SqlConnection.GetModelDetails(ProductNumber);
                    masterModel = ProductDetails[0];
                    
                    //Rigging Up settings
                    textBox4.Text = ProductDetails[0].ProductName;
                    textBox5.Text = ProductDetails[0].ProductCode;
                    textBox6.Text = ProductDetails[0].AssetVariable;
                    textBox7.Text = ProductDetails[0].Type;
                    textBox8.Text = ProductDetails[0].ImageName;
                    pictureBox1.Image = null;
                    pictureBox1.Image = new Bitmap(Appfunctions.ReadBlob(ProductDetails[0].AssetImage, ProductDetails[0].ImageName));

                    //Executing the engraving
                    //Check the runnign status of process
                    //else restart thr process
                    //save processid
                    //Else throw msg and exit
                    //Check if TCP server is running
                    //Else start
                    //Activate the Template file
                    //Send F2 key


                    //......Lets Start
                    TCPIPWorker.RunWorkerAsync();
                }
                return;
            }
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            MarkingSettings Markset = new MarkingSettings();
            if (Markset.ShowDialog() == DialogResult.OK)
                return;

        }

        private void TCPIPWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            if (CheckRunningPorcess() == false)
            {
                textBox1.SelectAll();
                return;
            }
            if (!server.IsListening)
            {
                // MessageBox.Show("Starting Server");
                server.Start();
            }
            Interaction.AppActivate(int.Parse(label10.Text));
           // System.Threading.Thread.Sleep(int.Parse(Appfunctions.GetConfiguration("Delay")));
            Keyboard keyboard = new Keyboard();
            keyboard.SendKeys(Appfunctions.GetConfiguration("SendKey1"), true);
            
            
        }

        private void TCPIPWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Activate();
            textBox1.SelectAll();
        }

        private void TCPIPWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }


        private bool CheckRunningPorcess()
        {
            try
            {
                var value = Process.GetProcessById(int.Parse(label10.Text));          
            }
            catch (ArgumentException ex)
            {
                if(string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Please select a template File");
                    return false;
                }
                var process = TaskRun(textBox3.Text);
                label10.Text = process.Id.ToString();

            }
            catch (FormatException ex)
            {
                MessageBox.Show("No File selected, Marking process stop");

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;

        }




        private void MarkingMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.DisconnectClient("127.0.0.1:2000");
            server.Dispose();
            var process = Process.GetProcessById(int.Parse(label10.Text));
           
              process.Kill();
            
        }
    }
    }
