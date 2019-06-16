﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace YKEngine
{
    public class FileUtils
    {
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="filter">格式 desc\0*.xxx\0 xxx为扩展名,举例txt\0*.txt</param>
        /// <param name="multiSelect">是否可以多选</param>
        /// <returns></returns>
        public static string OpenFile(string title,string filter,bool multiSelect)
        {
            var projectFolder = Directory.GetCurrentDirectory();
            FileDialogBase pth = new FileDialogBase();
            pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
            pth.filter = filter;
            pth.file = new string(new char[256]);
            pth.maxFile = pth.file.Length;
            pth.fileTitle = new string(new char[64]);
            pth.maxFileTitle = pth.fileTitle.Length;
            pth.initialDir = Application.dataPath;  // default path  
            pth.title = title;
            pth.defExt = "txt";
            pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
            if ( multiSelect )
            {
                pth.flags |= 0x00000200;
            }
            string filePath = null;
            if (OpenFileDialog.GetOpenFileName(pth))
            {
                filePath = pth.file;//选择的文件路径;  
                Debug.Log(filePath);
            }
            Directory.SetCurrentDirectory(projectFolder);
            return filePath     ;
        }

        public static string SaveFile(string title,string filter)
        {
            var projectFolder = Directory.GetCurrentDirectory();
            FileDialogBase pth = new FileDialogBase();
            pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
            pth.filter = filter;
            pth.file = new string(new char[256]);
            pth.maxFile = pth.file.Length;
            pth.fileTitle = new string(new char[64]);
            pth.maxFileTitle = pth.fileTitle.Length;
            pth.initialDir = Application.dataPath;  // default path  
            pth.title = title;
            pth.defExt = "txt";
            pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
            string filePath = null;
            if (SaveFileDialog.GetSaveFileName(pth))
            {
                filePath = pth.file;//选择的文件路径;  
                Debug.Log(filePath);
            }
            Directory.SetCurrentDirectory(projectFolder);
            return filePath;
        }

        public static void SerializableObjectToFile(string path,object obj)
        {
            FileStream fs;
            if ( File.Exists(path) )
            {
                fs = new FileStream(path, FileMode.Truncate);
            }
            else
            {
                fs = new FileStream(path, FileMode.Create);
            }
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, obj);
            fs.Close();
        }

        public static object DeserializeFileToObject(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(fs);
        }

        public static void WriteToFile(string data,string path)
        {
            StreamWriter file = new StreamWriter(path, false);
            //保存数据到文件
            file.Write(data);
            //关闭文件
            file.Close();
            //释放对象
            file.Dispose();
        }
    }



    class OpenFileDialog
    {
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] FileDialogBase ofd);
    }

    public class SaveFileDialog
    {
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] FileDialogBase ofd);
    }
}
