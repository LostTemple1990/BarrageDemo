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
            // 物品1
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.ItemType);
            nodeAttr.Init(this, "Item 1 type", null);
            attrs.Add(nodeAttr);
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Item 1 count", null);
            attrs.Add(nodeAttr);
            // 物品2
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.ItemType);
            nodeAttr.Init(this, "Item 2 type", null);
            attrs.Add(nodeAttr);
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Item 2 count", null);
            attrs.Add(nodeAttr);
            // 表达式
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
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
            nodeAttr = GetAttrByIndex(7);
            if (nodeAttr.GetValueString() != "")
            {
                ret = ret + string.Format(" with itemList {0}", nodeAttr.GetValueString());
            }
            else
            {
                // 从3开始
                int startIndex = 3;
                int count = 0;
                for (int i=0;i<2;i+=2)
                {
                    nodeAttr = GetAttrByIndex(startIndex + i);
                    if (nodeAttr.GetValueString() != "")
                    {
                        if (count != 0)
                        {
                            ret += ",";
                        }
                        ret += nodeAttr.GetValueString();
                        count++;
                    }
                }
            }
            ret = string.Format("{0} in rect (halfwidth={1},halfHeight={2})", ret, GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString());
            return ret;
        }

        public override string ToLuaHead()
        {
            if (GetAttrByIndex(3).GetValueString() == "" &&
                GetAttrByIndex(5).GetValueString() == "" &&
                GetAttrByIndex(7).GetValueString() == "")
            {
                return "";
            }
            string itemListStr;
            if (GetAttrByIndex(7).GetValueString() != "")
            {
                itemListStr = GetAttrByIndex(7).GetValueString();
            }
            else
            {
                int startIndex = 3;
                int counter = 0;
                BaseNodeAttr nodeAttr;
                itemListStr = "";
                for (int i = 0; i < 2; i += 2)
                {
                    nodeAttr = GetAttrByIndex(startIndex + i);
                    if (nodeAttr.GetValueString() != "")
                    {
                        if (counter == 0)
                            itemListStr += string.Format("{0},{1}", nodeAttr.GetValueString(), GetAttrByIndex(startIndex + i + 1).GetValueString());
                        else
                            itemListStr += string.Format(",{0},{1}", nodeAttr.GetValueString(), GetAttrByIndex(startIndex + i + 1).GetValueString());
                        counter++;
                    }
                }
            }
            string ret = string.Format("{0}:SetDropItems({1},{2},{3})",
                GetAttrByIndex(0).GetValueString(),
                itemListStr,
                GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString());
            return ret;
        }
    }
}
