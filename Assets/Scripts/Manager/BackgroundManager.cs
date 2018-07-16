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

    private Dictionary<int, Canvas> _canvasMap;

    public void Init()
    {
        if ( _canvasMap == null )
        {
            _canvasMap = new Dictionary<int, Canvas>();
        }
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
    
    public void AddBgBlocks(BgBlock block,int blockCount,int sortingOrder=0)
    {
        Canvas canvas;
        if ( !_canvasMap.TryGetValue(sortingOrder,out canvas) )
        {
            canvas = new Canvas();
            canvas.sortingOrder = sortingOrder;
            canvas.transform.parent = _bgTf;
        }
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

class BgBlockPanel
{
    private float _height;
    private float _speed;
    private Canvas _canvas;
    private Transform _canvasTf;
    private float _curPosZ;
    public BgBlockPanel(int sortingOrder)
    {
        _canvas = new Canvas();
        _canvasTf = _canvas.transform;
        _canvas.sortingOrder = sortingOrder;
        _curPosZ = 0;
    }

    public void AddBlocks(BgBlock block,int count)
    {
        block.AddToPanel(_canvas.transform);
        float posZ = 0;
        for (int i=1;i<count;i++)
        {
            BgBlock cloneBlock = block.Clone();
            cloneBlock.AddToPanel(_canvas.transform);
            posZ += block.height;
            cloneBlock.SetToPos(new Vector3(0, 0, posZ));
        }
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void Update()
    {
        
    }

    public static BgBlockPanel Create(BgBlock block,int count,int sortingOrder)
    {
        BgBlockPanel panel = new BgBlockPanel(sortingOrder);
        panel.AddBlocks(block, count);
        return panel;
    }

    public void Clear()
    {

    }
}
