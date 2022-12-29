using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 

namespace WRLLaserMarking
{
    public partial class MarkingSettings : Form
    {
        public MarkingSettings()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                label6.Text=openFileDialog1.SafeFileName;
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
                pictureBox1.ImageLocation = openFileDialog1.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox1.Text)|| string.IsNullOrEmpty(textBox2.Text)|| string.IsNullOrEmpty(textBox3.Text)|| string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Some Fields are empty, Kindly add");
                return;
            }
            if(comboBox1.SelectedIndex==0)
            {
                MessageBox.Show("Please Select vendor Type!");
                return;
            }

            MasterModel MModel = new MasterModel();
              MModel = MasterModel.AddModelValues(textBox2.Text,textBox1.Text,textBox3.Text,comboBox1.SelectedItem.ToString(),textBox4.Text,label6.Text,FlagCheck.Appfunctions.CreateBlob(pictureBox1.ImageLocation));
           
            if (SqlConnection.SetModelDetails(MModel) == false)
            {
                MessageBox.Show("Unable to Add records");
                return;
            }
            CLearParameters();
        }

        private void CLearParameters()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            comboBox1.SelectedIndex = 0;
            pictureBox1.Image = null;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

    
    }
}
