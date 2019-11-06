using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ReplayView : ViewBase, ICommand
{
    private const float BgWidth = 512;
    private const float BgHeight = 512;

    private const float InfoHeight = 32f;
    private const float DetailsHeight = 64f;
    /// <summary>
    /// ReplayItem的切换时间
    /// </summary>
    private const int TweenDuration = 15;

    private const int MaxInfoCount = 10;

    /// <summary>
    /// 背景图片go
    /// </summary>
    private RectTransform _bgTf;
    /// <summary>
    /// 选中的背景
    /// </summary>
    private RectTransform _selectedImgTf;

    private bool _isAniFinish;
    /// <summary>
    /// 当前选择的索引
    /// </summary>
    private int _selectIndex;

    private List<ReplayInfo> _infoList;
    private int _infoCount;

    private List<ReplayItem> _itemList;


    enum eViewState : byte
    {
        Wait = 0,
        Normal = 1,
        SelectNextAni = 2,
        EditReplayName = 3,
    }

    private eViewState _viewState;
    private int _stateTime;
    /// <summary>
    /// ReplayUI的打开模式
    /// </summary>
    private eReplayViewMode _mode;

    class ReplayItem
    {
        private GameObject _itemGo;
        private RectTransform _itemTf;
        private InputField _input;
        private GameObject _detailContainerGo;
        private Text _repNameText;
        private Text _durationText;
        private Text _dateText;
        private Text _charText;
        private Action<string> _editEndCallback;

        public void Init(GameObject go)
        {
            _itemGo = go;
            _itemTf = go.GetComponent<RectTransform>();
            RectTransform infoTf = _itemTf.Find("Info").GetComponent<RectTransform>();
            _input = infoTf.Find("Input").GetComponent<InputField>();
            _repNameText = infoTf.Find("ReplayNameText").GetComponent<Text>();
            _dateText = infoTf.Find("DateText").GetComponent<Text>();
            _charText = infoTf.Find("CharText").GetComponent<Text>();
            RectTransform detailTf = _itemTf.Find("Details").GetComponent<RectTransform>();
            _detailContainerGo = detailTf.gameObject;
            _durationText = detailTf.Find("DurationText").GetComponent<Text>();

            _detailContainerGo.SetActive(false);
            _input.gameObject.SetActive(false);
        }

        public void ShowDetails(bool show)
        {
            _detailContainerGo.SetActive(show);
        }

        public void SetInfo(ReplayInfo info)
        {
            if (info.replayIndex != -1)
            {
                _repNameText.text = info.name;
                DateTime date = new DateTime(info.dateTick);
                _dateText.text = date.ToString("yyyy/MM/dd");
                _charText.text = PlayerInterface.GetInstance().GetCharacterNameByIndex(info.characterIndex);

                date = new DateTime();
                int sec = Mathf.RoundToInt(info.lastFrame / 60);
                date = date.AddSeconds(sec);
                _durationText.text = date.ToString("mm:ss");
            }
            else
            {
                _repNameText.text = "---------------";
                _dateText.text = "----/--/--";
                _charText.text = "------";
                _durationText.text = "--:--";
            }
        }

        public void SetAnchoredPos(Vector2 pos)
        {
            _itemTf.anchoredPosition = pos;
        }

        public void TweenToPos(Vector2 pos,int duration)
        {
            TweenAnchoredPos tween = TweenManager.GetInstance().Create<TweenAnchoredPos>();
            tween.SetParas(_itemGo, 0, duration, ePlayMode.Once);
            tween.SetParas(pos, InterpolationMode.EaseOutQuad);
            TweenManager.GetInstance().AddTween(tween);
        }

        public void EditReplayName(Action<string> callback)
        {
            _repNameText.gameObject.SetActive(false);
            _input.gameObject.SetActive(true);
            _editEndCallback = callback;
            _input.ActivateInputField();
            _input.onEndEdit.AddListener(EditEndCallback);
        }

        private void EditEndCallback(string text)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelEdit();
            }
            else
            {
                _repNameText.text = text;
                Action<string> callback = _editEndCallback;
                CancelEdit();
                callback(text);
            }
        }

        public void CancelEdit()
        {
            _editEndCallback = null;
            _repNameText.gameObject.SetActive(true);
            _input.gameObject.SetActive(false);
            _input.onEndEdit.RemoveListener(EditEndCallback);
        }
    }

    public ReplayView()
    {
        
    }

    public void Execute(int cmd, object data)
    {
        if (cmd == CommandConsts.SaveReplaySuccess)
        {
            OnSaveReplaySuccess();
        }
    }

    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
        // 背景
        _bgTf = _viewTf.Find("Bg").GetComponent<RectTransform>();
        RectTransform containerTf = _viewTf.Find("Container").GetComponent<RectTransform>();
        _selectedImgTf = containerTf.Find("SelectedImg").GetComponent<RectTransform>();
        // 创建ReplayItem
        ReplayItem repItem;
        GameObject itemGo;
        _itemList = new List<ReplayItem>();
        for (int i=0;i<MaxInfoCount;i++)
        {
            repItem = new ReplayItem();
            itemGo = ResourceManager.GetInstance().GetCommonPrefab("Prefab/Views", "ReplayItem");
            itemGo.transform.SetParent(containerTf, false);
            repItem.Init(itemGo);
            _itemList.Add(repItem);
        }
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

    public override void OnShow(object data)
    {
        _mode = (eReplayViewMode)data;
        _selectIndex = -1;
        InitReplayItemInfo();
        SetSelectIndex(0,false);
        UIManager.GetInstance().RegisterViewUpdate(this);
    }

    private void InitReplayItemInfo()
    {
        // 初始化
        _infoList = new List<ReplayInfo>();
        ReplayInfo info;
        for (int i = 0; i < MaxInfoCount; i++)
        {
            info = new ReplayInfo
            {
                replayIndex = -1,
            };
            _infoList.Add(info);
        }
        // 读取replay的信息
        List<ReplayInfo> infos = ReplayManager.GetInstance().GetReplayInfoList();
        _infoCount = infos.Count;
        for (int i = 0; i < _infoCount; i++)
        {
            info = infos[i];
            _infoList[info.replayIndex] = info;
        }
        // 更新ReplayItem的显示
        for (int i = 0; i < MaxInfoCount; i++)
        {
            ReplayItem item = _itemList[i];
            item.SetInfo(_infoList[i]);
        }
    }

    public override void OnHide()
    {
        _infoList.Clear();
        if (_selectIndex != -1)
        {
            _itemList[_selectIndex].ShowDetails(false);
        }
        CommandManager.GetInstance().Remove(CommandConsts.SaveReplaySuccess, this);
        base.OnHide();
    }

    public override void Update()
    {
        if (_viewState == eViewState.SelectNextAni)
        {

        }
        else if (_viewState == eViewState.Normal)
        {
            OnNormalStateUpdate();
        }
        else if (_viewState == eViewState.Wait)
        {
            _stateTime++;
            if (_stateTime >= 1)
            {
                _viewState = eViewState.Normal;
            }
        }
        else if (_viewState == eViewState.EditReplayName)
        {
            OnEditReplayNameUpdate();
        }
    }

    #region NormalState
    private void OnNormalStateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnPressDownArrow();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnPressUpArrow();
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

    private void OnPressDownArrow()
    {
        int next = _selectIndex + 1;
        if (next >= MaxInfoCount)
            next = 0;
        SetSelectIndex(next, true);
    }

    private void OnPressUpArrow()
    {
        int next = _selectIndex - 1;
        if (next < 0)
            next = MaxInfoCount - 1;
        SetSelectIndex(next, true);
    }

    private void OnPressKeyZ()
    {
        if (_mode == eReplayViewMode.Save)
        {
            _viewState = eViewState.EditReplayName;
            _itemList[_selectIndex].EditReplayName(OnEditEndCallback);
        }
        else if (_mode == eReplayViewMode.Load)
        {
            ReplayInfo info = _infoList[_selectIndex];
            if (info.replayIndex != -1)
            {
                bool isSuccess = ReplayManager.GetInstance().LoadReplay(info.replayIndex);
                if (isSuccess)
                {
                    Logger.Log("Load replay" + info.replayIndex + " success!");
                }
                else
                {
                    Logger.Log("Load replay" + info.replayIndex + " fail!");
                }
            }
        }
    }

    private void OnPressKeyESC()
    {
        if (_mode == eReplayViewMode.Load)
        {
            CommandManager.GetInstance().RunCommand(CommandConsts.CancelSelectReplay);
            Hide();
        }
        else if (_mode == eReplayViewMode.Save)
        {
            Hide();
            CommandManager.GetInstance().RunCommand(CommandConsts.BackToTitle);
        }
    }
    #endregion

    private void OnEditReplayNameUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _itemList[_selectIndex].CancelEdit();
            _viewState = eViewState.Wait;
            _stateTime = 0;
        }
    }

    private void OnEditEndCallback(string repName)
    {
        ReplayInfo info = new ReplayInfo();
        info.name = repName;
        info.replayIndex = _selectIndex;
        CommandManager.GetInstance().Register(CommandConsts.SaveReplaySuccess, this);
        CommandManager.GetInstance().RunCommand(CommandConsts.SaveReplay, info);
    }

    private void OnSaveReplaySuccess()
    {
        InitReplayItemInfo();
        _viewState = eViewState.Wait;
        _stateTime = 0;
    }


    /// <summary>
    /// 设置主界面当前选中的选项
    /// </summary>
    /// <param name="index"></param>
    private void SetSelectIndex(int index,bool playAni)
    {
        ReplayItem item;
        if (_selectIndex != -1)
        {
            item = _itemList[_selectIndex];
            item.ShowDetails(false);
        }
        _selectIndex = index;
        if (!playAni)
        {
            float posY = 0;
            for (int i = 0; i < MaxInfoCount; i++)
            {
                item = _itemList[i];
                item.SetAnchoredPos(new Vector2(0, posY));
                if (i == _selectIndex)
                {
                    _selectedImgTf.anchoredPosition = new Vector2(0, posY);
                    item.ShowDetails(true);
                    posY -= DetailsHeight;
                }
                else
                {
                    posY -= InfoHeight;
                }
            }
            _viewState = eViewState.Wait;
            _stateTime = 0;
        }
        else
        {
            float posY = 0;
            for (int i = 0; i < MaxInfoCount; i++)
            {
                item = _itemList[i];
                Vector2 endPos = new Vector2(0, posY);
                item.TweenToPos(endPos, TweenDuration);
                if (i == _selectIndex)
                {
                    TweenAnchoredPos tween = TweenManager.GetInstance().Create<TweenAnchoredPos>();
                    tween.SetParas(_selectedImgTf.gameObject, 0, TweenDuration, ePlayMode.Once);
                    tween.SetParas(endPos, InterpolationMode.EaseOutQuad);
                    tween.SetFinishCallBack(SelItemTweenCallback);
                    TweenManager.GetInstance().AddTween(tween);
                    _viewState = eViewState.SelectNextAni;

                    posY -= DetailsHeight;
                }
                else
                {
                    posY -= InfoHeight;
                }
            }
        }
    }

    private void SelItemTweenCallback(GameObject go)
    {
        _itemList[_selectIndex].ShowDetails(true);
        _viewState = eViewState.Wait;
        _stateTime = 0;
    }

    public override LayerId GetLayerId()
    {
        return LayerId.GameUI_Normal;
    }
}

public enum eReplayViewMode : byte
{
    Save = 0,
    Load = 1,
}
