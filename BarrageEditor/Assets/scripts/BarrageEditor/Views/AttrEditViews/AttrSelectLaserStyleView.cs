using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class AttrSelectLaserStyleView : ViewBase
    {

        private GameObject _closeBtn;
        private RectTransform _itemContainerTf;

        struct StyleItem
        {
            public int styleId;
            public GameObject itemGo;
            public GameObject btn;
            public GameObject selectImgGo;
        }
        private int _curSelectedItemIndex;
        private List<StyleItem> _itemList;
        /// <summary>
        /// 子弹配置
        /// </summary>
        private List<LaserStyleCfg> _bulletCfgs;

        private Action<int> _selectCallback;
        /// <summary>
        /// 子弹类型
        /// </summary>
        private BulletType _bulletType;

        protected override void Init()
        {
            _closeBtn = _viewTf.Find("Panel/CloseBtn").gameObject;
            _itemContainerTf = _viewTf.Find("Panel/Window").GetComponent<RectTransform>();

            AddListeners();
        }

        private void InitBulletItems()
        {
            _itemList = new List<StyleItem>();
            _bulletCfgs = DatabaseManager.LaserDatabase.GetLaserStyleCfgsByType(_bulletType);
            for (int i = 0; i < _bulletCfgs.Count; i++)
            {
                LaserStyleCfg cfg = _bulletCfgs[i];
                GameObject item = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "LaserStyleItem");
                RectTransform itemTf = item.GetComponent<RectTransform>();
                itemTf.SetParent(_itemContainerTf, false);
                // 初始化StyleItem结构
                StyleItem styleItem = new StyleItem();
                styleItem.styleId = cfg.styleId;
                styleItem.itemGo = item;
                styleItem.btn = itemTf.Find("BtnBg").gameObject;
                styleItem.selectImgGo = itemTf.Find("SelectImg").gameObject;
                styleItem.selectImgGo.SetActive(false);
                // 设置子弹图像
                Image bulletImg = itemTf.Find("BulletImg").GetComponent<Image>();
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
            List<object> datas = data as List<object>;
            _bulletType = (BulletType)datas[0];
            InitBulletItems();
            int styleId = (int)datas[1];
            _curSelectedItemIndex = GetItemIndexByStyleId(styleId);
            _selectCallback = datas[2] as Action<int>;
            UpdateSelectStyle();
        }

        private int GetItemIndexByStyleId(int styleId)
        {
            for (int i=0;i<_bulletCfgs.Count;i++)
            {
                if (_bulletCfgs[i].styleId == styleId)
                {
                    return i;
                }
            }
            return 0;
        }

        private void UpdateSelectStyle()
        {
            _itemList[_curSelectedItemIndex].selectImgGo.SetActive(true);
        }

        private void OnStyleItemClickHandler(int itemIndex)
        {
            int styleId = _bulletCfgs[itemIndex].styleId;
            if (_selectCallback != null)
            {
                _selectCallback(styleId);
            }
            Close();
        }

        private void OnCloseBtnClickHandler()
        {
            Close();
        }

        public override void OnClose()
        {
            _selectCallback = null;
            StyleItem styleItem;
            for (int i=0;i<_itemList.Count;i++)
            {
                styleItem = _itemList[i];
                GameObject.Destroy(styleItem.itemGo);
            }
            _itemList.Clear();
        }
    }
}
