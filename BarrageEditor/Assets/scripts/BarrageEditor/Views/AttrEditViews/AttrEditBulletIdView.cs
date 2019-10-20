using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class AttrEditBulletIdView : ViewBase
    {

        private GameObject _closeBtn;
        private GameObject _cancelBtn;
        private GameObject _okBtn;

        private BaseNodeAttr _nodeAttr;

        private Dropdown _styleDropdown;
        private GameObject _styleEditBtn;
        private Text _styleText;
        private int _styleIndex;

        private Dropdown _colorDropdown;
        private GameObject _colorEditBtn;
        private Text _colorText;
        private int _colorId;
        /// <summary>
        /// 当前弹型所拥有的颜色列表
        /// </summary>
        private List<int> _colorList;

        private Dropdown _blendModeDropdown;
        private Text _blendText;
        private int _blendIndex;

        /// <summary>
        /// 预览id
        /// </summary>
        private Text _bulletIdText;
        /// <summary>
        /// 预览图像
        /// </summary>
        private Image _bulletPreviewImg;
        /// <summary>
        /// 当前展示的子弹id
        /// 如果子弹id为编辑过的，则默认显示红色札弹
        /// </summary>
        private int _curBulletId;

        protected override void Init()
        {
            _closeBtn = _viewTf.Find("Panel/CloseBtn").gameObject;
            _cancelBtn = _viewTf.Find("Panel/CancelBtn").gameObject;
            _okBtn = _viewTf.Find("Panel/OKBtn").gameObject;

            _bulletIdText = _viewTf.Find("Panel/Window/BulletIdText").GetComponent<Text>();
            _bulletPreviewImg = _viewTf.Find("Panel/Window/PreviewBg/PreviewImg").GetComponent<Image>();

            InitBulletStyleDropdown();
            InitBulletColorDropdown();
            InitBulletBlendModeDropdown();

            AddListeners();
        }

        private void InitBulletStyleDropdown()
        {
            RectTransform tf = _viewTf.Find("Panel/Window/BulletStyle").GetComponent<RectTransform>();
            _styleDropdown = tf.Find("Dropdown").GetComponent<Dropdown>();
            _styleEditBtn = tf.Find("EditBtn").gameObject;
            _styleText = tf.Find("Dropdown/Label").GetComponent<Text>();

            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            List<BulletStyleCfg> bulletCfgs = DatabaseManager.BulletDatabase.GetBulletStyleCfgs();
            for (int i = 0; i < bulletCfgs.Count; i++)
            {
                optionList.Add(new Dropdown.OptionData(bulletCfgs[i].name));
            }
            _styleDropdown.options = optionList;
            _styleDropdown.onValueChanged.AddListener(OnStyleDropdownValueChangedHandler);

            UIEventListener.Get(_styleEditBtn).AddClick(OnStyleEditBtnClickHandler);
        }

        private void OnStyleDropdownValueChangedHandler(int value)
        {
            Logger.Log("Change style to value " + value);
            BulletStyleCfg cfg = DatabaseManager.BulletDatabase.GetBulletStyleCfgByStyleId(value);
            _styleIndex = value;
            _styleText.text = cfg.name;
            UpdateColorDropdownOptions();
            // 如果选择的弹型没有之前选择的颜色，则默认设置为该弹型的第一种颜色
            if ( !CheckColorAvailable(_styleIndex,_colorId) )
            {
                _colorId = cfg.availableColors[0];
                _colorDropdown.value = _colorList.IndexOf(_colorId);
            }
            UpdateBulletId();
            UpdateBulletIdTextAndPreview();
        }

        private void OnStyleEditBtnClickHandler()
        {
            List<object> datas = new List<object>();
            datas.Add(_styleIndex);
            Action<int> callback = new Action<int>(SelectStyleCallback);
            datas.Add(callback);
            UIManager.GetInstance().OpenView(ViewID.AttrSelectBulletStyleView, datas);
        }

        private void SelectStyleCallback(int styleIndex)
        {
            OnStyleDropdownValueChangedHandler(styleIndex);
        }

        private void InitBulletColorDropdown()
        {
            RectTransform tf = _viewTf.Find("Panel/Window/BulletColor").GetComponent<RectTransform>();
            _colorDropdown = tf.Find("Dropdown").GetComponent<Dropdown>();
            _colorEditBtn = tf.Find("EditBtn").gameObject;
            _colorText = tf.Find("Dropdown/Label").GetComponent<Text>();

            UIEventListener.Get(_colorEditBtn).AddClick(OnColorEditBtnClickHandler);
        }

        private void OnColorDropdownValueChangedHandler(int value)
        {
            _colorId = _colorList[value];
            ColorCfg colorCfg = DatabaseManager.BulletDatabase.GetColorCfgByColorId(_colorId);
            _colorText.text = colorCfg.colorName;
            UpdateBulletId();
            UpdateBulletIdTextAndPreview();
        }

        private void OnColorEditBtnClickHandler()
        {
            List<object> datas = new List<object>();
            datas.Add(_colorList);
            datas.Add(_colorId);
            Action<int> callback = new Action<int>(SelectStyleCallback);
            datas.Add(callback);
            UIManager.GetInstance().OpenView(ViewID.AttrSelectBulletColorView, datas);
        }

        private void SelectColorCallback(int colorId)
        {
            _colorId = colorId;
            ColorCfg colorCfg = DatabaseManager.BulletDatabase.GetColorCfgByColorId(_colorId);
            _colorText.text = colorCfg.colorName;
            UpdateBulletId();
            UpdateBulletIdTextAndPreview();
        }

        private void InitBulletBlendModeDropdown()
        {
            RectTransform tf = _viewTf.Find("Panel/Window/BulletBlendMode").GetComponent<RectTransform>();
            _blendModeDropdown = tf.Find("Dropdown").GetComponent<Dropdown>();
            _blendText = tf.Find("Dropdown/Label").GetComponent<Text>();

            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            optionList.Add(new Dropdown.OptionData(eBlendMode.Normal.ToString()));
            optionList.Add(new Dropdown.OptionData(eBlendMode.SoftAdditive.ToString()));
            _blendModeDropdown.options = optionList;
            _blendModeDropdown.onValueChanged.AddListener(OnBlendModeDropdownValueChangedHandler);  
        }

        private void OnBlendModeDropdownValueChangedHandler(int value)
        {
            _blendIndex = value;
            _blendText.text = ((eBlendMode)value).ToString();
            UpdateBulletIdTextAndPreview();
        }

        private void UpdateBulletId()
        {
            _curBulletId = 100000 + _styleIndex * 1000 + _colorId * 10;
        }

        private void UpdateBulletIdTextAndPreview()
        {
            _bulletIdText.text = "BulletId : " + _curBulletId;
            _bulletPreviewImg.sprite = ResourceManager.GetInstance().GetSprite("STGBulletsAtlas", "Bullet" + _curBulletId);
            _bulletPreviewImg.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode((eBlendMode)_blendIndex);
            _bulletPreviewImg.SetNativeSize();
        }

        private void AddListeners()
        {
            UIEventListener.Get(_closeBtn).AddClick(OnCloseBtnClickHandler);
            UIEventListener.Get(_cancelBtn).AddClick(OnCloseBtnClickHandler);
            UIEventListener.Get(_okBtn).AddClick(OnOKBtnHandler);
        }

        public override void OnShow(object data)
        {
            _nodeAttr = data as BaseNodeAttr;
            _curBulletId = int.Parse(_nodeAttr.GetValueString());
            if (_curBulletId == 0 || !CheckColorAvailable(_curBulletId))
            {
                _curBulletId = 107010;
            }
            InitAllShow();
        }

        private void InitAllShow()
        {
            _styleIndex = (_curBulletId % 100000) / 1000;
            _colorId = (_curBulletId % 1000) / 10;
            _blendIndex = _curBulletId % 10;
            _styleDropdown.value = _styleIndex;
            // 更新子弹颜色的下拉框
            UpdateColorDropdownOptions();
            _colorDropdown.value = _colorList.IndexOf(_colorId);
            // blendMode下拉框
            _blendModeDropdown.value = _blendIndex;

            UpdateBulletIdTextAndPreview();
        }

        private void UpdateColorDropdownOptions()
        {
            _colorDropdown.onValueChanged.RemoveAllListeners();
            BulletStyleCfg bulletCfg = DatabaseManager.BulletDatabase.GetBulletStyleCfgByStyleId(_styleIndex);
            _colorList = bulletCfg.availableColors;
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            for (int i=0;i<_colorList.Count;i++)
            {
                ColorCfg colorCfg = DatabaseManager.BulletDatabase.GetColorCfgByColorId(_colorList[i]);
                Dropdown.OptionData optionData = new Dropdown.OptionData(colorCfg.colorName);
                options.Add(optionData);
            }
            _colorDropdown.options = options;
            _colorDropdown.onValueChanged.AddListener(OnColorDropdownValueChangedHandler);
        }

        private bool CheckColorAvailable(int bulletId)
        {
            int styleId = (bulletId % 100000) / 1000;
            int colorId = (bulletId % 1000) / 10;
            return CheckColorAvailable(styleId, colorId);
        }

        private bool CheckColorAvailable(int styleId,int colorId)
        {
            BulletStyleCfg bulletCfg = DatabaseManager.BulletDatabase.GetBulletStyleCfgByStyleId(styleId);
            if ( bulletCfg.styleId != styleId || bulletCfg.availableColors.IndexOf(colorId) == -1 )
            {
                return false;
            }
            return true;
        }

        private void OnOKBtnHandler()
        {
            _nodeAttr.SetValue(_curBulletId);
            Close();
        }

        private void OnCloseBtnClickHandler()
        {
            Close();
        }

        public override void OnClose()
        {

            base.OnClose();
        }
    }
}
