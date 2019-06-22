using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeIfElse : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.IfElse;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "else");
        }

        public override string GetNodeName()
        {
            return "else";
        }

        public override string ToDesc()
        {
            return string.Format("else");
        }

        public override string ToLuaHead()
        {
            return string.Format("else\n");
        }
    }
}