using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class AttrEditParasView : ViewBase
    {

        private GameObject _closeBtn;
        private GameObject _okBtn;

        private RectTransform _itemContainerTf;

        struct EditParaItem
        {
            public GameObject go;
            public InputField valueText;
        }

        private int _curSelectedItemIndex;
        private List<string> _paraNameList;
        private List<EditParaItem> _itemList;

        private BaseNodeAttr _nodeAttr;

        protected override void Init()
        {
            _closeBtn = _viewTf.Find("Panel/CancelBtn").gameObject;
            _okBtn = _viewTf.Find("Panel/OKBtn").gameObject;
            _itemContainerTf = _viewTf.Find("Panel/Window").GetComponent<RectTransform>();

            _itemList = new List<EditParaItem>();

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
            InitParas();
        }

        private void InitParas()
        {
            CustomDefineType type = CustomDefineType.SimpleBullet;
            if (_nodeAttr.Node.GetNodeType() == NodeType.CreateCustomizedBullet)
            {
                type = CustomDefineType.SimpleBullet;
            }
            string typeName = _nodeAttr.Node.attrs[0].GetValueString();
            CustomDefineData data = CustomDefine.GetDataByTypeAndName(type, typeName);
            // 获取参数名称的列表
            if ( data == null )
            {
                _paraNameList = new List<string>();
            }
            else
            {
                _paraNameList = new List<string>(data.paraListStr.Split(','));
            }
            // 获取参数值的列表
            List<string> paraValueList = new List<string>(_nodeAttr.GetValueString().Split(','));
            for (int i = 0; i < _paraNameList.Count; i++)
            {
                GameObject item = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "EditParaItem");
                RectTransform tf = item.GetComponent<RectTransform>();
                tf.SetParent(_itemContainerTf, false);
                EditParaItem itemSt = new EditParaItem
                {
                    go = item,
                    valueText = tf.Find("ParaValueField").GetComponent<InputField>(),
                };
                Text paraNameText = tf.Find("ParaNameText").GetComponent<Text>();
                paraNameText.text = _paraNameList[i];
                if ( i < paraValueList.Count )
                {
                    itemSt.valueText.text = paraValueList[i];
                }
                else
                {
                    itemSt.valueText.text = "";
                }
                _itemList.Add(itemSt);
            }
        }

        private void OnOKBtnHandler()
        {
            int paraCount = _paraNameList.Count;
            string valueListStr = "";
            for (int i = 0; i < paraCount;)
            {
                EditParaItem item = _itemList[i];
                if ( item.valueText.text == "" )
                {
                    valueListStr += "nil";
                }
                else
                {
                    valueListStr += item.valueText.text;
                }
                i++;
                if (i < paraCount)
                {
                    valueListStr += ",";
                }
            }
            _nodeAttr.SetValue(valueListStr);
            Close();
        }

        private void OnCloseBtnClickHandler()
        {
            Close();
        }

        public override void OnClose()
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                GameObject.Destroy(_itemList[i].go);
            }
            _paraNameList.Clear();
            _itemList.Clear();
        }
    }
}
