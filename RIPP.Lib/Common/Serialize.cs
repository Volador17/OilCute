using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;

namespace RIPP.Lib
{
    public class Serialize
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 
        /// <summary>
        /// 将对象序列化到文件中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Write<T>(T content, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(fileStream, content);
                fileStream.Close();
            }

            return true;

        }
        /// <summary>
        /// 读取文件内容，并将内容反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Read<T>(string path)
        {
            if (!File.Exists(path))
            {
                Log.Warn("文件不存在：" + path);
                return default(T);
            }
            
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryFormatter b = new BinaryFormatter();
                var s = b.Deserialize(fileStream);
                fileStream.Close();
                return (T)s;
            }

        }
        /// <summary>
        /// 尝试复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepClone<T>(T obj)
        {
            T r = default(T);
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                r = (T)bf.Deserialize(ms);
            }
            return r;
        }

        public static byte[] ObjectToByte(object content)
        {
            using(var stream=new MemoryStream())
            {
                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(stream, content);
                return stream.ToArray();
            }
        }

        public static T ByteToObject<T>(byte[] content)
        {
            T r = default(T);
            using (var stream = new System.IO.MemoryStream(content))
            {
                BinaryFormatter bf = new BinaryFormatter();
                r = (T)bf.Deserialize(stream);
            }
            return r;
        }
    }
}
