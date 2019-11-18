using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Create(name,packName,resName,posX,posY,appearPosX,appearPosY,highlightPosX,hightlightPosY)
/// Highlight(name,bool)
/// Sentence(style,text,posX,posY,scale,duration)
/// Disappear(name,dir)
/// </summary>
public class STGDialogView : ViewBase, ICommand
{
    enum eItemType : byte
    {
        Undefined = 0,
        Dialog = 1,
        Character = 2
    }

    abstract class BaseDialogItem
    {
        protected GameObject _itemGo;
        protected RectTransform _itemTf;
        protected string _name;
        protected bool _isEnable;

        public BaseDialogItem(Transform viewTf)
        {
            _isEnable = true;
        }

        public virtual void Update(int dTime)
        {

        }
        public virtual void Clear()
        {
            GameObject.Destroy(_itemGo);
            _itemGo = null;
            _itemTf = null;
        }

        public virtual eItemType GetItemType()
        {
            return eItemType.Undefined;
        }

        public string name
        {
            get { return _name; }
        }

        public bool isEnable
        {
            get { return _isEnable; }
        }
    }

    #region BaseDialogItem
    class DialogItem : BaseDialogItem
    {
        private readonly static List<Vector2> DialogSize = new List<Vector2> { new Vector2(160, 128) };
        private readonly static List<Vector2> TextOffsetsL = new List<Vector2> { new Vector2(24, -40) };
        private readonly static List<Vector2> TextOffsetsR = new List<Vector2> { new Vector2(24, -40) };

        private int _style;
        private float _itemScale;
        private float _dialogBoxScale;
        /// <summary>
        /// 对话框tf
        /// </summary>
        private RectTransform _dialogTf;
        /// <summary>
        /// 对话框图像
        /// </summary>
        private Image _dialogImg;
        /// <summary>
        /// 对话框文本
        /// </summary>
        private RectTransform _textTf;
        /// <summary>
        /// 对话框文本
        /// </summary>
        private Text _dialogText;
        private Vector2 _curPos;
        private float _curScale;
        private float _beginScale;
        private float _endScale;

        private bool _isScaling;
        private int _scaleTime;
        private int _scaleDuration;

        private int _existTime;
        private int _existDuration;

        public DialogItem(Transform viewTf)
            :base(viewTf)
        {
            _itemGo = ResourceManager.GetInstance().GetCommonPrefab("Prefab/Views", "STGDialogBoxItem");
            _itemGo.transform.SetParent(viewTf, false);
            _itemTf = _itemGo.GetComponent<RectTransform>();
            _dialogTf = _itemTf.Find("DialogImg").GetComponent<RectTransform>();
            _dialogImg = _dialogTf.GetComponent<Image>();
            _textTf = _itemTf.Find("Text").GetComponent<RectTransform>();
            _dialogText = _textTf.GetComponent<Text>();
            // 注册事件
            _dialogText.RegisterDirtyLayoutCallback(TextDirtyLayoutCallback);
        }

        public void Init(int style,string text,float posX,float posY,int duration,float scale)
        {
            _style = style;
            _itemScale = scale > 0 ? 1 : -1;
            _dialogBoxScale = Mathf.Abs(scale);
            _curPos = new Vector2(posX, posY);
            _itemTf.localScale = new Vector3(0, 1, 0);
            _itemTf.anchoredPosition = _curPos;
            _existTime = 0;
            _existDuration = duration;
            // text
            _textTf.localScale = new Vector3(_itemScale, 1, 1);
            SetText(text);
        }

        private void SetText(string text)
        {
            _dialogText.text = text;

            _isScaling = true;
            _beginScale = 0;
            _endScale = _itemScale;
            _scaleTime = 0;
            _scaleDuration = 15;
        }

        private void TextDirtyLayoutCallback()
        {
            Vector2 textOffset = _itemScale >= 0 ? TextOffsetsL[_style] : TextOffsetsR[_style];
            float preferredWidth = _dialogText.preferredWidth + TextOffsetsL[_style].x + TextOffsetsR[_style].x;
            // 计算对话框的宽度
            float minWidth = DialogSize[_style].x;
            if (preferredWidth < minWidth)
                preferredWidth = minWidth;
            _dialogTf.sizeDelta = new Vector2(preferredWidth, DialogSize[_style].y * _dialogBoxScale);
            _dialogTf.anchoredPosition = Vector2.zero;
            // 文本的位置
            _textTf.sizeDelta = new Vector2(_dialogText.preferredWidth, _dialogText.preferredHeight);
            Vector2 textPos = Vector2.zero;
            if (_itemScale >= 0)
            {
                textPos = new Vector3(textOffset.x, textOffset.y * _dialogBoxScale);
            }
            else
            {
                textPos = new Vector3(textOffset.x + _dialogText.preferredWidth, textOffset.y * _dialogBoxScale);
            }
            _textTf.anchoredPosition = textPos;
        }

        public override eItemType GetItemType()
        {
            return eItemType.Dialog;
        }

        public void Delete()
        {
            _isEnable = false;
            _isScaling = true;
            _beginScale = _curScale;
            _endScale = 0;
            _scaleTime = 0;
            _scaleDuration = 15;
        }

        public override void Update(int dTime)
        {
            if (!_isEnable)
                return;
            if (_isScaling)
            {
                _scaleTime += dTime;
                float factor = _scaleTime >= _scaleDuration ? 1 : (float)_scaleTime / _scaleDuration;
                _curScale = Mathf.Lerp(_beginScale, _endScale, factor);
                _itemTf.localScale = new Vector3(_curScale, 1, 1);
                if (_scaleTime >= _scaleDuration)
                {
                    _isScaling = false;
                    if ( _endScale == 0)
                    {
                        _isEnable = false;
                    }
                }
            }
            if (_existDuration > 0)
            {
                _existTime += dTime;
                if (_existTime >= _existDuration)
                {
                    _isEnable = false;
                }
            }
        }

        public override void Clear()
        {
            base.Clear();
        }
    }
    #endregion

    #region CharacterItem
    class CharacterItem : BaseDialogItem
    {
        private static Vector2 AppearOffset = new Vector2(-100, 0);
        private static Vector2 HighlightOffset = new Vector2(25, 0);
        private static float SegmentX = 300f;
        /// <summary>
        /// 人物立绘的tf
        /// </summary>
        private RectTransform _characterTf;
        /// <summary>
        /// 人物立绘
        /// </summary>
        private Image _characterImg;

        /// <summary>
        /// 立绘出现的位置
        /// </summary>
        private Vector2 _appearPos;
        /// <summary>
        /// 立绘默认位置
        /// </summary>
        private Vector2 _defaultPos;
        /// <summary>
        /// 立绘高亮时的位置
        /// </summary>
        private Vector2 _highlightPos;
        /// <summary>
        /// 立绘当前的位置
        /// </summary>
        private Vector2 _curPos;

        private bool _isAppearing;
        private bool _isActivating;
        private bool _isDeactivating;
        private bool _isFadingOut;

        private int _time;
        private int _duration;
        private float _curAlpha;
        private Color _curColor;
        private Color _beginColor;
        private Color _endColor;
        private Vector2 _startPos;
        private Vector2 _endPos;

        public CharacterItem(Transform viewTf)
            :base(viewTf)
        {
            _itemGo = ResourceManager.GetInstance().GetCommonPrefab("Prefab/Views", "STGDialogCGItem");
            _itemGo.transform.SetParent(viewTf, false);
            _itemTf = _itemGo.GetComponent<RectTransform>();
            _characterTf = _itemTf.Find("Face").GetComponent<RectTransform>();
            _characterImg = _characterTf.GetComponent<Image>();
        }

        public void Init(string name,string spName,float posX,float posY)
        {
            _name = name;
            _characterImg.sprite = Resources.Load<Sprite>("Faces/" + spName);
            _characterImg.SetNativeSize();
            _curColor = new Color(0.25f, 0.25f, 0.25f, 0);
            _characterImg.color = _curColor;
            _defaultPos = new Vector2(posX, posY);
            _characterTf.anchoredPosition = _defaultPos;
            if (_defaultPos.x < SegmentX)
            {
                _appearPos = new Vector2(_defaultPos.x + AppearOffset.x, _defaultPos.y);
                _highlightPos = new Vector2(_defaultPos.x + HighlightOffset.x, _defaultPos.y);
            }
            else
            {
                _appearPos = new Vector2(_defaultPos.x - AppearOffset.x, _defaultPos.y);
                _highlightPos = new Vector2(_defaultPos.x - HighlightOffset.x, _defaultPos.y);
            }

            _isAppearing = true;
            _time = 0;
            _duration = 30;
            _beginColor = _curColor;
            _endColor = new Color(0.25f, 0.25f, 0.25f, 1);
            // 位置缓动
            _curPos = _appearPos;
            _startPos = _appearPos;
            _endPos = _defaultPos;
        }

        public void Highlight(bool highlight)
        {
            if (highlight)
            {
                _isAppearing = false;
                _isActivating = true;
                _isDeactivating = false;
                _isFadingOut = false;
                _time = 0;
                _duration = 30;
                _startPos = _curPos;
                _endPos = _highlightPos;
                _beginColor = _curColor;
                _endColor = new Color(1, 1, 1, 1);
            }
            else
            {
                _isAppearing = false;
                _isActivating = false;
                _isDeactivating = true;
                _isFadingOut = false;
                _time = 0;
                _duration = 30;
                _startPos = _curPos;
                _endPos = _defaultPos;
                _beginColor = _curColor;
                _endColor = new Color(0.25f, 0.25f, 0.25f, 1);
            }
        }

        public void FadeOut()
        {
            _isAppearing = false;
            _isActivating = false;
            _isDeactivating = false;
            _isFadingOut = true;
            _time = 0;
            _duration = 60;
            _startPos = _curPos;
            _endPos = _appearPos;
            _beginColor = _curColor;
            _endColor = _curColor;
            _endColor.a = 0;
        }

        public override void Update(int dTime)
        {
            if (!_isEnable)
                return;
            if (_isAppearing)
            {
                Appearing();
            }
            if (_isActivating)
            {
                Activating();
            }
            if (_isDeactivating)
            {
                Deactivating();
            }
            if (_isFadingOut)
            {
                FadingOut();
            }
        }

        private void Appearing()
        {
            _time++;
            float factor = (float)_time / _duration;
            // 颜色
            _curColor = Color.Lerp(_beginColor, _endColor, factor);
            _characterImg.color = _curColor;
            // 位置
            _curPos = Vector2.Lerp(_startPos, _endPos, factor);
            _characterTf.anchoredPosition = _curPos;
            if (_time >= _duration)
            {
                _isAppearing = false;
            }
        }

        private void Activating()
        {
            _time++;
            float factor = (float)_time / _duration;
            // 颜色
            _curColor = Color.Lerp(_beginColor, _endColor, factor);
            _characterImg.color = _curColor;
            // 位置
            _curPos = Vector2.Lerp(_startPos, _endPos, factor);
            _characterTf.anchoredPosition = _curPos;
            if (_time >= _duration)
            {
                _isActivating = false;
            }
        }

        private void Deactivating()
        {
            _time++;
            float factor = (float)_time / _duration;
            // 颜色
            _curColor = Color.Lerp(_beginColor, _endColor, factor);
            _characterImg.color = _curColor;
            // 位置
            _curPos = Vector2.Lerp(_startPos, _endPos, factor);
            _characterTf.anchoredPosition = _curPos;
            if (_time >= _duration)
            {
                _isDeactivating = false;
            }
        }

        private void FadingOut()
        {
            _time++;
            float factor = (float)_time / _duration;
            // 颜色
            _curColor = Color.Lerp(_beginColor, _endColor, factor);
            _characterImg.color = _curColor;
            // 位置
            _curPos = Vector2.Lerp(_startPos, _endPos, factor);
            _characterTf.anchoredPosition = _curPos;
            if (_time >= _duration)
            {
                _isFadingOut = false;
                _isEnable = false;
            }
        }

        public override eItemType GetItemType()
        {
            return eItemType.Character;
        }

        public override void Clear()
        {
            _characterImg = null;
            _characterTf = null;
            base.Clear();
        }
    }
    #endregion

    private List<BaseDialogItem> _items;
    private int _itemCount;
    /// <summary>
    /// 开始剧情模式的时间
    /// </summary>
    private int _timeSinceStarted;
    /// <summary>
    /// 是否因为暂停而被隐藏
    /// </summary>
    private bool _isHiddenByPause;
    private GameObject _containerGo;
    private Transform _containerTf;

    public STGDialogView() : base()
    {
        _items = new List<BaseDialogItem>();
    }

    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
        _containerTf = _viewTf.Find("Container");
        _containerGo = _containerTf.gameObject; ;
    }

    public override void OnShow(object data)
    {
        base.OnShow(data);
        // 注册事件监听
        CommandManager.GetInstance().Register(CommandConsts.StartDialog, this);
        CommandManager.GetInstance().Register(CommandConsts.CreateDialogCG, this);
        CommandManager.GetInstance().Register(CommandConsts.HighlightDialogCG, this);
        CommandManager.GetInstance().Register(CommandConsts.FadeOutDialogCG, this);
        CommandManager.GetInstance().Register(CommandConsts.CreateDialogBox, this);
        CommandManager.GetInstance().Register(CommandConsts.DelDialogBox, this);
        CommandManager.GetInstance().Register(CommandConsts.UpdateDialog, this);
        CommandManager.GetInstance().Register(CommandConsts.PauseGame, this);
        CommandManager.GetInstance().Register(CommandConsts.ContinueGame, this);
        CommandManager.GetInstance().Register(CommandConsts.ContinueGameAfterGameOver, this);
        CommandManager.GetInstance().Register(CommandConsts.RetryGame, this);
        CommandManager.GetInstance().Register(CommandConsts.RetryStage, this);
        CommandManager.GetInstance().Register(CommandConsts.BackToTitle, this);

        _itemCount = 0;
        _isHiddenByPause = false;
        _containerGo.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="args"></param>
    public void Execute(int cmd, object data)
    {
        switch (cmd)
        {
            case CommandConsts.StartDialog:
                _timeSinceStarted = 0;
                break;
            case CommandConsts.CreateDialogCG:
                {
                    List<object> datas = data as List<object>;
                    CharacterItem item = new CharacterItem(_containerTf);
                    item.Init(datas[0] as string, datas[1] as string, (float)datas[2], (float)datas[3]);
                    AddItem(item);
                    break;
                }
            case CommandConsts.HighlightDialogCG:
                {
                    List<object> datas = data as List<object>;
                    CharacterItem item = GetCGItemByName(datas[0] as string);
                    if (item != null)
                    {
                        item.Highlight((bool)datas[1]);
                    }
                    break;
                }
            case CommandConsts.FadeOutDialogCG:
                {
                    CharacterItem item = GetCGItemByName(data as string);
                    if (item != null)
                    {
                        item.FadeOut();
                    }
                    break;
                }
            case CommandConsts.CreateDialogBox:
                {
                    List<object> datas = data as List<object>;
                    DialogItem item = new DialogItem(_containerTf);
                    item.Init((int)datas[0], datas[1] as string, (float)datas[2], (float)datas[3], (int)datas[4], (float)datas[5]);
                    AddItem(item);
                    break;
                }
            case CommandConsts.DelDialogBox:
                DelDialogItem();
                break;
            case CommandConsts.UpdateDialog:
                {
                    int dTime = data == null ? 1 : (int)data;
                    UpdateDialog(dTime);
                    break;
                }
            case CommandConsts.PauseGame:
                OnSTGPause();
                break;
            case CommandConsts.ContinueGame:
                OnSTGContinue();
                break;
            case CommandConsts.ContinueGameAfterGameOver:
                OnSTGContinue();
                break;
            case CommandConsts.RetryGame:
                OnRetry();
                break;
            case CommandConsts.RetryStage:
                OnRetry();
                break;
            case CommandConsts.BackToTitle:
                OnRetry();
                break;
        }
    }

    private void AddItem(BaseDialogItem item)
    {
        _items.Add(item);
        _itemCount++;
    }

    /// <summary>
    /// 根据名称获取对应的人物立绘item
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private CharacterItem GetCGItemByName(string name)
    {
        CharacterItem item = null;
        BaseDialogItem tmp;
        for (int i=0;i<_itemCount;i++)
        {
            tmp = _items[i];
            if (tmp != null && tmp.GetItemType() == eItemType.Character && tmp.name == name)
            {
                item = tmp as CharacterItem;
                break;
            }
        }
        return item;
    }

    private void DelDialogItem()
    {
        DialogItem item = null;
        BaseDialogItem tmp;
        for (int i = 0; i < _itemCount; i++)
        {
            tmp = _items[i];
            if (tmp != null && tmp.GetItemType() == eItemType.Dialog)
            {
                item = tmp as DialogItem;
                item.Delete();
                return;
            }
        }
    }

    /// <summary>
    /// 游戏暂停，隐藏剧情界面
    /// </summary>
    private void OnSTGPause()
    {
        _isHiddenByPause = true;
        _containerGo.SetActive(false);
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    private void OnSTGContinue()
    {
        _isHiddenByPause = false;
        _containerGo.SetActive(true);
    }

    private void OnRetry()
    {
        ClearAllItems();
        if (_isHiddenByPause)
        {
            _containerGo.SetActive(true);
            _isHiddenByPause = false;
        }
    }

    private void UpdateDialog(int dFrame = 1)
    {
        if (Global.IsPause) return;
        BaseDialogItem item;
        for (int i = 0; i < _itemCount; i++)
        {
            item = _items[i];
            if (item == null)
                continue;
            item.Update(dFrame);
            if (!item.isEnable)
            {
                item.Clear();
                _items[i] = null;
            }
        }

        _timeSinceStarted++;
    }

    public override void OnHide()
    {
        ClearAllItems();
        CommandManager.GetInstance().Remove(CommandConsts.StartDialog, this);
        CommandManager.GetInstance().Remove(CommandConsts.CreateDialogCG, this);
        CommandManager.GetInstance().Remove(CommandConsts.HighlightDialogCG, this);
        CommandManager.GetInstance().Remove(CommandConsts.FadeOutDialogCG, this);
        CommandManager.GetInstance().Remove(CommandConsts.CreateDialogBox, this);
        CommandManager.GetInstance().Remove(CommandConsts.DelDialogBox, this);
        CommandManager.GetInstance().Remove(CommandConsts.UpdateDialog, this);
        CommandManager.GetInstance().Remove(CommandConsts.PauseGame, this);
        CommandManager.GetInstance().Remove(CommandConsts.ContinueGame, this);
        CommandManager.GetInstance().Remove(CommandConsts.ContinueGameAfterGameOver, this);
        CommandManager.GetInstance().Remove(CommandConsts.RetryGame, this);
        CommandManager.GetInstance().Remove(CommandConsts.RetryStage, this);
        CommandManager.GetInstance().Remove(CommandConsts.BackToTitle, this);
    }

    private void ClearAllItems()
    {
        for (int i = 0; i < _itemCount; i++)
        {
            if (_items[i] != null)
            {
                _items[i].Clear();
            }
        }
        _items.Clear();
        _itemCount = 0;
    }

    public override LayerId GetLayerId()
    {
        return LayerId.GameUI_Bottom;
    }
}
