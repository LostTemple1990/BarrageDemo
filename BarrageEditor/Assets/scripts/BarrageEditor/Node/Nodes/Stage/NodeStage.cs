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
            // 背景
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Background", null);
            _attrs.Add(nodeAttr);
            // bgm
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "BGM", null);
            _attrs.Add(nodeAttr);
            // FPS同步
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "FixedFPS", null);
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
            string ret = string.Format("Stage[\"{0}\"] = {{ bg=\"{1}\",bgm=\"{2}\",fixedFPS={3} }}\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString());
            ret += string.Format("Stage[\"{0}\"].task = function(self)\n", GetAttrByIndex(0).GetValueString());
            return ret;
        }

        public override string ToLuaFoot()
        {
            return string.Format("    FinishStage()\nend\n");
        }
    }
}
