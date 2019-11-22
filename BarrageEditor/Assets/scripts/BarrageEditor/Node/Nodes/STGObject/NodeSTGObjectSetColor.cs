using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeSTGObjectSetColor : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.STGObjectSetColor;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "setcolor");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // Red value
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Red value", null);
            _attrs.Add(nodeAttr);
            // Green value
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Green value", null);
            _attrs.Add(nodeAttr);
            // Blue value
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Blue value", null);
            _attrs.Add(nodeAttr);
            // Alpha
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Alpha", null);
            _attrs.Add(nodeAttr);
            // 混合模式
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.BlendMode);
            nodeAttr.Init(this, "Blend mode", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "set color and blend mode of object";
        }

        public override string ToDesc()
        {
            return string.Format("set {0}'s color to ({1},{2},{3},{4}) and blend mode to \"{5}\"",
                GetAttrByIndex(0).GetValueString(), 
                GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString(), GetAttrByIndex(3).GetValueString(), GetAttrByIndex(4).GetValueString(),
                GetAttrByIndex(5).GetValueString());
        }

        public override string ToLuaHead()
        {
            string ret = string.Format("{0}:SetColor({1},{2},{3},{4})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),GetAttrByIndex(2).GetValueString(),GetAttrByIndex(3).GetValueString(),GetAttrByIndex(4).GetValueString());
            if (GetAttrByIndex(5).GetValueString() != "")
            {
                ret = string.Format("{0}{1}:SetBlendMode({2})\n",
                    ret,
                    GetAttrByIndex(0).GetValueString(),
                    GetAttrByIndex(5).GetValueString());
            }
            return ret;
        }
    }
}
