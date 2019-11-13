using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class AttrEditReboundView : ViewBase
    {

        private GameObject _closeBtn;
        private GameObject _okBtn;

        private RectTransform _itemContainerTf;

        class ReboundItem
        {
            public GameObject go;
            public Toggle toggle;
        }

        private int _curSelectedItemIndex;
        private List<string> _paraNameList;
        private Dictionary<eReboundBorder, ReboundItem> _itemDic;
        private List<eReboundBorder> _defaultGroups;

        private BaseNodeAttr _nodeAttr;

        protected override void Init()
        {
            _closeBtn = _viewTf.Find("Panel/CancelBtn").gameObject;
            _okBtn = _viewTf.Find("Panel/OKBtn").gameObject;
            _itemContainerTf = _viewTf.Find("Panel/Window").GetComponent<RectTransform>();

            InitGroupItems();
        }

        private void InitGroupItems()
        {
            // 可编辑的碰撞枚举
            _defaultGroups = new List<eReboundBorder> {
                eReboundBorder.Left, eReboundBorder.Right, eReboundBorder.Top, eReboundBorder.Bottom };
            _itemDic = new Dictionary<eReboundBorder, ReboundItem>();
            for (int i = 0; i < _defaultGroups.Count; i++)
            {
                GameObject item = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "EditReboundItem");
                RectTransform tf = item.GetComponent<RectTransform>();
                tf.SetParent(_itemContainerTf, false);
                ReboundItem itemCls = new ReboundItem
                {
                    go = item,
                    toggle = tf.Find("Toggle").GetComponent<Toggle>(),
                };
                Text reboundText = tf.Find("ReboundText").GetComponent<Text>();
                reboundText.text = _defaultGroups[i].ToString();
                _itemDic.Add(_defaultGroups[i], itemCls);
            }
        }

        private void AddListeners()
        {
            UIEventListener.Get(_closeBtn).AddClick(OnCloseBtnClickHandler);
            UIEventListener.Get(_okBtn).AddClick(OnOKBtnHandler);
        }

        public override void OnShow(object data)
        {
            _nodeAttr = data as BaseNodeAttr;
            UpdateGroupItems();
            AddListeners();
        }

        private void UpdateGroupItems()
        {
            int selectedGroups = int.Parse(_nodeAttr.GetValueString());
            ReboundItem item;
            for (int i = 0; i < _defaultGroups.Count; i++)
            {
                int group = (int)_defaultGroups[i];
                item = _itemDic[(eReboundBorder)group];
                // 设置
                if ((group & selectedGroups) != 0)
                {
                    item.toggle.isOn = true;
                }
                else
                {
                    item.toggle.isOn = false;
                }
            }
        }

        private void OnOKBtnHandler()
        {
            int selectedGroups = 0;
            ReboundItem item;
            for (int i = 0; i < _defaultGroups.Count; i++)
            {
                int group = (int)_defaultGroups[i];
                item = _itemDic[(eReboundBorder)group];
                if (item.toggle.isOn)
                {
                    selectedGroups |= group;
                }
            }
            _nodeAttr.SetValue(selectedGroups.ToString());
            Close();
        }

        private void OnCloseBtnClickHandler()
        {
            Close();
        }

        public override void OnClose()
        {
            RemoveListeners();
        }

        private void RemoveListeners()
        {
            UIEventListener.Get(_closeBtn).RemoveAllEvents();
            UIEventListener.Get(_okBtn).RemoveAllEvents();
        }
    }
}
