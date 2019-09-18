using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeUnitEventTrigger : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.UnitEventTrigger;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "callbackfunc");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 触发事件
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.UnitEventType);
            nodeAttr.Init(this, "Event type", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "unit event trigger";
        }

        public override string ToDesc()
        {
            string descStr = "undefined event type";
            string evt = GetAttrByIndex(0).GetValueString();
            if (evt == "OnKill")
                descStr = "on kill";
            return string.Format("{0}", descStr);
        }

        public override string ToLuaHead()
        {
            string evt = GetAttrByIndex(0).GetValueString();
            if (evt == "OnKill")
            {
                if (parentNode.GetNodeType() == NodeType.DefineEnemy)
                {
                    return string.Format("CustomizedEnemyTable[\"{0}\"].OnKill = function(self)\n", parentNode.GetAttrByIndex(0).GetValueString());
                }
            }
            return "";
        }

        public override string ToLuaFoot()
        {
            string evt = GetAttrByIndex(0).GetValueString();
            if (evt != "OnKill")
                return "";
            return "end\n";
        }
    }
}
