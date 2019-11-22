using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeResumeSound : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.PauseSound;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "resumebgm");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 音效名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Sound name", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "resume sound";
        }

        public override string ToDesc()
        {
            return string.Format("resume sound with name \"{0}\"", _attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("ResumeSound(\"{0}\")\n",
                _attrs[0].GetValueString());
        }
    }
}
