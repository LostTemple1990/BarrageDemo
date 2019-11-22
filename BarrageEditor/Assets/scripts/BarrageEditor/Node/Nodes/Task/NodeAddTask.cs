using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAddTask : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.AddTask;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "task");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // Unit
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "add task";
        }

        public override string ToDesc()
        {
            return string.Format("{0} add task", GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:AddTask(function()\n", GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaFoot()
        {
            return string.Format("end)\n");
        }
    }
}
