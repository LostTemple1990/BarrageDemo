using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeIfThen : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.IfThen;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "then");
        }

        public override string GetNodeName()
        {
            return "then";
        }

        public override string ToDesc()
        {
            return string.Format("then");
        }
    }
}
