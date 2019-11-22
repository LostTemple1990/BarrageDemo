using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeDropItems : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.DropItems;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "dropitems");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 中心X坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "CenterX", null);
            _attrs.Add(nodeAttr);
            // 中心Y坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "CenterY", null);
            _attrs.Add(nodeAttr);
            // 掉落矩形半宽
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "RectHalfWidth", null);
            _attrs.Add(nodeAttr);
            // 掉落矩形半高
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "RectHalfHeight", null);
            _attrs.Add(nodeAttr);
            // 表达式
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.DropItems);
            nodeAttr.Init(this, "Item List", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "drop items";
        }

        public override string ToDesc()
        {
            BaseNodeAttr nodeAttr;
            string ret = string.Format("drop items ");
            nodeAttr = GetAttrByIndex(4);
            if (nodeAttr.GetValueString() != "")
            {
                ret = ret + string.Format(" with itemList {0} ", nodeAttr.GetValueString());
            }
            ret = string.Format("{0}at ({1},{2}) in rect (halfwidth={3},halfHeight={4})", ret,
                 GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString(),
                 GetAttrByIndex(2).GetValueString(), GetAttrByIndex(3).GetValueString());
            return ret;
        }

        public override string ToLuaHead()
        {
            if (GetAttrByIndex(4).GetValueString() == "")
            {
                return "";
            }
            string itemListStr = GetAttrByIndex(4).GetValueString();
            string ret = string.Format("DropItems({0},{1},{2},{3},{4})\n", 
                GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString(), 
                GetAttrByIndex(2).GetValueString(), GetAttrByIndex(3).GetValueString(), 
                itemListStr);
            return ret;
        }
    }
}
