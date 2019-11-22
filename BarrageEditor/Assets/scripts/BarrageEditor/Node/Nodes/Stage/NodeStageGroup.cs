using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeStageGroup : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.StageGroup;
            _extraDepth = 0;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "stagegroup");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // group名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Name", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "stage group";
        }

        public override string ToDesc()
        {
            return string.Format("stage group \"{0}\"", GetAttrByIndex(0).GetValueString());
        }
    }
}
