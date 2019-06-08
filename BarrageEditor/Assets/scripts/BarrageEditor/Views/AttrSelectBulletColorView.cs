using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class AttrSelectBulletColorView : ViewBase
    {

        private GameObject _closeBtn;
        private RectTransform _itemContainerTf;

        struct ColorItem
        {
            public GameObject item;
            public GameObject btn;
            public GameObject selectImgGo;

            public void Clear()
            {
                item = null;
                btn = null;
                selectImgGo = null;
            }
        }

        private int _bulletId;
        private int _styleId;
        private int _colorId;

        private List<ColorItem> _itemList;
        /// <summary>
        /// 子弹配置
        /// </summary>
        private List<ColorCfg> _colorCfgs;

        private Action<int> _selectCallback;

        protected override void Init()
        {
            _closeBtn = _viewTf.Find("Panel/CloseBtn").gameObject;
            _itemContainerTf = _viewTf.Find("Panel/Window").GetComponent<RectTransform>();

            AddListeners();
        }

        private void InitColorItems()
        {
            _itemList = new List<ColorItem>();
            BulletStyleCfg styleCfg = DatabaseManager.BulletDatabase.GetBulletStyleCfgByStyleId(_styleId);
            
            List<int> availableColors = styleCfg.availableColors;
            for (int i = 0; i < availableColors.Count; i++)
            {
                ColorCfg cfg = DatabaseManager.BulletDatabase.GetColorCfgByColorId(availableColors[i]);
                GameObject item = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "BulletColorItem");
                RectTransform itemTf = item.GetComponent<RectTransform>();
                itemTf.SetParent(_itemContainerTf, false);
                // 初始化StyleItem结构
                ColorItem colorItem = new ColorItem();
                colorItem.item = item;
                colorItem.btn = itemTf.Find("Btn").gameObject;
                colorItem.selectImgGo = itemTf.Find("SelectImg").gameObject;
                colorItem.selectImgGo.SetActive(false);
                // 设置子弹图像
                Image colorImg = itemTf.Find("ColorImg").GetComponent<Image>();
                colorImg.sprite = ResourceManager.GetInstance().GetSprite(cfg.packName, cfg.resName);
                // 添加事件监听
                UIEventListener.Get(colorItem.btn).AddClick(() =>
                {
                    OnColorItemClickHandler(cfg.colorId);
                });
                _itemList.Add(colorItem);
            }
        }

        private void AddListeners()
        {
            UIEventListener.Get(_closeBtn).AddClick(OnCloseBtnClickHandler);
        }

        public override void OnShow(object data)
        {
            List<object> datas = data as List<object>;
            _bulletId = (int)datas[0];
            _styleId = (_bulletId % 100000) / 1000;
            _colorId = (_bulletId % 1000) / 10;
            _selectCallback = datas[1] as Action<int>;
            InitColorItems();
            UpdateSelectColor();
        }

        private void UpdateSelectColor()
        {
            _itemList[_colorId].selectImgGo.SetActive(true);
        }

        private void OnColorItemClickHandler(int colorId)
        {
            if (_selectCallback != null)
            {
                _selectCallback(colorId);
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
                ColorItem colorItem = _itemList[i];
                UIEventListener.Get(colorItem.btn).RemoveAllEvents();
                GameObject.Destroy(colorItem.item);
                _itemList[i].Clear();
            }
            _itemList.Clear();
            _selectCallback = null;
        }
    }
}
