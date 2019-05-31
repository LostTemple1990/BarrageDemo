using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public class BaseNodeAttr
    {
        public string attrName;
        public string attrDesc;
        public object value;
        public delegate bool CheckValueAvailable(object value);

        public CheckValueAvailable CheckValueFunc;

        public virtual void Init(string name,NodeAttrType type,CheckValueAvailable checkFunc)
        {
            
        }

        public virtual void SetValue(object value)
        {

        }

        public virtual string ToDesc()
        {
            return "undefined node attr desc";
        }

    }

    public enum NodeAttrType : byte
    {
        Any = 0,
        Bool = 1,
        Int = 2,
        String = 3, 
    }
}
