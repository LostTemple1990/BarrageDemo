using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateSTGObject : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateCusomizedSTGObject;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "objectcreate");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 自定义物体名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CustomizedType);
            nodeAttr.Init(this, "Type name", null);
            attrs.Add(nodeAttr);
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.ParaList);
            nodeAttr.Init(this, "Parameter list", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create object";
        }

        public override string ToDesc()
        {
            string ret = string.Format("create object with of type\"{0}\"", attrs[0].GetValueString());
            if (GetAttrByIndex(1).GetValueString() != "")
            {
                ret = ret + string.Format(" with parameter {0}", attrs[1].GetValueString());
            }
            return ret;
        }

        public override string ToLuaHead()
        {
            string typeName = attrs[0].GetValueString();
            CustomDefineData data = CustomDefine.GetDataByTypeAndName(CustomDefineType.Enemy, typeName);
            int paraCount = 0;
            if (data != null)
            {
                if (data.paraListStr.IndexOf(',') != -1)
                {
                    paraCount = data.paraListStr.Split(',').Length;
                }
            }
            return string.Format("last = CreateCustomizedSTGObject(\"{0}\"{1})\n",
                typeName,
                GetAttrByIndex(1).GetValueString() == "" ? "" : "," + GetAttrByIndex(1).GetValueString());
        }
    }
}
