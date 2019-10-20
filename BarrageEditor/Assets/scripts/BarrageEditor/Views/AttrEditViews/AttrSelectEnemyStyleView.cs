using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class AttrSelectEnemyStyleView : ViewBase
    {

        private GameObject _closeBtn;
        private RectTransform _itemContainerTf;

        struct StyleItem
        {
            public int enemyId;
            public GameObject itemGo;
            public GameObject btn;
            public GameObject selectImgGo;
        }

        private int _curSelectedItemIndex;
        private List<StyleItem> _itemList;
        /// <summary>
        /// 子弹配置
        /// </summary>
        private List<EnemyStyleCfg> _enemyCfgs;
        /// <summary>
        /// 编辑enemyId的节点属性
        /// </summary>
        private BaseNodeAttr _nodeAttr;
        /// <summary>
        /// 填入的敌机id
        /// </summary>
        private int _enemyId;

        protected override void Init()
        {
            _closeBtn = _viewTf.Find("Panel/CloseBtn").gameObject;
            _itemContainerTf = _viewTf.Find("Panel/Window").GetComponent<RectTransform>();
            InitEnemyItems();

            AddListeners();
        }

        private void InitEnemyItems()
        {
            _itemList = new List<StyleItem>();
            _curSelectedItemIndex = -1;
            _enemyCfgs = DatabaseManager.EnemyDatabase.GetEnemyStyleCfgs();
            for (int i = 0; i < _enemyCfgs.Count; i++)
            {
                EnemyStyleCfg cfg = _enemyCfgs[i];
                GameObject item = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "EnemyStyleItem");
                RectTransform itemTf = item.GetComponent<RectTransform>();
                itemTf.SetParent(_itemContainerTf, false);
                // 初始化StyleItem结构
                StyleItem styleItem = new StyleItem();
                styleItem.enemyId = cfg.styleId;
                styleItem.itemGo = item;
                styleItem.btn = itemTf.Find("BtnBg").gameObject;
                styleItem.selectImgGo = itemTf.Find("SelectImg").gameObject;
                styleItem.selectImgGo.SetActive(false);
                // 设置敌机图像
                Image bulletImg = itemTf.Find("EnemyImg").GetComponent<Image>();
                bulletImg.sprite = ResourceManager.GetInstance().GetSprite(cfg.packName, cfg.resName);
                int itemIndex = i;
                // 添加事件监听
                UIEventListener.Get(styleItem.btn).AddClick(() =>
                {
                    OnStyleItemClickHandler(itemIndex);
                });
                _itemList.Add(styleItem);
            }
        }

        private void AddListeners()
        {
            UIEventListener.Get(_closeBtn).AddClick(OnCloseBtnClickHandler);
        }

        public override void OnShow(object data)
        {
            _nodeAttr = data as BaseNodeAttr;
            if (!int.TryParse(_nodeAttr.GetValueString(), out _enemyId))
                _enemyId = 0;
            UpdateSelectStyle();
        }

        private void UpdateSelectStyle()
        {
            if (_enemyId == 0)
            {
                _curSelectedItemIndex = 0;
            }
            else
            {
                for (int i = 0; i < _itemList.Count; i++)
                {
                    if (_itemList[i].enemyId == _enemyId)
                    {
                        _curSelectedItemIndex = i;
                        break;
                    }
                }
            }
            _itemList[_curSelectedItemIndex].selectImgGo.SetActive(true);
        }

        private void OnStyleItemClickHandler(int itemIndex)
        {
            int styleId = _itemList[itemIndex].enemyId;
            _nodeAttr.SetValue(styleId);
            Close();
        }

        private void OnCloseBtnClickHandler()
        {
            Close();
        }

        public override void OnClose()
        {
            _itemList[_curSelectedItemIndex].selectImgGo.SetActive(false);
            _nodeAttr = null;
        }
    }
}
