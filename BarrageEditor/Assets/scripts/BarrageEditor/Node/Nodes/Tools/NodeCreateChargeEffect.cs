using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateChargeEffect : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateChargeEffect;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "charge");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 坐标X
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosX", null);
            _attrs.Add(nodeAttr);
            // 坐标Y
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosY", null);
            _attrs.Add(nodeAttr);
            // 缩放
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Scale", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create charge effect";
        }

        public override string ToDesc()
        {
            return string.Format("create charge effect at ({0},{1})",
                _attrs[0].GetValueString(), _attrs[1].GetValueString());
        }

        public override string ToLuaHead()
        {
            string scaleParam = GetAttrByIndex(2).GetValueString() == "" ? "" : string.Format(",{0}", GetAttrByIndex(2).GetValueString());
            return string.Format("CreateChargeEffect({0},{1}{2})\n",
                GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString(),
                scaleParam);
        }
    }
}
