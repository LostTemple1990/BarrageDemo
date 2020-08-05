using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectCharView : ViewBase, ICommand
{
    private const float BgWidth = 512;
    private const float BgHeight = 512;

    private static Vector2 CGDefaultPos = new Vector2(-128, 0);
    private static Vector2 CGLeftAppearPos = new Vector2(-158, 0);
    private static Vector2 CGRightAppearPos = new Vector2(-98, 0);

    /// <summary>
    /// 背景图片go
    /// </summary>
    private RectTransform _bgTf;
    /// <summary>
    /// 角色立绘go
    /// </summary>
    private RectTransform _charCGTf;
    private Image _charCGImg;
    /// <summary>
    /// 角色描述go
    /// </summary>
    private RectTransform _charDesTf;
    private Image _charDesImg;

    private bool _isAniFinish;
    /// <summary>
    /// 当前选择的索引
    /// </summary>
    private int _selectIndex;

    private List<SelCharacterData> _charDatas;

    struct SelCharacterData
    {
        public string spName;
        public string desSp;
        public int index;
    }

    enum eViewState : byte
    {
        Wait = 0,
        Normal = 1,
        ChangeCharAni = 2,
    }

    private eViewState _viewState;
    private int _stateTime;

    public SelectCharView()
    {
        _charDatas = new List<SelCharacterData>();
        SelCharacterData data;
        data = new SelCharacterData
        {
            spName = "ReimuCG",
            desSp = "ReimuDes",
            index = 0,
        };
        _charDatas.Add(data);
        data = new SelCharacterData
        {
            spName = "MarisaCG",
            desSp = "MarisaDes",
            index = 1,
        };
        _charDatas.Add(data);
    }

    public void Execute(int cmd, object data)
    {
        
    }

    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
        // 背景
        _bgTf = _viewTf.Find("Bg").GetComponent<RectTransform>();

        _charCGTf = _viewTf.Find("CG").GetComponent<RectTransform>();
        _charCGImg = _charCGTf.GetComponent<Image>();
        _charDesTf = _viewTf.Find("DesImg").GetComponent<RectTransform>();
        _charDesImg = _charDesTf.GetComponent<Image>();
    }

    public override void Adaptive()
    {
        // 计算背景图在实际UI尺寸下的大小
        Vector2 uiSize = UIManager.GetInstance().GetUIRootSize();
        float refFactor = uiSize.x / uiSize.y;
        float imgFactor = BgWidth / BgHeight;
        float bgWidth, bgHeight;
        if (refFactor >= imgFactor)
        {
            bgWidth = uiSize.x;
            bgHeight = Mathf.Round(uiSize.x / BgWidth * BgHeight);
        }
        else
        {
            bgHeight = uiSize.y;
            bgWidth = Mathf.Round(uiSize.y / BgHeight * BgWidth);
        }
        _bgTf.sizeDelta = new Vector2(bgWidth, bgHeight);
    }

    protected override void OnShow(object data = null)
    {
        SetSelectIndex(0, Consts.DIR_NULL);
        UIManager.GetInstance().RegisterViewUpdate(this);
    }

    protected override void OnHide()
    {

        base.OnHide();
    }

    public override void Update()
    {
        if (_viewState == eViewState.ChangeCharAni)
        {
            
        }
        else if (_viewState == eViewState.Normal)
        {
            OnNormalStateUpdate();
        }
        else if (_viewState == eViewState.Wait)
        {
            _stateTime++;
            if (_stateTime >= 10)
            {
                _viewState = eViewState.Normal;
            }
        }
    }

    private void OnNormalStateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnPressLeftArrow();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnPressRightArrow();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            OnPressKeyZ();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPressKeyESC();
        }
    }

    private void OnPressRightArrow()
    {
        int next = _selectIndex + 1;
        if (next >= _charDatas.Count)
            next = 0;
        SetSelectIndex(next, Consts.DIR_RIGHT);
        SoundManager.GetInstance().Play("se_select", Consts.DefaultUISEVolume, false, false);
    }

    private void OnPressLeftArrow()
    {
        int next = _selectIndex - 1;
        if (next < 0)
            next = _charDatas.Count - 1;
        SetSelectIndex(next, Consts.DIR_LEFT);
        SoundManager.GetInstance().Play("se_select", Consts.DefaultUISEVolume, false, false);
    }

    private void OnPressKeyZ()
    {
        SoundManager.GetInstance().Play("se_selectok", Consts.DefaultUISEVolume, false, false);
        SelCharacterData data = _charDatas[_selectIndex];
        CommandManager.GetInstance().RunCommand(CommandConsts.SelectCharacter, data.index);
    }

    private void OnPressKeyESC()
    {
        CommandManager.GetInstance().RunCommand(CommandConsts.CancelSelectCharacter);
        Hide();
        SoundManager.GetInstance().Play("se_selectcancel", Consts.DefaultUISEVolume, false, false);
    }


    /// <summary>
    /// 设置主界面当前选中的选项
    /// </summary>
    /// <param name="index"></param>
    private void SetSelectIndex(int index,int appearDir)
    {
        _selectIndex = index;
        SelCharacterData data = _charDatas[_selectIndex];
        _charCGImg.sprite = ResourceManager.GetInstance().GetSprite("SelCharView", data.spName);
        _charDesImg.sprite = ResourceManager.GetInstance().GetSprite("SelCharView", data.desSp);

        if (appearDir == Consts.DIR_NULL)
        {
            _viewState = eViewState.Wait;
            _stateTime = 0;
        }
        else
        {
            TweenRotation tween0 = TweenManager.GetInstance().Create<TweenRotation>();
            tween0.SetParas(_charDesTf.gameObject, 0, 30, ePlayMode.Once);

            TweenAnchoredPos tween1 = TweenManager.GetInstance().Create<TweenAnchoredPos>();
            tween1.SetParas(_charCGTf.gameObject, 0, 30, ePlayMode.Once);

            if (appearDir == Consts.DIR_LEFT)
            {
                _charDesTf.localEulerAngles = new Vector3(0, 270, 0);
                tween0.SetParas(new Vector3(0, 360, 0), InterpolationMode.Linear);
                _charCGTf.anchoredPosition = CGLeftAppearPos;
            }
            else
            {
                _charDesTf.localEulerAngles = new Vector3(0, 90, 0);
                tween0.SetParas(Vector3.zero, InterpolationMode.Linear);
                _charCGTf.anchoredPosition = CGRightAppearPos;
            }
            tween1.SetParas(CGDefaultPos, InterpolationMode.EaseInQuad);
            tween1.SetFinishCallBack(SelCharTweenCallback);
            TweenManager.GetInstance().AddTween(tween0);
            TweenManager.GetInstance().AddTween(tween1);
            _viewState = eViewState.ChangeCharAni;
        }
    }

    private void SelCharTweenCallback(GameObject go)
    {
        _viewState = eViewState.Wait;
        _stateTime = 0;
    }

    public override LayerId GetLayerId()
    {
        return LayerId.GameUI_Normal;
    }
}
