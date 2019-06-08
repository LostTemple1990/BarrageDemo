using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class AttrSelectBulletStyleView : ViewBase
    {

        private GameObject _closeBtn;
        private RectTransform _itemContainerTf;

        struct StyleItem
        {
            public GameObject btn;
            public GameObject selectImgGo;
        }
        private int _curSelectedItemIndex;
        private List<StyleItem> _itemList;
        /// <summary>
        /// 子弹配置
        /// </summary>
        private List<BulletStyleCfg> _bulletCfgs;

        private Action<int> _selectCallback;

        protected override void Init()
        {
            _closeBtn = _viewTf.Find("Panel/CloseBtn").gameObject;
            _itemContainerTf = _viewTf.Find("Panel/Window").GetComponent<RectTransform>();
            InitBulletItems();

            AddListeners();
        }

        private void InitBulletItems()
        {
            _itemList = new List<StyleItem>();
            _curSelectedItemIndex = -1;
            _bulletCfgs = DatabaseManager.BulletDatabase.GetBulletStyleCfgs();
            for (int i=0;i< _bulletCfgs.Count;i++)
            {
                BulletStyleCfg cfg = _bulletCfgs[i];
                GameObject item = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "BulletStyleItem");
                RectTransform itemTf = item.GetComponent<RectTransform>();
                itemTf.SetParent(_itemContainerTf, false);
                // 初始化StyleItem结构
                StyleItem styleItem = new StyleItem();
                styleItem.btn = itemTf.Find("Btn").gameObject;
                styleItem.selectImgGo = itemTf.Find("SelectImg").gameObject;
                styleItem.selectImgGo.SetActive(false);
                // 设置子弹图像
                Image bulletImg = itemTf.Find("BulletImg").GetComponent<Image>();
                bulletImg.sprite = ResourceManager.GetInstance().GetSprite(cfg.packName, cfg.resName);
                // 添加事件监听
                UIEventListener.Get(styleItem.btn).AddClick(()=>
                {
                    OnStyleItemClickHandler(i);
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
            _curSelectedItemIndex = (int)datas[0];
            _selectCallback = datas[1] as Action<int>;
            UpdateSelectStyle();
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
            if ( _curSelectedItemIndex != -1 )
            {
                _itemList[_curSelectedItemIndex].selectImgGo.SetActive(false);
                _curSelectedItemIndex = -1;
            }
        }
    }
}
