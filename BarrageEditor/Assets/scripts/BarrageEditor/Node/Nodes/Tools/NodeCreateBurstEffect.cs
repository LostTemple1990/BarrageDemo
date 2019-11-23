using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateBurstEffect : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateBurstEffect;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "burst");
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
            return "create burst effect";
        }

        public override string ToDesc()
        {
            return string.Format("create burst effect at ({0},{1})",
                _attrs[0].GetValueString(), _attrs[1].GetValueString());
        }

        public override string ToLuaHead()
        {
            string scaleParam = GetAttrByIndex(2).GetValueString() == "" ? "" : string.Format(",{0}", GetAttrByIndex(2).GetValueString());
            return string.Format("CreateBurstEffect({0},{1}{2})\n",
                GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString(),
                scaleParam);
        }
    }
}
