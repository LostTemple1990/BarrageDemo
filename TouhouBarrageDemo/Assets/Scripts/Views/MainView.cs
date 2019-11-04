using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainView : ViewBase, ICommand
{
    private const float BgWidth = 1320;
    private const float BgHeight = 825;

    private const float TitleItemStartPosX = 100;
    private const float TitleItemStartPosY = 200;
    private const float TitleItemIntervalY = -25;
    /// <summary>
    /// 动画效果出现的x坐标
    /// </summary>
    private const float TitleItemAppearPosX = -80;

    private const int IndexStartGame = 0;
    private const int IndexExtraStart = 1;
    private const int IndexPricticeStart = 2;
    private const int IndexReplay = 3;
    private const int IndexPlaerData = 4;
    private const int IndexOption = 5;
    private const int IndexQuit = 6;

    enum eMainViewState : byte
    {
        AppearAni = 0,
        Normal = 1,
        SelectChar = 2,
        SelectReplay = 3,
    }

    /// <summary>
    /// 背景图片go
    /// </summary>
    private RectTransform _bgTf;

    class TitleItem
    {
        public GameObject go;
        public RectTransform tf;
        public Image img;
        /// <summary>
        /// 出现效果的起始位置
        /// </summary>
        public Vector2 start;
        /// <summary>
        /// 出现效果的结束位置
        /// </summary>
        public Vector2 end;

        public void Clear()
        {
            go = null;
            tf = null;
            img = null;
        }
    }

    private List<TitleItem> _itemList;
    private int _itemCount;

    private eMainViewState _state;

    private int _availableBit;
    private List<int> _availableIndexList;

    private bool _isAniFinish;
    /// <summary>
    /// 当前选择的索引(对应itemList的索引
    /// </summary>
    private int _selectIndex;
    /// <summary>
    /// 当前可用的下标索引
    /// </summary>
    private int _curIndexInAvailableList;

    struct TitleItemData
    {
        public string goName;
        public string spName;
    }

    private List<TitleItemData> _titleItemDataList;

    private bool _isWaitingEvent;

    public MainView()
    {
        #region titleItem配置
        _titleItemDataList = new List<TitleItemData>();
        TitleItemData data;
        data = new TitleItemData
        {
            goName = "GameStartItem",
            spName = "GameStart",
        };
        _titleItemDataList.Add(data);
        data = new TitleItemData
        {
            goName = "ExtraStartItem",
            spName = "ExtraStart",
        };
        _titleItemDataList.Add(data);
        data = new TitleItemData
        {
            goName = "PracticeStartItem",
            spName = "PracticeStart",
        };
        _titleItemDataList.Add(data);
        data = new TitleItemData
        {
            goName = "ReplayItem",
            spName = "Replay",
        };
        _titleItemDataList.Add(data);
        data = new TitleItemData
        {
            goName = "PlayerDataItem",
            spName = "PlayerData",
        };
        _titleItemDataList.Add(data);
        data = new TitleItemData
        {
            goName = "OptionItem",
            spName = "Option",
        };
        _titleItemDataList.Add(data);
        data = new TitleItemData
        {
            goName = "QuitItem",
            spName = "Quit",
        };
        _titleItemDataList.Add(data);
        #endregion
    }

    public void Execute(int cmd, object data)
    {
        if (cmd == CommandConsts.CancelSelectCharacter)
        {
            _isWaitingEvent = false;
        }
        else if (cmd == CommandConsts.CancelSelectReplay)
        {
            _isWaitingEvent = false;
        }
    }

    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
        // 背景
        _bgTf = _viewTf.Find("Bg").GetComponent<RectTransform>();
        // 选项Go的名称
        _itemCount = _titleItemDataList.Count;
        _itemList = new List<TitleItem>();
        for (int i = 0; i < _itemCount; i++)
        {
            TitleItem item = new TitleItem();
            RectTransform tf = _viewTf.Find("Container/" + _titleItemDataList[i].goName).GetComponent<RectTransform>();
            item.tf = tf;
            item.go = tf.gameObject;
            item.img = tf.Find("Img").GetComponent<Image>();
            float posY = TitleItemStartPosY + i * TitleItemIntervalY;
            item.start = new Vector2(TitleItemAppearPosX, posY);
            item.end = new Vector2(TitleItemStartPosX, posY);
            _itemList.Add(item);
        }
    }

    public override void Adaptive()
    {
        float bgRealWidth = Mathf.Round(Consts.RefResolutionY / BgHeight * BgWidth);
        _bgTf.sizeDelta = new Vector2(bgRealWidth, Consts.RefResolutionY);
    }

    public override void OnShow(object data = null)
    {
        _selectIndex = -1;
        InitItemsOnShow();
        UIManager.GetInstance().RegisterViewUpdate(this);
    }

    private void InitItemsOnShow()
    {
        _availableBit = 0;
        _availableBit |= 1 << 0;
        _availableBit |= 1 << 3;
        _availableBit |= 1 << 6;
        _availableIndexList = new List<int>();
        for (int i = 0; i < _itemCount; i++)
        {
            if ((_availableBit & (1 << i)) != 0)
            {
                _availableIndexList.Add(i);
            }
        }
        TitleItem item;
        for (int i = 0; i < _itemCount; i++)
        {
            item = _itemList[i];
            item.tf.anchoredPosition = item.start;
            // 起始状态
            if ((_availableBit & (1 << i)) != 0)
            {
                item.img.sprite = ResourceManager.GetInstance().GetSprite("MainViewAtlas", _titleItemDataList[i].spName + "_1");
                item.img.color = new Color(1, 1, 1, 1);
            }
            else
            {
                item.img.sprite = ResourceManager.GetInstance().GetSprite("MainViewAtlas", _titleItemDataList[i].spName + "_0");
                item.img.color = new Color(0.25f, 0.25f, 0.25f, 1);
            }
            // 添加缓动动画
            TweenAnchoredPos tween = TweenManager.GetInstance().Create<TweenAnchoredPos>();
            tween.SetParas(item.go, 15 * i, 20, ePlayMode.Once);
            tween.SetParas(item.end, InterpolationMode.EaseInQuad);
            if (i == _itemCount - 1)
            {
                tween.SetFinishCallBack(OnAppearAniFinish);
            }
            TweenManager.GetInstance().AddTween(tween);
        }
        _isAniFinish = false;
        _state = eMainViewState.AppearAni;
    }

    private void OnAppearAniFinish(GameObject itemGo)
    {
        _isAniFinish = true;
    }

    public override void Update()
    {
        if (_state == eMainViewState.AppearAni)
        {
            OnAppearAniStateUpdate();
        }
        else if (_state == eMainViewState.Normal)
        {
            OnNormalStateUpdate();
        }
        else if (_state == eMainViewState.SelectChar)
        {
            OnSelCharacterStateUpdate();
        }
        else if (_state == eMainViewState.SelectReplay)
        {
            OnSelReplayStateUpdate();
        }
    }

    private void OnAppearAniStateUpdate()
    {
        if (_isAniFinish)
        {
            _state = eMainViewState.Normal;
            _curIndexInAvailableList = 0;
            SetSelectIndex(_availableIndexList[_curIndexInAvailableList]);
        }
    }

    private void OnNormalStateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnPressDownArrow();
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnPressUpArrow();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            OnPressKeyZ();
        }
    }

    private void OnPressDownArrow()
    {
        _curIndexInAvailableList = _curIndexInAvailableList + 1;
        if (_curIndexInAvailableList >= _availableIndexList.Count)
            _curIndexInAvailableList = 0;
        SetSelectIndex(_availableIndexList[_curIndexInAvailableList]);
    }

    private void OnPressUpArrow()
    {
        _curIndexInAvailableList = _curIndexInAvailableList - 1;
        if (_curIndexInAvailableList < 0)
            _curIndexInAvailableList = _availableIndexList.Count - 1;
        SetSelectIndex(_availableIndexList[_curIndexInAvailableList]);
    }

    private void OnPressKeyZ()
    {
        if (_selectIndex == IndexStartGame)
        {
            CommandManager.GetInstance().Register(CommandConsts.CancelSelectCharacter, this);
            _state = eMainViewState.SelectChar;
            _isWaitingEvent = true;
            UIManager.GetInstance().ShowView(WindowName.SelectCharView);
        }
        else if (_selectIndex == IndexReplay)
        {
            CommandManager.GetInstance().Register(CommandConsts.CancelSelectReplay, this);
            _state = eMainViewState.SelectReplay;
            _isWaitingEvent = true;
            UIManager.GetInstance().ShowView(WindowName.ReplayView, eReplayViewMode.Load);
        }
        else if (_selectIndex == IndexQuit)
        {

        }
    }

    /// <summary>
    /// 设置主界面当前选中的选项
    /// </summary>
    /// <param name="index"></param>
    private void SetSelectIndex(int index)
    {
        if (_selectIndex != -1)
        {
            TitleItem item = _itemList[_selectIndex];
            TweenManager.GetInstance().RemoveTweenByGo(item.img.gameObject);
            item.img.color = new Color(1, 1, 1, 1);
        }
        _selectIndex = index;
        if (_selectIndex != -1)
        {
            TitleItem item = _itemList[_selectIndex];
            TweenColor tween = TweenManager.GetInstance().Create<TweenColor>();
            tween.SetParas(item.img.gameObject, 0, 30, ePlayMode.PingPong);
            tween.SetParas(new Color(1, 1, 1, 1), new Color(0.5f, 0, 0, 1), InterpolationMode.Linear);
            TweenManager.GetInstance().AddTween(tween);
        }
    }

    private void OnSelCharacterStateUpdate()
    {
        if (!_isWaitingEvent)
        {
            _state = eMainViewState.Normal;
        }
    }

    private void OnSelReplayStateUpdate()
    {
        if (!_isWaitingEvent)
        {
            _state = eMainViewState.Normal;
        }
    }

    public override void OnHide()
    {
        SetSelectIndex(-1);
        CommandManager.GetInstance().Remove(CommandConsts.CancelSelectCharacter, this);
        base.OnHide();
    }

    public override LayerId GetLayerId()
    {
        return LayerId.GameUI_Normal;
    }
}
