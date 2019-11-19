using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace RIPP.App.Chem
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();

            
           

        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Forms.Spec.MainForm().Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            new RIPP.App.Chem.Forms.Spec.MainForm().ShowDialog();
        }

        private void label1_MouseHover(object sender, EventArgs e)
        {
            var lable = sender as Label;
            lable.BackColor = System.Drawing.SystemColors.ButtonHighlight;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            var lable = sender as Label;
            lable.BackColor = System.Drawing.SystemColors.ButtonFace;
        }

        private void label6_Click(object sender, EventArgs e)
        {
         //   new RIPP.App.Chem.Forms.Model.ModelBindForm().ShowDialog();
        }

        private void label2_Click(object sender, EventArgs e)
        {
          //  new RIPP.App.Chem.Forms.Model.MainForm().ShowDialog();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            new RIPP.App.Chem.Forms.Identify.MainForm().ShowDialog();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            new RIPP.App.Chem.Forms.Fitting.MainForm().ShowDialog();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            new RIPP.App.Chem.Forms.Mix.MainForm().ShowDialog();
        }

        //private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF }; 
        //public static string EncryptDES(string encryptString, string encryptKey)
        //{
        //    try
        //    {

        //        byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
        //        byte[] rgbIV = Keys;
        //        byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
        //        DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
        //        MemoryStream mStream = new MemoryStream();
        //        CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
        //        cStream.Write(inputByteArray, 0, inputByteArray.Length);
        //        cStream.FlushFinalBlock();
        //        return Convert.ToBase64String(mStream.ToArray());
        //    }
        //    catch
        //    {
        //        return encryptString;
        //    }
        //} 
    }
}
