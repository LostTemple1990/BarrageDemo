using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeStartDialog : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.StartDialog;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "dialog");
        }

        public override void CreateDefaultAttrs()
        {
            
        }

        public override void CreateDefualtChilds()
        {
            // comment
            BaseNode commentNode = NodeManager.CreateNode(NodeType.Comment);
            commentNode.GetAttrByIndex(0).SetValue("insert dialog node here");
            InsertChildNode(commentNode, -1);
        }

        public override string GetNodeName()
        {
            return "start dialog";
        }

        public override string ToDesc()
        {
            return string.Format("StartDialog");
        }

        public override string ToLuaHead()
        {
            return string.Format("if StartDialog(function()\n");
        }

        public override string ToLuaFoot()
        {
            return string.Format("end) == false then return end\n");
        }
    }
}
