using UnityEngine;
using UnityEngine.UI;

namespace BarrageEditor
{
    public class NodeRoot : BaseNode
    {
        public override void Init(BaseNode parent, RectTransform parentTf)
        {
            base.Init(parent, parentTf);
            _expandImg.gameObject.SetActive(false);
            // 移动功能标识图像
            RectTransform rect = _functionImg.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, 0);
            // 移动描述文本的位置
            rect = _selectedImg.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(31, 0);
        }

        public override string GetNodeName()
        {
            return "project";
        }

        public override string ToDesc()
        {
            return "project";
        }
    }
}
