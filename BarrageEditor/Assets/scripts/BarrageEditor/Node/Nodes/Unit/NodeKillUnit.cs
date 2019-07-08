using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeKillUnit : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.KillUnit;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "unitkill");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            attrs.Add(nodeAttr);
            // 是否触发事件
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Trigger event", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "kill unit";
        }

        public override string ToDesc()
        {
            return string.Format("kill {0}", GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("KillUnit({0},{1})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString());
        }
    }
}
