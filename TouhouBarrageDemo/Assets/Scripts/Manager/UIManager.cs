﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager {

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

    private Dictionary<string, ViewBase> _viewsMap;

    private RectTransform _uiRootTf;
    private GameObject _stgRoot;
    /// <summary>
    /// STG本体的最上层layer
    /// </summary>
    private Transform _stgLayerTf;
    /// <summary>
    /// STG本体相机
    /// </summary>
    private Camera _stgCamera;
    /// <summary>
    /// 需要执行update方法的view
    /// </summary>
    private List<ViewBase> _viewUpdateList;
    /// <summary>
    /// 需要执行update方法的view的数目
    /// </summary>
    private int _viewUpdateCount;

    public void Init()
    {
        _stgRoot = GameObject.Find("GameMainCanvas");
        _stgLayerTf = _stgRoot.transform.Find("GameLayer");
        _stgCamera = _stgRoot.transform.Find("GameCamera").GetComponent<Camera>();
        _stgCamera.cullingMask = 0;
        // STG里面各个层级
        _layersMap = new Dictionary<LayerId, Transform>();
        _layersMap.Add(LayerId.STGBottomView, _stgLayerTf.Find("STGBottomViewLayer"));
        _layersMap.Add(LayerId.STGBottomEffect, _stgLayerTf.Find("STGBottomEffectLayer"));
        _layersMap.Add(LayerId.PlayerBarage, _stgLayerTf.Find("PlayerBarrageLayer"));
        _layersMap.Add(LayerId.Player, _stgLayerTf.Find("PlayerLayer"));
        _layersMap.Add(LayerId.Enemy, _stgLayerTf.Find("EnemyLayer"));
        _layersMap.Add(LayerId.Item, _stgLayerTf.Find("ItemLayer"));
        _layersMap.Add(LayerId.STGNormalEffect, _stgLayerTf.Find("STGNormalEffectLayer"));
        _layersMap.Add(LayerId.EnemyBarrage, _stgLayerTf.Find("EnemyBarrageLayer"));
        _layersMap.Add(LayerId.PlayerCollisionPoint, _stgLayerTf.Find("PlayerCollisionPointLayer"));
        _layersMap.Add(LayerId.STGTopEffect, _stgLayerTf.Find("STGTopEffectLayer"));
        _layersMap.Add(LayerId.GameInfo, _stgLayerTf.Find("GameInfoLayer"));
        if ( _viewsMap == null )
        {
            _viewsMap = new Dictionary<string, ViewBase>();
        }
        _uiRootTf = GameObject.Find("UIRoot").GetComponent<RectTransform>();
        _layersMap.Add(LayerId.GameUI_Bottom, _uiRootTf.Find("BottomLayer"));
        _layersMap.Add(LayerId.GameUI_Normal, _uiRootTf.Find("NormalLayer"));
        _layersMap.Add(LayerId.GameUI_Top, _uiRootTf.Find("TopLayer"));
        if ( _viewUpdateList == null )
        {
            _viewUpdateList = new List<ViewBase>();
        }
        _viewUpdateCount = 0;
        DoScreenAdaption();
    }

    public void RemoveBulletFromLayer(GameObject bullet)
    {
        bullet.SetActive(false);
    }

    public void AddGoToLayer(GameObject go,LayerId layerId)
    {
        if ( go != null )
        {
            Transform layerTf;
            if (_layersMap.TryGetValue(layerId, out layerTf))
            {
                go.transform.SetParent(layerTf, false);
                //go.transform.localScale = Vector3.one;
            }
            else
            {
                Logger.Log("Get layer fail,layerId = " + layerId);
            }
        }
    }

    public void AddGoToLayer(GameObject go, LayerId layerId,Vector3 pos)
    {
        if (go != null)
        {
            Transform layerTf;
            if (_layersMap.TryGetValue(layerId, out layerTf))
            {
                go.transform.SetParent(layerTf, false);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = pos;
            }
            else
            {
                Logger.Log("Get layer fail,layerId = " + layerId);
            }
        }
    }
    private Vector3 removeVector = new Vector3(2000, 2000, 0);
    public void RemoveGoFromLayer(GameObject go)
    {
        if ( go != null )
        {
            //go.SetActive(false);
            go.transform.localPosition = removeVector;
        }
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
        float scaleHeight = Screen.height / Consts.RefResolutionY;
        GameObject gameLayer = GameObject.Find("GameLayer");
        Transform tf = gameLayer.transform;
        // STG游戏部分的实际宽度 = 总宽度- (左边框宽度 + 右边框宽度)
        float stgWidth = Screen.width - (32 + 224) * scaleHeight;
        float scaleWidth = stgWidth / Consts.GameWidth;
        tf.localScale = new Vector3(scaleWidth, scaleHeight, 1);
        float rectX = 32 * scaleHeight / Screen.width;
        float rectY = 1f / 30;
        float rectWidth = stgWidth / Screen.width;
        float rectHeight = 1 - 2 * rectY;
        _stgCamera.rect = new Rect(rectX, rectY, rectWidth, rectHeight);
        // 背景部分
        Transform bgTf = GameObject.Find("GameMainCanvas/BgLayer").transform;
        bgTf.localScale = new Vector3(scaleWidth, scaleHeight, 1);
        Global.STGAcutalScaleWidth = scaleWidth;
        Global.STGAcutalScaleHeight = scaleHeight;
        Global.STGActualSize = new Vector2(Consts.GameWidth*scaleWidth,Consts.GameHeight*scaleHeight);
    }

    public void ShowView(string name,object data=null)
    {
        ViewBase view;
        if ( !_viewsMap.TryGetValue(name,out view) )
        {
            GameObject viewGO = ResourceManager.GetInstance().GetCommonPrefab("Prefab/Views", name);
            if ( viewGO != null )
            {
                System.Type classType = System.Type.GetType(name);
                view = System.Activator.CreateInstance(classType) as ViewBase;
                view.Init(viewGO);
                _viewsMap.Add(name, view);
                Transform parentTf;
                if ( !_layersMap.TryGetValue(view.GetLayerId(),out parentTf) )
                {
                    parentTf = _uiRootTf.Find("NormalLayer");
                }
                viewGO.transform.SetParent(parentTf, false);
            }
        }
        if ( view != null )
        {
            view.Show(data);
        }
    }

    public void HideView(string name)
    {
        ViewBase view;
        if ( _viewsMap.TryGetValue(name, out view) )
        {
            view.Hide();
        }
    }

    public void DestroyView(string name)
    {
        ViewBase view;
        if (_viewsMap.TryGetValue(name, out view))
        {
            view.Destroy();
            _viewsMap.Remove(name);
        }
    }

    public void Update()
    {
        bool needClear = false;
        ViewBase view;
        for (int i=0;i<_viewUpdateCount;i++)
        {
            view = _viewUpdateList[i];
            if ( view != null )
            {
                view.Update();
            }
            else
            {
                needClear = true;
            }
        }
        if ( needClear )
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
        if ( _viewUpdateList.IndexOf(view) == -1 )
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
        if ( index != -1 )
        {
            _viewUpdateList[index] = null;
        }
    }

    /// <summary>
    /// 获取游戏本体摄像机
    /// </summary>
    /// <returns></returns>
    public Camera GetSTGCamera()
    {
        return _stgCamera;
    }

    /// <summary>
    /// 获取STG本体的layer
    /// </summary>
    /// <returns></returns>
    public Transform GetSTGLayerTf()
    {
        return _stgLayerTf;
    }

    public Vector2 GetUIRootSize()
    {
        return _uiRootTf.rect.size;
    }
}

public enum LayerId : int
{
    STGBottomView = 110,
    STGBottomEffect = 130,
    STGNormalEffect = 150,
    Item = 250,
    Enemy = 300,
    EnemyBarrage = 500,
    Player = 400,
    PlayerBarage = 200,
    PlayerCollisionPoint = 600,
    STGTopEffect = 650,
    GameInfo = 700,
    UI = 100,
    GameUI_Bottom = 900,
    GameUI_Normal = 1000,
    GameUI_Top = 1100,
}
