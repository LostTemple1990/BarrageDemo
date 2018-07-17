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

    private GameObject _bgLayer;
    private Transform _bgTf;
    private Camera _bgCamera;
    private Transform _bgCameraTf;

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

    public void Update()
    {
        if ( _isCameraMoving )
        {
            MoveCamera();
        }
        foreach(BgBlockContainer container in _containerList)
        {
            container.Update();
        }
        //MoveBg();
        //UpdatePos();
    }

    private void MoveCamera()
    {
        _bgCameraTf.rotation = Quaternion.Euler(MathUtil.GetEaseInOutQuadInterpolation(94, 30, _cameraMoveTime, _cameraMoveDuration), 0, 0);
        _bgCamera.fieldOfView = MathUtil.GetLinearInterpolation(45, 20, _cameraMoveTime, _cameraMoveDuration);
        _bgCameraTf.localPosition = MathUtil.GetEaseInOutQuadInterpolation(new Vector3(0, 2.8f, 0.3f), new Vector3(0, 3.43f, -4.33f), _cameraMoveTime, _cameraMoveDuration);
        _cameraMoveTime++;
        if ( _cameraMoveTime >= _cameraMoveDuration )
        {
            _isCameraMoving = false;
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
