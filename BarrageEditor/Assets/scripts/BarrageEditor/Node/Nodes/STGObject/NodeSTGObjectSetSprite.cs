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
            attrs.Add(nodeAttr);
            // 图集名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Atlas name", null);
            attrs.Add(nodeAttr);
            // 精灵名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Sprite name", null);
            attrs.Add(nodeAttr);
            // 混合模式
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.BlendMode);
            nodeAttr.Init(this, "Blend mode", null);
            attrs.Add(nodeAttr);
            // 层级
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Layer);
            nodeAttr.Init(this, "Layer", null);
            attrs.Add(nodeAttr);
            // 是否缓存
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Cached", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "set sprite for object";
        }

        public override string ToDesc()
        {
            return string.Format("set sprite {0}/{1} for {1} in {2} with {3}", GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:SetStyleById({1})\n", GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString());
        }
    }
}
