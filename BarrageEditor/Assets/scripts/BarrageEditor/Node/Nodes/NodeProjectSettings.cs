using UnityEngine;
using UnityEngine.UI;

namespace BarrageEditor
{
    public class NodeProjectSettings : BaseNode
    {
        public override void Init(BaseNode parent, RectTransform parentTf)
        {
            base.Init(parent, parentTf);
        }

        public override string ToDesc()
        {
            return "project settings";
        }
    }
}
