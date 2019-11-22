using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeUnitAttachTo : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.UnitAttachTo;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "connect");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 链接的单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Master", null);
            _attrs.Add(nodeAttr);
            // 是否跟随master一起被消除
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Eliminate with master", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "attach to";
        }

        public override string ToDesc()
        {
            return string.Format("set {0} as servant of {1}",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:AttachTo({1},{2})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString());
        }
    }
}
