using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeSTGObjectSetSprite : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.SetSpriteForSTGObject;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "objectsetimg");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 图集名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Atlas name", null);
            _attrs.Add(nodeAttr);
            // 精灵名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Sprite name", null);
            _attrs.Add(nodeAttr);
            // 混合模式
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.BlendMode);
            nodeAttr.Init(this, "Blend mode", null);
            _attrs.Add(nodeAttr);
            // 层级
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Layer);
            nodeAttr.Init(this, "Layer", null);
            _attrs.Add(nodeAttr);
            // 是否缓存
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Cached", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "set sprite for object";
        }

        public override string ToDesc()
        {
            return string.Format("set sprite {0}/{1} for {2} in {3} with {4}", 
                GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(4).GetValueString(), GetAttrByIndex(3).GetValueString());
        }

        public override string ToLuaHead()
        {
            string ret = string.Format("{0}:SetSprite({1},{2},{3},{4},{5})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString(),
                GetAttrByIndex(5).GetValueString());
            return ret;
        }
    }
}
