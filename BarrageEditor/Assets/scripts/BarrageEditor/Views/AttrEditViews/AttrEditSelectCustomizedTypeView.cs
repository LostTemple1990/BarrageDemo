using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class AttrEditSelectCustomizedTypeView : ViewBase
    {
        private static Color UnSelectedColor = new Color(0, 0, 1, 0);
        private static Color SelectedColor = new Color(0, 0, 1, 1);

        private GameObject _closeBtn;
        private GameObject _okBtn;

        private ScrollRect _contentScrollRect;
        private Scrollbar _contentScrollbar;
        private RectTransform _itemContainerTf;

        private int _curSelectedItemIndex;
        private List<String> _typeNameList;
        private List<GameObject> _itemList;

        private BaseNodeAttr _nodeAttr;

        private Vector2 _contentDefaultSize;
        private float _contentPreferredHeight;

        protected override void Init()
        {
            _closeBtn = _viewTf.Find("Panel/CancelBtn").gameObject;
            _okBtn = _viewTf.Find("Panel/OKBtn").gameObject;
            _contentScrollRect = _viewTf.Find("Panel/Window/ScrollView").GetComponent<ScrollRect>();
            _contentScrollbar = _viewTf.Find("Panel/Window/ScrollView/Scrollbar Vertical").GetComponent<Scrollbar>();
            _itemContainerTf = _viewTf.Find("Panel/Window/ScrollView/Viewport/Content").GetComponent<RectTransform>();
            _contentDefaultSize = _itemContainerTf.sizeDelta;
            _contentPreferredHeight = _contentDefaultSize.y;

            _itemList = new List<GameObject>();

            AddListeners();
        }

        private void AddListeners()
        {
            UIEventListener.Get(_closeBtn).AddClick(OnCloseBtnClickHandler);
            UIEventListener.Get(_okBtn).AddClick(OnOKBtnHandler);
        }

        public override void OnShow(object data)
        {
            _nodeAttr = data as BaseNodeAttr;
            InitTypeItems();
        }

        private void InitTypeItems()
        {
            NodeType nodeType = _nodeAttr.Node.GetNodeType();
            CustomDefineType type = CustomDefine.GetTypeByNodeType(nodeType);
            _typeNameList = CustomDefine.GetCustomDefineListByType(type);
            for (int i=0;i< _typeNameList.Count;i++)
            {
                GameObject item = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "SelectCustomizedTypeItem");
                RectTransform tf = item.GetComponent<RectTransform>();
                tf.SetParent(_itemContainerTf, false);
                Text typeNameText = tf.Find("CustomizedTypeText").GetComponent<Text>();
                typeNameText.text = _typeNameList[i];
                item.GetComponent<Image>().color = UnSelectedColor;
                int itemIndex = i;
                UIEventListener.Get(item).AddClick(() =>{
                    OnItemClickHandler(itemIndex, false);
                });
                _itemList.Add(item);
            }
            // 计算content面板的高度
            float preferredHeight = _typeNameList.Count * 30 + 5;
            _contentPreferredHeight = preferredHeight < _contentDefaultSize.y ? _contentDefaultSize.y : preferredHeight;
            _itemContainerTf.sizeDelta = new Vector2(_contentDefaultSize.x, _contentPreferredHeight);
            _contentScrollRect.Rebuild(CanvasUpdate.PostLayout);
            _curSelectedItemIndex = -1;
            // 默认选中节点值的那个item，如果没有对应的，则不选中任何一个
            int index = _typeNameList.IndexOf(_nodeAttr.GetValueString());
            OnItemClickHandler(index, true);
        }

        /// <summary>
        /// <para>bool scrollToItem 是否将滚动条设置到item处</para>
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <param name="scrollToItem"></param>
        private void OnItemClickHandler(int itemIndex,bool scrollToItem)
        {
            if ( _curSelectedItemIndex != -1 )
            {
                _itemList[_curSelectedItemIndex].GetComponent<Image>().color = UnSelectedColor;
            }
            _curSelectedItemIndex = itemIndex;
            if (_curSelectedItemIndex == -1) return;
            _itemList[_curSelectedItemIndex].GetComponent<Image>().color = SelectedColor;
            if (scrollToItem)
            {
                float toY = (itemIndex - 1) * 30 + 5;
                float maxY = _contentPreferredHeight - _contentDefaultSize.y;
                float value;
                if (toY >= maxY)
                    value = 0;
                else
                    value = 1 - toY / _contentPreferredHeight;
                _contentScrollbar.value = value;
            }
        }

        private void OnOKBtnHandler()
        {
            if ( _curSelectedItemIndex != -1 )
            {
                _nodeAttr.SetValue(_typeNameList[_curSelectedItemIndex]);
            }
            Close();
        }

        private void OnCloseBtnClickHandler()
        {
            Close();
        }

        public override void OnClose()
        {
            for (int i=0;i<_itemList.Count;i++)
            {
                GameObject.Destroy(_itemList[i]);
            }
            _typeNameList.Clear();
            _itemList.Clear();
        }
    }
}
