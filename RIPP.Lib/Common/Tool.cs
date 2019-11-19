using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace RIPP.Lib
{
    /// <summary>
    /// 常用工具类
    /// </summary>
    public class Tool
    {
        /// <summary>
        /// 生成Guid
        /// </summary>
        /// <returns></returns>
        public static long GenerateId()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// 返回随机数
        /// </summary>
        /// <param name="VcodeNum">随机数位数</param>
        /// <returns></returns>
        public static string RndNum(int VcodeNum)
        {
            string Vchar = "0,1,2,3,4,5,6,7,8,9";

            string[] VcArray = Vchar.Split(',');
            string VNum = "";//由于字符串很短，就不用StringBuilder了
            int temp = -1;//记录上次随机数值，尽量避免生产几个一样的随机数

            //采用一个简单的算法以保证生成随机数的不同
            Random rand = new Random();
            for (int i = 1; i < VcodeNum + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));
                }
                //int t =  rand.Next(35) ;
                int t = rand.Next(9);
                if (temp != -1 && temp == t)
                {
                    return RndNum(VcodeNum);
                }
                temp = t;
                VNum += VcArray[t];

            }
            return VNum;
        }

        /// <summary>
        /// 根据时间生成ID
        /// </summary>
        /// <returns></returns>
        public static int GenerateIdByTime()
        {
            DateTime time = DateTime.Now;
            string bh = time.Month.ToString() + time.Day.ToString() + time.Hour.ToString() + time.Minute.ToString() + time.Second.ToString();
            return int.Parse(bh);
        }

        /// <summary>
        /// 判断是否为Double数据
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsDouble(string s)
        {
            double d = 0;
            return double.TryParse(s, out d);
        }

        public static void ChangeControl<T>(T target, ThreadStart method) where T : Control
        {
            //if (target.InvokeRequired)
            {
                target.BeginInvoke(method);
            }
           // else
            {
                
                //method.BeginInvoke(null, null);
            }
        }

        /// <summary>
        /// 计算文件的MD5校验
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="srcdir"></param>
        /// <param name="desdir"></param>
        public static void CopyDirectory(string srcdir, string desdir)
        {
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1);

            string desfolderdir = desdir + "\\" + folderName;

            if (desdir.LastIndexOf("\\") == (desdir.Length - 1))
            {
                desfolderdir = desdir + folderName;
            }
            string[] filenames = Directory.GetFileSystemEntries(srcdir);

            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {

                    string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }

                    CopyDirectory(file, desfolderdir);
                }

                else // 否则直接copy文件
                {
                    string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);

                    srcfileName = desfolderdir + "\\" + srcfileName;


                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }


                    File.Copy(file, srcfileName);
                }
            }//foreach
        }//function end

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFolder(string path)
        {
            string[] strTemp;

            //先删除该目录下的文件
            strTemp = System.IO.Directory.GetFiles(path);
            foreach (string str in strTemp)
            {
                System.IO.File.Delete(str);
            }
            //删除子目录，递归
            strTemp = System.IO.Directory.GetDirectories(path);
            foreach (string str in strTemp)
            {
                DeleteFolder(str);
            }
            //删除该目录
            System.IO.Directory.Delete(path);

        }

        #region DataGridView导出到csv格式的Excel
        public static void DataGridView2Excel_cvs(DataGridView dgv)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Excel files (*.xls)|*.xls";
            dlg.FilterIndex = 0;
            dlg.RestoreDirectory = true;
            //dlg.CreatePrompt = true;
            dlg.Title = "保存为Excel文件";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Stream myStream;
                myStream = dlg.OpenFile();
                StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));
                string columnTitle = "";
                try
                {
                    //写入列标题
                    for (int i = 0; i < dgv.ColumnCount; i++)
                    {
                        if (i > 0)
                        {
                            columnTitle += "\t";
                        }
                        columnTitle += dgv.Columns[i].HeaderText;
                    }
                    sw.WriteLine(columnTitle);
                    //写入内容
                    for (int j = 0; j < dgv.Rows.Count; j++)
                    {
                        string columnValue = "";
                        for (int k = 0; k < dgv.Columns.Count; k++)
                        {
                            if (k > 0)
                            {
                                columnValue += "\t";
                            }
                            if (dgv.Rows[j].Cells[k].Value == null)
                            {
                                columnValue += "";
                            }
                            else
                            {
                                columnValue += dgv.Rows[j].Cells[k].Value.ToString().Trim();
                            }
                        }
                        sw.WriteLine(columnValue);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    sw.Close();
                    myStream.Close();
                }
            }
        }
        #endregion


        /// <summary>
        /// 将整个控件保存为图片
        /// </summary>
        /// <param name="myControl"></param>
        /// <param name="filename"></param>
        public static void PrintInvisibleControl(Control myControl, string filename)
        {

            Graphics g = myControl.CreateGraphics();
            //new bitmap object to save the image        
            Bitmap bmp = new Bitmap(myControl.Width, myControl.Height);
            //Drawing control to the bitmap        
            myControl.DrawToBitmap(bmp, new Rectangle(0, 0, myControl.Width, myControl.Height));
            bmp.Save(filename, ImageFormat.Png);
            bmp.Dispose();
        }
    }
}
