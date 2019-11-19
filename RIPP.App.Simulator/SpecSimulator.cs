using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RIPP.App.Simulator
{
    public partial class SpecSimulator : Form
    {
        private Config _config = new Config();
        public SpecSimulator()
        {
            InitializeComponent();
            if (string.IsNullOrWhiteSpace(this._config.InputPath))
                this.txbInput.Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "光谱文件");
            else
                this.txbInput.Text = this._config.InputPath;

            if (string.IsNullOrWhiteSpace(this._config.OutpuPath))
                this.txbOutput.Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "spec");
            else
                this.txbOutput.Text = this._config.OutpuPath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(this.txbInput.Text.Trim()))
            {
                MessageBox.Show("来源光谱路径不存在");
                return;
            }

            if (!Directory.Exists(this.txbOutput.Text.Trim()))
            {
                MessageBox.Show("输出路径不存在");
                return;
            }

            DirectoryInfo di = new DirectoryInfo(this.txbInput.Text.Trim());

            FileInfo[] files = di.GetFiles("*.rip");

            Random myrand = new Random();
            int idx = myrand.Next(0, files.Count() - 1);

            string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + files[idx].Extension;

           var streamS = File.Open(files[idx].FullName, FileMode.Open);
           byte[] data = new byte[streamS.Length];
           streamS.Read(data, 0, (int)streamS.Length);

           using (var streamO = File.OpenWrite(Path.Combine(this.txbOutput.Text.Trim(), filename)))
           {
               streamO.Write(data, 0, data.Length);
           }
           streamS.Close();
            
           


         //   File.Copy(files[idx].FullName, Path.Combine(this.txbOutput.Text.Trim(), filename));

            var s = new RIPP.NIR.Spectrum(files[idx].FullName);
            this.specGraph1.DrawSpec(s);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var folder = new FolderBrowserDialog();
            folder.SelectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "光谱文件");
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txbInput.Text = folder.SelectedPath;
                this._config.InputPath = folder.SelectedPath;
                this._config.Save();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var folder = new FolderBrowserDialog();
            folder.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
            folder.SelectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "spec");
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txbOutput.Text = folder.SelectedPath;
                _config.OutpuPath = folder.SelectedPath;
                this._config.Save();
            }
        }
    }

    [Serializable]
    public class Config
    {
        private string _filepath = "SimulatorConfig.db";

        /// <summary>
        /// 光谱文件存放路径
        /// </summary>
        public string InputPath { set; get; }

        public string OutpuPath { set; get; }


        public Config()
        {
            if (!File.Exists(_filepath))
            {
                this.Save();
            }
            else
            {
                var d = RIPP.Lib.Serialize.Read<Config>(this._filepath);
                this.InputPath = d.InputPath;
                this.OutpuPath = d.OutpuPath;
            }
        }

        public void Save()
        {
            RIPP.Lib.Serialize.Write<Config>(this, this._filepath);
        }

        

    }

}
