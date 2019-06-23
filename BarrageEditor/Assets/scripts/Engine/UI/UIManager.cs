using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace YKEngine
{
    public class UIManager
    {

        private static UIManager _instance;

        public static UIManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }

        private UIManager()
        {

        }

        private Dictionary<LayerId, Transform> _layersMap;

        private Vector3 _hideVector = new Vector3(2000, 2000, 0);

        private Dictionary<int, ViewBase> _viewsMap;

        private RectTransform _uiRootTf;
        private GameObject _uiRoot;
        /// <summary>
        /// rootTf的宽度
        /// </summary>
        private float _uiRootWidth;
        /// <summary>
        /// rootTf的高度
        /// </summary>
        private float _uiRootHeight;
        /// <summary>
        /// 添加ui的默认位置
        /// </summary>
        private Vector2 _uiDefaultPos;
        /// <summary>
        /// STG本体的最上层layer
        /// </summary>
        private Transform _stgLayerTf;
        /// <summary>
        /// UICamera
        /// </summary>
        private Camera _uiCamera;
        /// <summary>
        /// 需要执行update方法的view
        /// </summary>
        private List<ViewBase> _viewUpdateList;
        /// <summary>
        /// 需要执行update方法的view的数目
        /// </summary>
        private int _viewUpdateCount;

        private Dictionary<int, ViewCfg> _viewCfgs;

        private EventSystem _eventSystem;
        private GraphicRaycaster _rayCaster;
        /// <summary>
        /// 当前焦点的界面
        /// </summary>
        private UView _curFocusView;

        private List<int> _focusViewStack;

        public void Init()
        {
            _uiRoot = GameObject.Find("UIRoot");
            _uiRootTf = _uiRoot.GetComponent<RectTransform>();
            Rect rootRect = _uiRootTf.rect;
            _uiRootWidth = rootRect.width;
            _uiRootHeight = rootRect.height;
            _uiDefaultPos = new Vector2(_uiRootWidth / 2, _uiRootHeight / 2);
            _stgLayerTf = _uiRoot.transform.Find("GameLayer");
            _uiCamera = _uiRoot.transform.Find("UICamera").GetComponent<Camera>();
            // UIRoot层级
            _layersMap = new Dictionary<LayerId, Transform>();
            int layerCount = (int)LayerId.LayerCount;
            for (int i = 1; i < layerCount ; i++)
            {
                LayerId tmpLayerId = (LayerId)i;
                GameObject layerGo = new GameObject("Layer_" + tmpLayerId.ToString());
                RectTransform layerTf = layerGo.AddComponent<RectTransform>();
                layerTf.SetParent(_uiRootTf, false);
                layerTf.localPosition = new Vector3(0, 0, (layerCount-i) * 250);
                layerTf.anchorMin = Vector2.zero;
                layerTf.anchorMax = Vector2.one;
                layerTf.sizeDelta = Vector2.zero;
                // 添加到layerMap中
                _layersMap.Add(tmpLayerId, layerTf);
            }
            if (_viewsMap == null)
            {
                _viewsMap = new Dictionary<int, ViewBase>();
            }
            if (_viewUpdateList == null)
            {
                _viewUpdateList = new List<ViewBase>();
            }
            _viewUpdateCount = 0;
            InitFocus();

            DoScreenAdaption();
        }

        private void InitFocus()
        {
            _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            _rayCaster = _uiRoot.GetComponent<GraphicRaycaster>();
            _curFocusView = null;
            _focusViewStack = new List<int>();
        }

        private void UIRootClickHandler()
        {
            PointerEventData eventData = new PointerEventData(_eventSystem);
            eventData.position = Input.mousePosition;
            List<RaycastResult> resultList = new List<RaycastResult>();
            _rayCaster.Raycast(eventData, resultList);
            // 检测到第一个界面
            foreach (RaycastResult result in resultList)
            {
                GameObject curGo = result.gameObject;
                Transform curTf = curGo.transform;
                while ( curTf.GetComponent<UView>() == null )
                {
                    curTf = curTf.parent;
                }
                UView focusView = curTf.GetComponent<UView>();
                if (focusView.focusWhenOpen)
                {
                    FocusOnView(focusView.GetView().ViewId);
                    break;
                }
            }
        }

        public void InitViewCfgs(Dictionary<int, ViewCfg> viewCfgs)
        {
            _viewCfgs = viewCfgs;
        }

        /// <summary>
        /// 隐藏当前的go(移动到当前屏幕看不见的地方)
        /// </summary>
        /// <param name="go"></param>
        public void HideGo(GameObject go)
        {
            if (go != null)
            {
                go.transform.localPosition = _hideVector;
            }
        }

        public void DoScreenAdaption()
        {

        }

        public void OpenView(int viewId, object data = null)
        {
            OpenView(viewId, data, _uiDefaultPos);
        }

        public void OpenView(int viewId, object data, Vector2 pos)
        {
            ViewCfg cfg = null;
            if (!_viewCfgs.TryGetValue(viewId, out cfg))
            {
                return;
            }
            ViewBase view;
            if (!_viewsMap.TryGetValue(viewId, out view))
            {
                GameObject viewGo = ResourceManager.GetInstance().GetPrefab("Prefabs/Views", cfg.resPath);
                if (viewGo != null)
                {
                    // 创建viewBase
                    System.Type classType = System.Type.GetType(cfg.className);
                    view = System.Activator.CreateInstance(classType) as ViewBase;
                    // uviw初始化，切换窗口焦点
                    // 切换焦点
                    UView uview = viewGo.GetComponent<UView>();
                    uview.Init(view);
                    // view初始化
                    view.Init(viewId,viewGo);
                    _viewsMap.Add(viewId, view);
                    Transform parentTf;
                    if (!_layersMap.TryGetValue(cfg.layer, out parentTf))
                    {
                        parentTf = _layersMap[LayerId.Normal];
                    }
                    RectTransform viewTf = viewGo.GetComponent<RectTransform>();
                    viewTf.SetParent(parentTf, false);
                    SetUIPosition(viewTf, pos);
                    // 设置焦点界面
                    if (uview.focusWhenOpen)
                    {
                        FocusOnView(viewId);
                    }
                    // OnShow接口
                    view.OnShowImpl(data);
                }
            }
            else
            {
                GameObject viewGo = view.GetViewObject();
                // 切换焦点
                UView uview = viewGo.GetComponent<UView>();
                if (uview.focusWhenOpen)
                {
                    FocusOnView(viewId);
                }
                view.Refresh(data);
                viewGo.transform.SetSiblingIndex(-1);
            }
        }

        public void CloseView(int viewId)
        {
            ViewBase view;
            if (_viewsMap.TryGetValue(viewId, out view))
            {
                view.OnCloseImpl();
                _viewsMap.Remove(viewId);
                RemoveFocusStack(viewId);
            }
        }

        /// <summary>
        /// 当前UI是否打开
        /// </summary>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public bool IsOpen(int viewId)
        {
            return _viewsMap.ContainsKey(viewId);
        }

        /// <summary>
        /// 获取指定UI的控制类
        /// </summary>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public ViewBase GetView(int viewId)
        {
            ViewBase view;
            if (_viewsMap.TryGetValue(viewId, out view))
            {
                return view;
            }
            return null;
        }

        public void Update()
        {
            CheckFocus();
            UpdateViews();
        }

        /// <summary>
        /// UI焦点检测
        /// </summary>
        private void CheckFocus()
        {
            if ( Input.GetMouseButton(0) )
            {
                UIRootClickHandler();
            }
        }

        /// <summary>
        ///调用ViewBase的Update函数
        /// </summary>
        private void UpdateViews()
        {
            bool needClear = false;
            ViewBase view;
            for (int i = 0; i < _viewUpdateCount; i++)
            {
                view = _viewUpdateList[i];
                if (view != null)
                {
                    view.Update();
                }
                else
                {
                    needClear = true;
                }
            }
            if (needClear)
            {
                _viewUpdateCount = CommonUtils.RemoveNullElementsInList<ViewBase>(_viewUpdateList);
            }
        }

        /// <summary>
        /// 注册update函数
        /// </summary>
        /// <param name="view"></param>
        public void RegisterViewUpdate(ViewBase view)
        {
            if (_viewUpdateList.IndexOf(view) == -1)
            {
                _viewUpdateList.Add(view);
                _viewUpdateCount++;
            }
        }

        /// <summary>
        /// 撤销注册update函数
        /// </summary>
        /// <param name="view"></param>
        public void UnregisterViewUpdate(ViewBase view)
        {
            int index = _viewUpdateList.IndexOf(view);
            if (index != -1)
            {
                _viewUpdateList[index] = null;
            }
        }

        /// <summary>
        /// 获取游戏本体摄像机
        /// </summary>
        /// <returns></returns>
        public Camera GetUICamera()
        {
            return _uiCamera;
        }

        public Vector2 GetUIPosition(RectTransform rectTf)
        {
            Vector2 localPos = _uiRootTf.InverseTransformPoint(rectTf.position);
            return new Vector2(localPos.x + _uiRootWidth / 2, localPos.y + _uiRootHeight / 2);
        }

        public void SetUIPosition(RectTransform rectTf,Vector2 pos)
        {
            float z = rectTf.position.z;
            Vector2 localPos = new Vector2(pos.x - _uiRootWidth / 2, pos.y - _uiRootHeight / 2);
            Vector3 newPos = _uiRootTf.TransformPoint(localPos);
            newPos.z = z;
            rectTf.position = newPos;
        }

        private void FocusOnView(int viewId)
        {
            int index = _focusViewStack.IndexOf(viewId);
            if ( index != -1)
            {
                // 若viewId对应的界面正是当前的焦点界面
                // 则直接返回
                if (viewId == GetFocusViewId())
                {
                    return;
                }
                _focusViewStack.RemoveAt(index);
            }
            _focusViewStack.Add(viewId);
            ViewBase view = GetView(viewId);
            if (view != null)
            {
                _curFocusView = view.GetViewObject().GetComponent<UView>();
            }
            else
            {
                throw new Exception(string.Format("Add focus stack error!view of id {0} is not exitst.", viewId));
            }
            EventManager.GetInstance().PostEvent(EngineEventID.WindowFocusChanged, viewId);
        }

        private void RemoveFocusStack(int viewId)
        {
            int index = _focusViewStack.IndexOf(viewId);
            if (index == -1)
                return;
            // 不是最上层的焦点界面
            if (index != _focusViewStack.Count - 1)
            {
                _focusViewStack.RemoveAt(index);
            }
            else
            {
                _focusViewStack.RemoveAt(index);
                // 切换焦点界面
                if (_focusViewStack.Count > 0)
                {
                    int nextFocusId = _focusViewStack[_focusViewStack.Count - 1];
                    _curFocusView = GetView(nextFocusId).GetViewObject().GetComponent<UView>();
                    EventManager.GetInstance().PostEvent(EngineEventID.WindowFocusChanged, nextFocusId);
                }
                else
                {
                    _curFocusView = null;
                    Logger.Log("NoFocus View");
                }
            }
        }

        public int GetFocusViewId()
        {
            if (_focusViewStack.Count>0)
            {
                return _focusViewStack[_focusViewStack.Count - 1];
            }
            return -1;
        }
    }

    public enum LayerId : int
    {
        Bottom = 1,
        Normal = 2,
        Top = 3,
        LayerCount,
    }
}
