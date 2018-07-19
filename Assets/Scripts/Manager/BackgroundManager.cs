using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager
{
    private static BackgroundManager _instance = new BackgroundManager();

    public static BackgroundManager GetInstance()
    {
        return _instance;
    }

    private static int ClearDuration = 60;

    private GameObject _bgLayer;
    private Transform _bgTf;
    private Camera _bgCamera;
    private Transform _bgCameraTf;
    // 前景层的tf组件
    private Transform _foregroundLayerTf;
    // 先前的fov
    private float _preFov;
    /// <summary>
    /// 表示是否需要重新计算前景的scale
    /// 一般来说是摄像机的FOV改变了
    /// </summary>
    private bool _fgIsDirty;

    private Vector3 _curPos;
    private float _speedZ;

    private bool _isCameraMoving;
    private int _cameraMoveTime;
    private int _cameraMoveDuration;

    private float _isBgMoving;
    private int _bgMoveTime;
    private int _bgMoveDuration;

    private List<BgBlockContainer> _containerList;
    private int _containerCount;
    /// <summary>
    /// 背景精灵对象
    /// </summary>
    private List<BgSpriteObject> _spriteObjList;
    /// <summary>
    /// 背景精灵对象的数目
    /// </summary>
    private int _spriteObjCount;

    private int _clearTime;

    public BackgroundManager()
    {
        _spriteObjList = new List<BgSpriteObject>();
        _spriteObjCount = 0;
    }

    public void Init()
    {
        GameObject go = GameObject.Find("Background");
        _bgTf = go.transform.Find("BgLayer");
        _bgCameraTf = go.transform.Find("BgCamera");
        _bgCamera = _bgCameraTf.GetComponent<Camera>();
        _curPos = Vector3.zero;
        _speedZ = -0.008f;

        _isCameraMoving = true;
        _cameraMoveTime = 0;
        _cameraMoveDuration = 150;
        InitBg();
        _bgCamera.fieldOfView = 45;
        _bgCameraTf.localPosition = new Vector3(0,2.8f,0.3f);
        // 初始化前景层
        _foregroundLayerTf = _bgCameraTf.Find("ForegroundLayer");
        // 重置清除时间
        _clearTime = 0;
        _preFov = _bgCamera.fieldOfView;
    }

    private void InitBg()
    {
        _containerList = new List<BgBlockContainer>();
        CreateBgBlockContainer(_bgTf.Find("BgBlock").gameObject, 4);
        _containerCount = _containerList.Count;
    }

    public void CreateBgBlockContainer(GameObject blockObj,int count)
    {
        BgBlockContainer container = new BgBlockContainer(_bgTf);
        container.Init();
        container.SetBlockObject(blockObj, count);
        container.SetSpeed(_speedZ);
        container.SetDisappearZ(-2.56f);
        container.SetIntervalRangeZ(2.56f, 2.56f);
        container.GenerateBlocks(new Vector3(0,0,0));
        //添加到列表中
        _containerList.Add(container);
    }

    /// <summary>
    /// 创建单个背景精灵
    /// </summary>
    /// <param name="spName"></param>
    public BgSpriteObject CreateBgSpriteObject(string spName)
    {
        BgSpriteObject spObj = ObjectsPool.GetInstance().GetPoolClassAtPool<BgSpriteObject>();
        spObj.Init(_foregroundLayerTf);
        spObj.SetSprite(spName);
        return spObj;
    }

    /// <summary>
    /// 将背景精灵添加到列表中
    /// </summary>
    /// <param name="spObj"></param>
    public void AddBgSpriteObject(BgSpriteObject spObj)
    {
        _spriteObjList.Add(spObj);
        _spriteObjCount++;
    }

    public void Update()
    {
        if ( _isCameraMoving )
        {
            MoveCamera();
        }
        int i;
        foreach(BgBlockContainer container in _containerList)
        {
            container.Update();
        }
        BgSpriteObject spObj;
        for (i=0;i<_spriteObjCount;i++)
        {
            spObj = _spriteObjList[i];
            if ( spObj != null )
            {
                spObj.Update();
                if ( !spObj.IsActive )
                {
                    ObjectsPool.GetInstance().RestorePoolClassToPool(spObj);
                    _spriteObjList[i] = null;
                }
            }
        }
        _clearTime++;
        if ( _clearTime >= ClearDuration )
        {
            _clearTime = 0;
            _spriteObjCount = CommonUtils.RemoveNullElementsInList<BgSpriteObject>(_spriteObjList, _spriteObjCount);
        }
        //MoveBg();
        //UpdatePos();
    }

    private void MoveCamera()
    {
        _bgCameraTf.rotation = Quaternion.Euler(MathUtil.GetEaseOutQuadInterpolation(94, 30, _cameraMoveTime, _cameraMoveDuration), 0, 0);
        _bgCamera.fieldOfView = MathUtil.GetLinearInterpolation(45, 20, _cameraMoveTime, _cameraMoveDuration);
        _bgCameraTf.localPosition = MathUtil.GetEaseOutQuadInterpolation(new Vector3(0, 2.8f, 0.3f), new Vector3(0, 3.43f, -4.33f), _cameraMoveTime, _cameraMoveDuration);
        _cameraMoveTime++;
        if ( _cameraMoveTime >= _cameraMoveDuration )
        {
            _isCameraMoving = false;
        }
        if ( _preFov != _bgCamera.fieldOfView )
        {
            _preFov = _bgCamera.fieldOfView;
            //float scale = 2.25f / (5 * Mathf.Tan(_preFov * Mathf.Deg2Rad));
            float scale = (5 * Mathf.Tan(_preFov / 2 * Mathf.Deg2Rad)) / 2.25f;
            _foregroundLayerTf.localScale = new Vector3(scale, scale, 1);
        }
    }

    private void MoveBg()
    {
        _curPos.z += _speedZ;
        if ( _curPos.z <= -5.12 )
        {
            _curPos.z += 5.12f;
        }
    }

    private void UpdatePos()
    {
        _bgTf.localPosition = _curPos;
    }

    /// <summary>
    /// 设置背景相机的位置
    /// </summary>
    /// <param name="pos"></param>
    public void SetBgCameraPos(Vector3 pos)
    {
        _bgCameraTf.localPosition = pos;
    }

    /// <summary>
    /// 设置背景相机的视野
    /// </summary>
    /// <param name="value"></param>
    public void SetBgCameraFieldOfView(float value)
    {
        _bgCamera.fieldOfView = value;
    }

    public void Clear()
    {

    }
}
