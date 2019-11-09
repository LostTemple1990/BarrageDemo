using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using YKEngine;

namespace BarrageEditor
{
    public class AttrEditLaserIdView : ViewBase
    {

        private GameObject _closeBtn;
        private GameObject _cancelBtn;
        private GameObject _okBtn;

        private BaseNodeAttr _nodeAttr;

        private DropdownExtend _styleDropdown;
        private GameObject _styleEditBtn;
        private Text _styleText;
        private int _styleId;

        private DropdownExtend _colorDropdown;
        private GameObject _colorEditBtn;
        private Text _colorText;
        private int _colorId;
        /// <summary>
        /// 当前弹型所拥有的颜色列表
        /// </summary>
        private List<int> _colorList;

        private DropdownExtend _blendModeDropdown;
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
        /// 为0表示当前填入的字符串无法解析为对应的子弹id
        /// </summary>
        private int _curBulletId;
        /// <summary>
        /// 当前界面的显示类型
        /// </summary>
        private BulletType _showType;
        /// <summary>
        /// 填入的显示的激光id
        /// 可能是数字，也可能是一个变量
        /// </summary>
        private string _showId;
        /// <summary>
        /// 当前激光类型的所有配置
        /// </summary>
        private List<LaserStyleCfg> _styleCfgs;
        /// <summary>
        /// 当前显示的激光的配置
        /// </summary>
        private LaserStyleCfg _curLaserCfg;

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
            _styleDropdown = tf.Find("Dropdown").GetComponent<DropdownExtend>();
            _styleEditBtn = tf.Find("EditBtn").gameObject;
            _styleText = tf.Find("Dropdown/Label").GetComponent<Text>();

            UIEventListener.Get(_styleEditBtn).AddClick(OnStyleEditBtnClickHandler);
        }

        private void OnStyleDropdownValueChangedHandler(int index)
        {
            //Logger.Log("Change style to value " + value);
            _curLaserCfg = _styleCfgs[index];
            _styleId = _curLaserCfg.styleId;
            _styleText.text = _curLaserCfg.name;
            UpdateColorDropdownOptions();
            // 如果选择的弹型没有之前选择的颜色，则默认设置为该弹型的第一种颜色
            if (!CheckColorAvailable(_styleId, _colorId))
            {
                _colorId = _curLaserCfg.availableColors[0];
                _colorDropdown.value = _colorList.IndexOf(_colorId);
            }
            else
            {
                _colorDropdown.value = _colorList.IndexOf(_colorId);
            }
            if (_blendIndex == -1)
            {
                _blendIndex = 0;
                _blendModeDropdown.value = _blendIndex;
            }
            UpdateBulletId();
            UpdateBulletIdTextAndPreview();
        }

        private void OnStyleEditBtnClickHandler()
        {
            List<object> datas = new List<object>();
            datas.Add(_showType);
            datas.Add(_styleId);
            Action<int> callback = new Action<int>(SelectStyleCallback);
            datas.Add(callback);
            UIManager.GetInstance().OpenView(ViewID.AttrSelectLaserStyleView, datas);
        }

        private void SelectStyleCallback(int styleId)
        {
            int index = -1;
            for (int i = 0; i < _styleCfgs.Count; i++)
            {
                if (_styleCfgs[i].styleId == styleId)
                {
                    index = i;
                    break;
                }
            }
            OnStyleDropdownValueChangedHandler(index);
        }

        private void InitBulletColorDropdown()
        {
            RectTransform tf = _viewTf.Find("Panel/Window/BulletColor").GetComponent<RectTransform>();
            _colorDropdown = tf.Find("Dropdown").GetComponent<DropdownExtend>();
            _colorEditBtn = tf.Find("EditBtn").gameObject;
            _colorText = tf.Find("Dropdown/Label").GetComponent<Text>();

            UIEventListener.Get(_colorEditBtn).AddClick(OnColorEditBtnClickHandler);
        }

        private void OnColorDropdownValueChangedHandler(int value)
        {
            _colorId = _colorList[value];
            UpdateBulletId();
            UpdateBulletIdTextAndPreview();
        }

        private void OnColorEditBtnClickHandler()
        {
            if (_styleId == -1)
                return;
            List<object> datas = new List<object>();
            datas.Add(_colorList);
            datas.Add(_colorId);
            Action<int> callback = new Action<int>(SelectColorCallback);
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
            _blendModeDropdown = tf.Find("Dropdown").GetComponent<DropdownExtend>();
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
            if (_styleId == -1 || _colorId == -1 || _blendIndex == -1)
                return;
            int baseNum = 200000;
            if (_showType == BulletType.Laser)
                baseNum = 200000;
            else if (_showType == BulletType.LinearLaser)
                baseNum = 300000;
            else if (_showType == BulletType.CurveLaser)
                baseNum = 400000;
            _curBulletId = baseNum + _styleId * 1000 + _colorId * 10 + _blendIndex;
        }

        private void UpdateBulletIdTextAndPreview()
        {
            if (_curBulletId == 0)
            {
                _bulletIdText.text = "BulletId : " + _showId;
                _bulletPreviewImg.sprite = null;
            }
            else
            {
                _bulletIdText.text = "BulletId : " + _curBulletId;
                string spName;
                if (_curBulletId % 10 == 0)
                    spName = "Laser" + _curBulletId;
                else
                    spName = "Laser" + (_curBulletId / 10 * 10);
                _bulletPreviewImg.sprite = ResourceManager.GetInstance().GetSprite(_curLaserCfg.packName, spName);
                _bulletPreviewImg.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode((eBlendMode)_blendIndex);
            }
        }

        private void AddListeners()
        {
            UIEventListener.Get(_closeBtn).AddClick(OnCloseBtnClickHandler);
            UIEventListener.Get(_cancelBtn).AddClick(OnCloseBtnClickHandler);
            UIEventListener.Get(_okBtn).AddClick(OnOKBtnHandler);
        }

        /// <summary>
        /// data
        /// <para>BulletType type 激光类型</para>
        /// <para>BaseNodeAttr nodeAttr</para>
        /// </summary>
        /// <param name="data"></param>
        public override void OnShow(object data)
        {
            List<object> datas = data as List<object>;
            _showType = (BulletType)datas[0];
            _nodeAttr = datas[1] as BaseNodeAttr;
            _showId = _nodeAttr.GetValueString();
            if (!int.TryParse(_showId,out _curBulletId))
            {
                _curBulletId = 0;
            }
            _styleCfgs = DatabaseManager.LaserDatabase.GetLaserStyleCfgsByType(_showType);
            InitAllShow();
        }

        private void InitAllShow()
        {
            InitStyleDropdownOptions();
            if (_curBulletId == 0)
            {
                _styleId = -1;
                _colorId = -1;
                _blendIndex = -1;
                UpdateColorDropdownOptions();
                StartCoroutine(UpdateDropdownText("undefined", "undefined", "undefined"));
            }
            else
            {
                _styleId = (_curBulletId % 100000) / 1000;
                _colorId = (_curBulletId % 1000) / 10;
                _blendIndex = _curBulletId % 10;
                _curLaserCfg = DatabaseManager.LaserDatabase.GetLaserStyleCfg(_showType, _styleId);
                // 更新子弹颜色的下拉框
                UpdateColorDropdownOptions();
                ColorCfg colorCfg = DatabaseManager.LaserDatabase.GetColorCfgByColorId(_colorId);
                string blendText = ((eBlendMode)_blendIndex).ToString();
                StartCoroutine(UpdateDropdownText(_curLaserCfg.name, colorCfg.colorName, blendText));
            }
            UpdateBulletIdTextAndPreview();
        }

        /// <summary>
        /// 协程，当前帧结束之后刷新下拉控件的文本
        /// </summary>
        /// <param name="styleText"></param>
        /// <param name="colorText"></param>
        /// <param name="blendText"></param>
        /// <returns></returns>
        private IEnumerator UpdateDropdownText(string styleText,string colorText,string blendText)
        {
            yield return new WaitForEndOfFrame();
            _styleText.text = styleText;
            _colorText.text = colorText;
            _blendText.text = blendText;
        }

        /// <summary>
        /// 获取激光类型在Dropdown中的下拉索引
        /// </summary>
        /// <param name="styleId"></param>
        /// <returns></returns>
        private int GetStyleDropdownIndex(int styleId)
        {
            int index = -1;
            for (int i=0;i<_styleCfgs.Count;i++)
            {
                if (_styleCfgs[i].styleId == styleId)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private void InitStyleDropdownOptions()
        {
            _styleDropdown.onValueChanged.RemoveAllListeners();
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            for (int i = 0; i < _styleCfgs.Count; i++)
            {
                optionList.Add(new Dropdown.OptionData(_styleCfgs[i].name));
            }
            _styleDropdown.options = optionList;
            _styleDropdown.onValueChanged.AddListener(OnStyleDropdownValueChangedHandler);
        }

        private void UpdateColorDropdownOptions()
        {
            if (_styleId == -1)
                return;
            _colorDropdown.onValueChanged.RemoveAllListeners();
            LaserStyleCfg cfg = DatabaseManager.LaserDatabase.GetLaserStyleCfg(_showType, _styleId);
            _colorList = cfg.availableColors;
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            for (int i = 0; i < _colorList.Count; i++)
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

        /// <summary>
        /// 检查对应style的激光是否包含colorId对应的颜色
        /// </summary>
        /// <param name="styleId"></param>
        /// <param name="colorId"></param>
        /// <returns></returns>
        private bool CheckColorAvailable(int styleId, int colorId)
        {
            if (_styleId == -1 || _colorId == -1)
                return false;
            LaserStyleCfg cfg = DatabaseManager.LaserDatabase.GetLaserStyleCfg(_showType, styleId);
            if (cfg.type == BulletType.Undefined || cfg.availableColors.IndexOf(colorId) == -1)
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
            _styleDropdown.ClearOptions();
            _colorDropdown.ClearOptions();
            _styleCfgs.Clear();
            _styleCfgs = null;
            base.OnClose();
        }
    }
}
