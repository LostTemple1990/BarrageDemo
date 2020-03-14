using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class AttrEditRadioView : ViewBase
    {
        private const float ItemHeight = 40f;
        private const float WindowTopBottom = 65 + 25 + 20;
        /// <summary>
        /// Panel的最小高度
        /// </summary>
        private const float PanelMinHeight = 360f;

        private GameObject _closeBtn;
        private GameObject _okBtn;

        private RectTransform _panelTf;
        private RectTransform _itemContainerTf;
        private Text _titleText;

        class RadioItem
        {
            public GameObject go;
            public Toggle toggle;
            public string value;
        }

        private int _curSelectedIndex;
        List<RadioItem> _radioItemList;

        private BaseNodeAttr _nodeAttr;

        protected override void Init()
        {
            _panelTf = _viewTf.Find("Panel").GetComponent<RectTransform>();
            _closeBtn = _panelTf.Find("CancelBtn").gameObject;
            _okBtn = _panelTf.Find("OKBtn").gameObject;
            _titleText = _panelTf.Find("Title").GetComponent<Text>();
            _itemContainerTf = _panelTf.Find("Window").GetComponent<RectTransform>();

            _radioItemList = new List<RadioItem>();
            AddListeners();
        }

        private void AddListeners()
        {
            UIEventListener.Get(_closeBtn).AddClick(OnCloseBtnClickHandler);
            UIEventListener.Get(_okBtn).AddClick(OnOKBtnHandler);
        }

        public override void OnShow(object data)
        {
            List<object> datas = data as List<object>;
            _nodeAttr = datas[0] as BaseNodeAttr;
            SetTitle(datas[1] as string);
            InitRadioItems(datas[2] as string[]);
            ResizePanel();
        }

        private void SetTitle(string title)
        {
            _titleText.text = title;
        }

        private void InitRadioItems(string[] datas)
        {
            string attrValue = _nodeAttr.GetValueString();
            RadioItem item;
            for (int i = 0; i < datas.Length; i++)
            {
                GameObject itemGo = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "EditRadioItem");
                RectTransform tf = itemGo.GetComponent<RectTransform>();
                tf.SetParent(_itemContainerTf, false);
                item = new RadioItem
                {
                    go = itemGo,
                    toggle = tf.Find("Toggle").GetComponent<Toggle>(),
                    value = datas[i],
                };
                Text valueText = tf.Find("ValueText").GetComponent<Text>();
                valueText.text = datas[i];
                int tmpIndex = i;
                UIEventListener.Get(item.toggle.gameObject).AddClick(() => {
                    if (_curSelectedIndex != -1)
                    {
                        RadioItem tmpItem = _radioItemList[_curSelectedIndex];
                        tmpItem.toggle.isOn = false;
                    }
                    _curSelectedIndex = tmpIndex;
                });
                _radioItemList.Add(item);
                // 默认选择
                if (attrValue == datas[i])
                {
                    item.toggle.isOn = true;
                    _curSelectedIndex = i;
                }
            }
        }

        private void ResizePanel()
        {
            int countInVertical = (_radioItemList.Count + 1) / 2;
            float preferredHeight = countInVertical * ItemHeight + WindowTopBottom;
            if (preferredHeight < PanelMinHeight)
                preferredHeight = PanelMinHeight;
            // Resize
            Vector2 size = _panelTf.sizeDelta;
            size.y = preferredHeight;
            _panelTf.sizeDelta = size;
        }

        private void OnOKBtnHandler()
        {
            string curValue = _radioItemList[_curSelectedIndex].value;
            _nodeAttr.SetValue(curValue);
            Close();
        }

        private void OnCloseBtnClickHandler()
        {
            Close();
        }

        public override void OnClose()
        {
            for (int i = 0; i < _radioItemList.Count; i++)
            {
                UIEventListener.Get(_radioItemList[i].go).RemoveAllEvents();
                GameObject.Destroy(_radioItemList[i].go);
                _radioItemList[i].go = null;
                _radioItemList[i].toggle = null;
            }
            _radioItemList.Clear();
        }
    }
}
