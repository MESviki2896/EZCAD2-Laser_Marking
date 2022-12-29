using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using System.Windows.Forms;
using System.Configuration;
using WRLLaserMarking;
using System.IO;

namespace FlagCheck
{
    internal class Appfunctions
    {
        
        public static string GetConfiguration(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
        public static void SetConfiguration(string Key, string value)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[Key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
        }
        public static void AddConfiguration(string Key, string value)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationElement keel = new KeyValueConfigurationElement(Key, value);
            config.AppSettings.Settings.Add(keel);
            config.Save(ConfigurationSaveMode.Modified);
        }        //check scanned data
       
        public static bool KeyboardFunction(string serialnumber)
        {
            Process[] process;
            process = Process.GetProcessesByName(Appfunctions.GetConfiguration("ProcessName"));
            try
            {
                if(process.Length>0)
                {
                    int val = process[0].Id;
                    Interaction.AppActivate(val);
                    Keyboard keyboard = new Keyboard();
                    keyboard.SendKeys(serialnumber, true);
                    keyboard.SendKeys("{ENTER}", true);
                    //Appfunctions.logger("FlagCheck Sendkeyboardfunction Result: sent Successfully!");
                    return true;
                }
                else
                {
                    //Appfunctions.logger("FlagCheck Sendkeyboardfunction Result: Make sure tool is running");
                    MessageBox.Show("Please make sure the required tool is running");
                    return false;
                }
            }
            catch(Exception ex)
            {
               // Appfunctions.logger("FlagCheck Sendkeyboardfunction Result: "+ex.Message);
                return false;
            }           
        }
        public static byte[] CreateBlob(string filename)
        {
            if(string.IsNullOrWhiteSpace(filename))
            {
                return null;
            }
            using(FileStream imgStream = File.OpenRead(filename))
            {
                byte[] blob = new byte[imgStream.Length];
                imgStream.Read(blob, 0, (int)imgStream.Length);
                if(blob.Count()>0)
                {
                    return blob;
                }
                return null;

            }

        }
        public static string ReadBlob(byte[] Blob,string Filename)
        {
            string temppath = System.IO.Path.GetTempFileName();
            var TempFile = temppath.Replace(".tmp","."+ Filename.Split('.')[1]);
            //if(File.Exists(TempFile)) File.Delete(TempFile);
            //File.Move(temppath, value);
            
            File.WriteAllBytes(TempFile, Blob);
            return TempFile;
        }

    }
}
