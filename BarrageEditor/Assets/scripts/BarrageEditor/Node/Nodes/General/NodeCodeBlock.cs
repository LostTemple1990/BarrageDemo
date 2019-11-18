using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCodeBlock : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CodeBlock;
            _extraDepth = 1;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "codeblock");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 代码块名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Title", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "code block";
        }

        public override string ToDesc()
        {
            return attrs[0].GetValueString();
        }

        public override string ToLuaHead()
        {
            return string.Format("do  --{0}\n", GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaFoot()
        {
            return string.Format("end\n");
        }
    }
}
