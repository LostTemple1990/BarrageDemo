using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeProjectSettings : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.ProjectSetting;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "setting");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 输出名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Output name", null);
            attrs.Add(nodeAttr);
            // 作者
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Author", null);
            attrs.Add(nodeAttr);
            // 是否允许练习模式
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Allow practice", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "project settings";
        }

        public override string ToDesc()
        {
            return "project settings";
        }

        public override string ToLuaHead()
        {
            return string.Format("-- Mod name: {0}\n--author=\"{1}\"\n--allow_practice={2}\n",
                attrs[0].GetValueString(), attrs[1].GetValueString(), attrs[2].GetValueString());
        }
    }
}
