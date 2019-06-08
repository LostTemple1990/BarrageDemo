using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeProjectSettings : BaseNode
    {
        public override void Init(BaseNode parent, RectTransform parentTf)
        {
            base.Init(parent, parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "setting");
        }

        public override void CreateDefailtAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 输出名称
            nodeAttr = NodeFactory.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Output name", null);
            nodeAttr.SetValue("unnamed");
            attrs.Add(nodeAttr);
            // 作者
            nodeAttr = NodeFactory.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Author", null);
            nodeAttr.SetValue("YK");
            attrs.Add(nodeAttr);
            // 是否允许练习模式
            nodeAttr = NodeFactory.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Allow practice", null);
            nodeAttr.SetValue("true");
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
    }
}
