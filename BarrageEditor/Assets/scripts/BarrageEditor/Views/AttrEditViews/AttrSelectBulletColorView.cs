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
            public int colorId;
            public GameObject itemGo;
            public GameObject btn;
            public GameObject selectImgGo;

            public void Clear()
            {
                itemGo = null;
                btn = null;
                selectImgGo = null;
            }
        }

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

        private void InitColorItems(List<int> availableColors)
        {
            _itemList = new List<ColorItem>();

            for (int i = 0; i < availableColors.Count; i++)
            {
                ColorCfg cfg = DatabaseManager.BulletDatabase.GetColorCfgByColorId(availableColors[i]);
                GameObject item = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/EditViews", "BulletColorItem");
                RectTransform itemTf = item.GetComponent<RectTransform>();
                itemTf.SetParent(_itemContainerTf, false);
                // 初始化StyleItem结构
                ColorItem colorItem = new ColorItem();
                colorItem.colorId = availableColors[i];
                colorItem.itemGo = item;
                colorItem.btn = itemTf.Find("BtnBg").gameObject;
                colorItem.selectImgGo = itemTf.Find("SelectImg").gameObject;
                // 设置选中
                colorItem.selectImgGo.SetActive(cfg.colorId == _colorId);
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

        /// <summary>
        /// <para>datas List</para>
        /// <para>availableColors List</para>
        /// <para>colorSelected int</para>
        /// <para>selectCallback Action(int) 参数为colorId</para>
        /// </summary>
        /// <param name="data"></param>
        public override void OnShow(object data)
        {
            List<object> datas = data as List<object>;
            List<int> availableColors = datas[0] as List<int>;
            _colorId = (int)datas[1];
            _selectCallback = datas[2] as Action<int>;
            InitColorItems(availableColors);
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
                GameObject.Destroy(colorItem.itemGo);
                _itemList[i].Clear();
            }
            _itemList.Clear();
            _selectCallback = null;
        }
    }
}
