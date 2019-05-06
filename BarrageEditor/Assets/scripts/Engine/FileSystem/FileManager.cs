using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace YKEngine
{
    public class FileManager
    {
        public static string OpenFile(string title,string filter,bool multiSelect)
        {
            FileDialogBase pth = new FileDialogBase();
            pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
            pth.filter = "txt (*.txt)";
            pth.file = new string(new char[256]);
            pth.maxFile = pth.file.Length;
            pth.fileTitle = new string(new char[64]);
            pth.maxFileTitle = pth.fileTitle.Length;
            pth.initialDir = Application.dataPath;  // default path  
            pth.title = "打开项目";
            pth.defExt = "txt";
            pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
            if (OpenFileDialog.GetOpenFileName(pth))
            {
                string filepath = pth.file;//选择的文件路径;  
                Debug.Log(filepath);
                return filepath;
            }
            return null;
        }
    }

    class OpenFileDialog
    {
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] FileDialogBase ofd);
    }
}
