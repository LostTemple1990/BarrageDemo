using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeFadeOutDialogCG : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.FadeoutDialogCG;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "dialogcgfadeout");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 自定义标识名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Name", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "fade out dialog CG";
        }

        public override string ToDesc()
        {
            return string.Format("fade out dialog CG of name \"{0}\"",
                GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("FadeOutDialogCG(\"{0}\")\n",
                GetAttrByIndex(0).GetValueString());
        }
    }
}
