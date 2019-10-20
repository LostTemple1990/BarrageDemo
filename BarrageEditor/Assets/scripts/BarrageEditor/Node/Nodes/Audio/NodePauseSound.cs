using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodePauseSound : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.PauseSound;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "pausebgm");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 音效名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Sound name", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "pause sound";
        }

        public override string ToDesc()
        {
            return string.Format("pause sound with name \"{0}\"", attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("PauseSound(\"{0}\")\n",
                attrs[0].GetValueString());
        }
    }
}
