using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public class Clipboard
    {
        private static object _data;

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
