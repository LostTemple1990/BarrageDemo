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

        private RectTransform _itemContainerTf;

        private int _curSelectedItemIndex;
        private List<String> _typeNameList;
        private List<GameObject> _itemList;

        private BaseNodeAttr _nodeAttr;

        protected override void Init()
        {
            _closeBtn = _viewTf.Find("Panel/CancelBtn").gameObject;
            _okBtn = _viewTf.Find("Panel/OKBtn").gameObject;
            _itemContainerTf = _viewTf.Find("Panel/Window/ScrollView/Viewport/Content").GetComponent<RectTransform>();

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
            CustomDefineType type = CustomDefineType.SimpleBullet;
            if ( _nodeAttr.Node.GetNodeType() == NodeType.CreateBullet )
            {
                type = CustomDefineType.SimpleBullet;
            }
            _typeNameList = CustomDefine.GetCustomDefineListByType(type);
            for (int i=0;i< _typeNameList.Count;i++)
            {
                GameObject item = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "SelectCustomizedTypeItem");
                RectTransform tf = item.GetComponent<RectTransform>();
                tf.SetParent(_itemContainerTf, false);
                Text typeNameText = tf.Find("CustomizedTypeText").GetComponent<Text>();
                typeNameText.text = _typeNameList[i];
                item.GetComponent<Image>().color = UnSelectedColor;
                _itemList.Add(item);
            }
            _curSelectedItemIndex = -1;
            // 默认选中节点值的那个item，如果没有对应的，则不选中任何一个
            int index = _typeNameList.IndexOf(_nodeAttr.GetValueString());
            OnItemClickHandler(index);
        }

        private void OnItemClickHandler(int itemIndex)
        {
            if ( _curSelectedItemIndex != -1 )
            {
                _itemList[_curSelectedItemIndex].GetComponent<Image>().color = UnSelectedColor;
            }
            _curSelectedItemIndex = itemIndex;
            if (_curSelectedItemIndex == -1) return;
            _itemList[_curSelectedItemIndex].GetComponent<Image>().color = SelectedColor;
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
