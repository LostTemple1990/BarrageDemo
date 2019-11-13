using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class AttrEditDropItemsView : ViewBase
    {

        private GameObject _closeBtn;
        private GameObject _okBtn;

        private RectTransform _itemContainerTf;

        struct EditItem
        {
            public eDropItemType type;
            public GameObject go;
            public InputField valueText;
        }

        private int _curSelectedItemIndex;
        private List<eDropItemType> _itemTypeList;
        private List<EditItem> _itemList;

        private BaseNodeAttr _nodeAttr;

        protected override void Init()
        {
            _closeBtn = _viewTf.Find("Panel/CancelBtn").gameObject;
            _okBtn = _viewTf.Find("Panel/OKBtn").gameObject;
            _itemContainerTf = _viewTf.Find("Panel/Window").GetComponent<RectTransform>();

            InitItems();
        }

        private void InitItems()
        {
            _itemTypeList = new List<eDropItemType> { eDropItemType.PowerNormal, eDropItemType.PowerBig, eDropItemType.PowerFull,
                eDropItemType.LifeFragment, eDropItemType.Life, eDropItemType.BombFragment, eDropItemType.Bomb };
            _itemList = new List<EditItem>();
            for (int i = 0; i < _itemTypeList.Count; i++)
            {
                GameObject itemGo = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "EditDropItem");
                RectTransform tf = itemGo.GetComponent<RectTransform>();
                tf.SetParent(_itemContainerTf, false);
                EditItem editItem = new EditItem
                {
                    type = _itemTypeList[i],
                    go = itemGo,
                    valueText = tf.Find("ItemCountField").GetComponent<InputField>(),
                };
                Text nameText = tf.Find("ItemNameText").GetComponent<Text>();
                nameText.text = _itemTypeList[i].ToString();
                _itemList.Add(editItem);
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
            InitItemsCount();
            AddListeners();
        }

        private void InitItemsCount()
        {
            // 清空当前显示
            for (int i = 0; i < _itemList.Count; i++)
            {
                _itemList[i].valueText.text = "";
            }
            string attrValueStr = _nodeAttr.GetValueString();
            // 获取掉落道具的列表
            List<string> paraList;
            if (attrValueStr == "")
            {
                paraList = new List<string>();
            }
            else
            {
                paraList = new List<string>(attrValueStr.Split(','));
            }
            for (int i = 0; i < paraList.Count; i += 2)
            {
                eDropItemType curType = (eDropItemType)int.Parse(paraList[i]);
                EditItem editItem = GetEditItemByType(curType);
                editItem.valueText.text = paraList[i + 1];
            }
        }

        private EditItem GetEditItemByType(eDropItemType type)
        {
            for (int i=0;i<_itemList.Count;i++)
            {
                if (_itemList[i].type == type)
                    return _itemList[i];
            }
            throw new Exception("Cannot find edit item of type " + type);
        }

        private void OnOKBtnHandler()
        {
            int totalCount = _itemList.Count;
            string valueStr = "";
            bool isFirstCount = true;
            for (int i = 0; i < totalCount; i++)
            {
                EditItem item = _itemList[i];
                if (item.valueText.text != "" && item.valueText.text != "0")
                {
                    if (!isFirstCount)
                    {
                        valueStr += ",";
                    }
                    isFirstCount = false;
                    valueStr += (int)item.type + "," + item.valueText.text;
                }
            }
            _nodeAttr.SetValue(valueStr);
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
