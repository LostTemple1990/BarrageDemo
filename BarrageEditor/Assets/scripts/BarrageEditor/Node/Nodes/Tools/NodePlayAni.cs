using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodePlayAni : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.PlayAni;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "playani");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 播放动画的单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 动作类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.AniActionType);
            nodeAttr.Init(this, "ActionType", null);
            _attrs.Add(nodeAttr);
            // 方向
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Direction);
            nodeAttr.Init(this, "Direction", null);
            _attrs.Add(nodeAttr);
            // 循环次数
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "LoopCount", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "unit play ani";
        }

        public override string ToDesc()
        {
            BaseNodeAttr nodeAttr = GetAttrByIndex(3);
            string loopCountStr = nodeAttr.GetValueString() == "" ? "" : " , loopCount = " + nodeAttr.GetValueString();
            string ret = "";
            ret = string.Format("{0} play Ani \"{1}\" with {2}{3}",
                 GetAttrByIndex(0).GetValueString(), 
                 GetAttrByIndex(1).GetValueString(),GetAttrByIndex(2).GetValueString(),
                 loopCountStr);
            return ret;
        }

        public override string ToLuaHead()
        {
            string ret = string.Format("{0}:PlayAni({1},{2}{3})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString() == "" ? "" : "," + GetAttrByIndex(3).GetValueString());
            return ret;
        }
    }
}
