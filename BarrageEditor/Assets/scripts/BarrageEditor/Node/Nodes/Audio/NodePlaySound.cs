using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodePlaySound : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.PlaySound;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "playbgm");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 音效名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Sound name", null);
            _attrs.Add(nodeAttr);
            // 音量大小
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Volume[0.0~1.0]", null);
            _attrs.Add(nodeAttr);
            // 是否循环
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Is loop", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "play sound";
        }

        public override string ToDesc()
        {
            return string.Format("play sound \"{0}\" with volume of {1},loop = {2}",
                _attrs[0].GetValueString(), _attrs[1].GetValueString(), _attrs[2].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("PlaySound(\"{0}\",{1},{2})\n",
                _attrs[0].GetValueString(), _attrs[1].GetValueString(), _attrs[2].GetValueString());
        }
    }
}
