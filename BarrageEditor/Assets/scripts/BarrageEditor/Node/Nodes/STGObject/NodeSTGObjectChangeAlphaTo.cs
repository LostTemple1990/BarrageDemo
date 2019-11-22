using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeSTGObjectChangeAlphaTo : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.STGObjectChangeAlphaTo;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "changealphato");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 结束alpha值
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "To alpha", null);
            _attrs.Add(nodeAttr);
            // 持续时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Duration", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "change alpha of object in frame(s)";
        }

        public override string ToDesc()
        {
            return string.Format("change {0}'s alpha to ({1}) in {2} frame(s)",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(), 
                GetAttrByIndex(2).GetValueString());
        }

        public override string ToLuaHead()
        {
            string ret = string.Format("{0}:ChangeAlphaTo({1},{2})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(), 
                GetAttrByIndex(2).GetValueString());
            return ret;
        }
    }
}
