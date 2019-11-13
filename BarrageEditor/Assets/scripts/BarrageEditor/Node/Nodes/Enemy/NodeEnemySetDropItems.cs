using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeEnemySetDropItems : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.SetDropItems;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "dropitem");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            attrs.Add(nodeAttr);
            // 中心X坐标
            // 中心Y坐标
            //nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            //nodeAttr.Init(this, "CenterY", null);
            //attrs.Add(nodeAttr);
            // 掉落矩形半宽
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "RectHalfWidth", null);
            attrs.Add(nodeAttr);
            // 掉落矩形半高
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "RectHalfHeight", null);
            attrs.Add(nodeAttr);
            // 表达式
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.DropItems);
            nodeAttr.Init(this, "Item List", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "enemy drop item";
        }

        public override string ToDesc()
        {
            BaseNodeAttr nodeAttr;
            string ret = string.Format("set {0}'s drop items", GetAttrByIndex(0).GetValueString());
            nodeAttr = GetAttrByIndex(3);
            if (nodeAttr.GetValueString() != "")
            {
                ret = ret + string.Format(" with itemList {0}", nodeAttr.GetValueString());
            }
            ret = string.Format("{0} in rect (halfwidth={1},halfHeight={2})", ret, GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString());
            return ret;
        }

        public override string ToLuaHead()
        {
            if (GetAttrByIndex(3).GetValueString() == "")
            {
                return "";
            }
            string itemListStr = GetAttrByIndex(3).GetValueString();
            string ret = string.Format("{0}:SetDropItems({1},{2},{3})\n",
                GetAttrByIndex(0).GetValueString(),
                itemListStr,
                GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString());
            return ret;
        }
    }
}
