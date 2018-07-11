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
    private Transform _cameraTf;

    private Vector3 _curPos;
    private float _speedZ;

    private bool _isCameraMoving;
    private int _cameraMoveTime;
    private int _cameraMoveDuration;

    private float _isBgMoving;
    private int _bgMoveTime;
    private int _bgMoveDuration;

    public void Init()
    {
        GameObject go = GameObject.Find("Background");
        _bgTf = go.transform.Find("BgLayer");
        _cameraTf = go.transform.Find("BgCamera");
        _curPos = Vector3.zero;
        _speedZ = -0.008f;

        _isCameraMoving = true;
        _cameraMoveTime = 0;
        _cameraMoveDuration = 150;
    }

    public void Update()
    {
        if ( _isCameraMoving )
        {
            MoveCamera();
        }
        MoveBg();
        UpdatePos();
    }

    private void MoveCamera()
    {
        _cameraTf.rotation = Quaternion.Euler(MathUtil.GetEaseInOutQuadInterpolation(94, 80, _cameraMoveTime, _cameraMoveDuration), 0, 0);
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

    public void Clear()
    {

    }
}
