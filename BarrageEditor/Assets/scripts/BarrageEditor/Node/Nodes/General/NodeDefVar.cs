using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeDefVar : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.DefVar;
            _extraDepth = 1;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "variable");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 变量名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Name", null);
            _attrs.Add(nodeAttr);
            // 变量值
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Initial value", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "define variable";
        }

        public override string ToDesc()
        {
            BaseNodeAttr nameAttr = GetAttrByIndex(0);
            BaseNodeAttr valueAttr = GetAttrByIndex(1);
            if ( valueAttr.GetValueString() == "" )
            {
                return string.Format("define local variable {0}", nameAttr.GetValueString());
            }
            else
            {
                return string.Format("define local variable {0} = {1}", nameAttr.GetValueString(), valueAttr.GetValueString());
            }
        }

        public override string ToLuaHead()
        {
            BaseNodeAttr nameAttr = GetAttrByIndex(0);
            BaseNodeAttr valueAttr = GetAttrByIndex(1);
            if (valueAttr.GetValueString() == "")
            {
                return string.Format("local {0}\n", nameAttr.GetValueString());
            }
            else
            {
                return string.Format("local {0} = {1}\n", nameAttr.GetValueString(), valueAttr.GetValueString());
            }
        }
    }
}
