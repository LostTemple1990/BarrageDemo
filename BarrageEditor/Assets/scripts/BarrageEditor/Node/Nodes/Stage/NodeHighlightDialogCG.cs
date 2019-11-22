using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeHighlightDialogCG : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.HighlightDialogCG;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "highlightdialogcg");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 自定义标识名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Name", null);
            _attrs.Add(nodeAttr);
            // 高亮、取消高亮
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Highlight", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "highlight dialog CG";
        }

        public override string ToDesc()
        {
            return string.Format("{0}highlight dialog CG of name \"{1}\"",
                GetAttrByIndex(1).GetValueString() == "true" ? "" : "cancel ",
                GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("HighlightDialogCG(\"{0}\",{1})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString());
        }
    }
}
