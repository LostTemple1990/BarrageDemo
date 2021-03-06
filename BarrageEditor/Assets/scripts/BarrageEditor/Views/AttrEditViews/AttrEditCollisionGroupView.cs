﻿using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class AttrEditCollisionGroupView : ViewBase
    {

        private GameObject _closeBtn;
        private GameObject _okBtn;

        private RectTransform _itemContainerTf;

        class CollisionGroupItem
        {
            public GameObject go;
            public Toggle toggle;
        }

        private int _curSelectedItemIndex;
        private List<string> _paraNameList;
        private Dictionary<eColliderGroup, CollisionGroupItem> _itemDic;
        private List<eColliderGroup> _defaultGroups;

        private BaseNodeAttr _nodeAttr;

        protected override void Init()
        {
            _closeBtn = _viewTf.Find("Panel/CancelBtn").gameObject;
            _okBtn = _viewTf.Find("Panel/OKBtn").gameObject;
            _itemContainerTf = _viewTf.Find("Panel/Window").GetComponent<RectTransform>();

            InitGroupItems();
            AddListeners();
        }

        private void InitGroupItems()
        {
            // 可编辑的碰撞枚举
            _defaultGroups = new List<eColliderGroup> {
                eColliderGroup.Player, eColliderGroup.PlayerBullet, eColliderGroup.Enemy, eColliderGroup.EnemyBullet,eColliderGroup.Item,
                eColliderGroup.CustomizedType0, eColliderGroup.CustomizedType1, eColliderGroup.CustomizedType2,
                eColliderGroup.CustomizedType3, eColliderGroup.CustomizedType4, eColliderGroup.CustomizedType5 };
            _itemDic = new Dictionary<eColliderGroup, CollisionGroupItem>();
            for (int i = 0; i < _defaultGroups.Count; i++)
            {
                GameObject item = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "EditCollisionItem");
                RectTransform tf = item.GetComponent<RectTransform>();
                tf.SetParent(_itemContainerTf, false);
                CollisionGroupItem itemCls = new CollisionGroupItem
                {
                    go = item,
                    toggle = tf.Find("Toggle").GetComponent<Toggle>(),
                };
                Text collisionGroupText = tf.Find("CollisionGroupText").GetComponent<Text>();
                collisionGroupText.text = _defaultGroups[i].ToString();
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
        }

        private void UpdateGroupItems()
        {
            int selectedGroups = int.Parse(_nodeAttr.GetValueString());
            CollisionGroupItem item;
            for (int i=0;i<_defaultGroups.Count;i++)
            {
                int group = (int)_defaultGroups[i];
                item = _itemDic[(eColliderGroup)group];
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
            CollisionGroupItem item;
            for (int i = 0; i < _defaultGroups.Count; i++)
            {
                int group = (int)_defaultGroups[i];
                item = _itemDic[(eColliderGroup)group];
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

        }
    }
}
