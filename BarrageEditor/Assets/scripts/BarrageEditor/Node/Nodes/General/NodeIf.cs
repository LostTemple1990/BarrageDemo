using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeIf : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.If;
            _extraDepth = 0;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "if");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Condition", null);
            attrs.Add(nodeAttr);
        }

        public override void CreateDefualtChilds()
        {
            // 添加then,else
            BaseNode ifThenNode = NodeManager.CreateNode(NodeType.IfThen);
            ifThenNode.SetAttrsDefaultValues();
            InsertChildNode(ifThenNode, -1);
            BaseNode ifElseNode = NodeManager.CreateNode(NodeType.IfElse);
            ifElseNode.SetAttrsDefaultValues();
            InsertChildNode(ifElseNode, -1);
        }

        public override string GetNodeName()
        {
            return "if";
        }

        public override string ToDesc()
        {
            return string.Format("if {0}", attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("if {0} then\n", attrs[0].GetValueString());
        }

        public override string ToLuaFoot()
        {
            return string.Format("end\n");
        }
    }
}
