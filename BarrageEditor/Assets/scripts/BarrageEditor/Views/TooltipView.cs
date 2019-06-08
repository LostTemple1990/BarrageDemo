using System;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class TooltipView : ViewBase
    {
        /// <summary>
        /// tip位置与鼠标位置Y轴偏移
        /// </summary>
        private const float TipYOffset = -20;

        private const float ExtraWidth = 20;
        private const float ExtraHeight = 10;

        private RectTransform _bgTf;
        private Text _tipText;
        private RectTransform _tipTextTf;

        protected override void Init()
        {
            _bgTf = _viewTf.Find("Bg").GetComponent<RectTransform>();
            _tipTextTf = _viewTf.Find("Bg/Text").GetComponent<RectTransform>();
            _tipText = _tipTextTf.GetComponent<Text>();
            _tipText.RegisterDirtyLayoutCallback(DirtyLayoutCallback);
        }

        public override void OnShow(object data)
        {
            Refresh(data);
        }

        private void DirtyLayoutCallback()
        {
            float width = _tipText.preferredWidth;
            float height = _tipText.preferredHeight;

            _bgTf.sizeDelta = new Vector2(width + ExtraWidth, height + ExtraHeight);
            _tipTextTf.sizeDelta = new Vector2(width, height);
            _tipTextTf.anchoredPosition = new Vector2((width + ExtraWidth) / 2, 0);
        }

        public override void Refresh(object data)
        {
            string tipStr = data as String;
            _tipText.text = tipStr;
            UpdateTipPosition();
        }

        private void UpdateTipPosition()
        {
            Vector2 mousePos = Input.mousePosition;
            mousePos.y -= 20;
            UIManager.GetInstance().SetUIPosition(_viewTf, mousePos);
        }
    }
}
