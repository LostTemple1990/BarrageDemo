using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        public void Init()
        {
            _uiRoot = GameObject.Find("UIRoot");
            _uiRootTf = _uiRoot.GetComponent<RectTransform>();
            _stgLayerTf = _uiRoot.transform.Find("GameLayer");
            _uiCamera = _uiRoot.transform.Find("UICamera").GetComponent<Camera>();
            // UIRoot层级
            _layersMap = new Dictionary<LayerId, Transform>();
            int layerCount = (int)LayerId.LayerCount;
            for (int i = layerCount - 1; i >= 1; i--)
            {
                LayerId tmpLayerId = (LayerId)i;
                GameObject layerGo = new GameObject("Layer" + i);
                RectTransform layerTf = layerGo.AddComponent<RectTransform>();
                layerTf.SetParent(_uiRootTf, false);
                layerTf.localPosition = new Vector3(0, 0, i * 250);
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
            DoScreenAdaption();
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
            OpenView(viewId, data, Vector2.zero);
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
                    System.Type classType = System.Type.GetType(cfg.className);
                    view = System.Activator.CreateInstance(classType) as ViewBase;
                    view.Init(viewGo);
                    _viewsMap.Add(viewId, view);
                    Transform parentTf;
                    if (!_layersMap.TryGetValue(cfg.layer, out parentTf))
                    {
                        parentTf = _layersMap[LayerId.Normal];
                    }
                    RectTransform viewTf = viewGo.GetComponent<RectTransform>();
                    viewTf.SetParent(parentTf, false);
                    viewTf.localPosition = pos;
                    view.OnShowImpl(data);
                }
            }
            else
            {
                view.Refresh(data);
            }
        }

        public void CloseView(int viewId)
        {
            ViewBase view;
            if (_viewsMap.TryGetValue(viewId, out view))
            {
                view.OnCloseImpl();
                _viewsMap.Remove(viewId);
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
            return Vector2.zero;
        }

        /// <summary>
        /// 获取STG本体的layer
        /// </summary>
        /// <returns></returns>
        public Transform GetSTGLayerTf()
        {
            return _stgLayerTf;
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
