using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeDialogWait : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.DialogWait;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "dialogwait");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // wait for frames
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "nFrame", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "dialog wait";
        }

        public override string ToDesc()
        {
            return string.Format("wait {0} frame(s)", GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("if Wait({0})==false then return end\n", GetAttrByIndex(0).GetValueString());
        }
    }
}
