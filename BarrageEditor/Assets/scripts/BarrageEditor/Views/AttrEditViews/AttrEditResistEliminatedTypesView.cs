using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class AttrEditResistEliminatedTypesView : ViewBase
    {

        private GameObject _closeBtn;
        private GameObject _okBtn;

        private RectTransform _itemContainerTf;

        class EliminatedTypesItem
        {
            public GameObject go;
            public Toggle toggle;
        }

        private int _curSelectedItemIndex;
        private List<string> _paraNameList;
        private Dictionary<eEliminatedTypes, EliminatedTypesItem> _itemDic;
        private List<eEliminatedTypes> _defaultTypes;

        private BaseNodeAttr _nodeAttr;

        protected override void Init()
        {
            _closeBtn = _viewTf.Find("Panel/CancelBtn").gameObject;
            _okBtn = _viewTf.Find("Panel/OKBtn").gameObject;
            _itemContainerTf = _viewTf.Find("Panel/Window").GetComponent<RectTransform>();

            InitEliminatedTypesItems();
            AddListeners();
        }

        private void InitEliminatedTypesItems()
        {
            // 可编辑的碰撞枚举
            _defaultTypes = new List<eEliminatedTypes> {
                eEliminatedTypes.PlayerSpellCard, eEliminatedTypes.PlayerDead, eEliminatedTypes.HitPlayer, eEliminatedTypes.PlayerBullet,
                eEliminatedTypes.HitObjectCollider, eEliminatedTypes.GravitationField,
                eEliminatedTypes.CustomizedType0, eEliminatedTypes.CustomizedType1, eEliminatedTypes.CustomizedType2,
                eEliminatedTypes.CustomizedType3, eEliminatedTypes.CustomizedType4, eEliminatedTypes.CustomizedType5 };
            _itemDic = new Dictionary<eEliminatedTypes, EliminatedTypesItem>();
            for (int i = 0; i < _defaultTypes.Count; i++)
            {
                GameObject item = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "EditIgnoreCollisionItem");
                RectTransform tf = item.GetComponent<RectTransform>();
                tf.SetParent(_itemContainerTf, false);
                EliminatedTypesItem itemCls = new EliminatedTypesItem
                {
                    go = item,
                    toggle = tf.Find("Toggle").GetComponent<Toggle>(),
                };
                Text collisionGroupText = tf.Find("CollisionGroupText").GetComponent<Text>();
                collisionGroupText.text = _defaultTypes[i].ToString();
                _itemDic.Add(_defaultTypes[i], itemCls);
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
            UpdateEliminatedTypesItems();
        }

        private void UpdateEliminatedTypesItems()
        {
            int ignoreGroups = int.Parse(_nodeAttr.GetValueString());
            EliminatedTypesItem item;
            for (int i = 0; i < _defaultTypes.Count; i++)
            {
                int group = (int)_defaultTypes[i];
                item = _itemDic[(eEliminatedTypes)group];
                // 设置
                if ((group & ignoreGroups) != 0)
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
            int resistTypes = 0;
            EliminatedTypesItem item;
            for (int i = 0; i < _defaultTypes.Count; i++)
            {
                int type = (int)_defaultTypes[i];
                item = _itemDic[(eEliminatedTypes)type];
                if (item.toggle.isOn)
                {
                    resistTypes |= type;
                }
            }
            _nodeAttr.SetValue(resistTypes.ToString());
            Close();
        }

        private void OnCloseBtnClickHandler()
        {
            Close();
        }

        public override void OnClose()
        {

        }
    }
}
