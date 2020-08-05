using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

namespace BarrageEditor
{
    public class Clipboard
    {
        private static object _data;

        public static bool CopyToClipboard(object obj)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();
                string str = Convert.ToBase64String(buffer);
                str = "/\\nd/\\" + str + "/\\nd/\\";
                GUIUtility.systemCopyBuffer = str;
                return true;
            }
            catch (Exception e)
            {
                BarrageProject.LogError(string.Format("Serialize object fail , {0}", e.Message));
                return false;
            }
        }

        public static bool GetClipboardObject<T>(out T obj)
        {
            string clipboardStr = GUIUtility.systemCopyBuffer;
            obj = default(T);
            if (clipboardStr.StartsWith("/\\nd/\\") && clipboardStr.EndsWith("/\\nd/\\"))
            {
                try
                {
                    clipboardStr = clipboardStr.Substring(6, clipboardStr.Length - 12);
                    IFormatter formatter = new BinaryFormatter();
                    byte[] buffer = Convert.FromBase64String(clipboardStr);
                    MemoryStream stream = new MemoryStream(buffer);
                    obj = (T)formatter.Deserialize(stream);
                    stream.Flush();
                    stream.Close();
                    return true;
                }
                catch (Exception e)
                {
                    BarrageProject.LogError(string.Format("Deserialize to object fail , {0}", e.Message));
                    return false;
                }
            }
            return false;
        }



        public static void SetDataObject(object obj)
        {
            _data = obj;
        }

        public static object GetDataObject()
        {
            return _data;
        }
    }
}
