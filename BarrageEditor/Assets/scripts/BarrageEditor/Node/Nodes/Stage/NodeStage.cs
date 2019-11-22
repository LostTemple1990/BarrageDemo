using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeStage : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.Stage;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "stage");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // stage名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Name", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "stage";
        }

        public override string ToDesc()
        {
            return string.Format("stage \"{0}\"", GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("Stage[\"{0}\"] = function()\n", GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaFoot()
        {
            return string.Format("    FinishStage()\nend\n");
        }
    }
}
