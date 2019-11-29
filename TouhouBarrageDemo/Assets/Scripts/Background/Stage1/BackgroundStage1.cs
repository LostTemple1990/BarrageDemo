using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundStage1 : BackgroundBase
{
    // 先前的fov
    private float _preFov;
    /// <summary>
    /// 表示是否需要重新计算前景的scale
    /// 一般来说是摄像机的FOV改变了
    /// </summary>
    private bool _fgIsDirty;

    private bool _isCameraMoving;
    private int _cameraMoveTime;
    private int _cameraMoveDuration;

    private float _isBgMoving;
    private int _bgMoveTime;
    private int _bgMoveDuration;

    public BackgroundStage1()
    {

    }

    public override void Init(Transform rootTf)
    {
        base.Init(rootTf);

        _isCameraMoving = true;
        _cameraMoveTime = 0;
        _cameraMoveDuration = 150;
        _bgCamera.fieldOfView = 45;
        _bgCameraTf.localPosition = new Vector3(0, 2.8f, 0.3f);
        _preFov = _bgCamera.fieldOfView;

        InitBgBlockContainer();
    }

    private void InitBgBlockContainer()
    {
        BgBlockContainer container = CreateBgBlockContainer("Stage5BgBlock0", 4);
        container.SetSpeed(-0.008f);
        container.SetDisappearZ(-2.56f);
        container.SetIntervalRangeZ(2.56f, 2.56f);
        container.GenerateBlocks(new Vector3(0, 0, 0));

        container = CreateBgBlockContainer("Stage5BgBlock1", 4);
        container.SetSpeed(-0.008f);
        container.SetDisappearZ(-2.56f);
        container.SetIntervalRangeZ(2.56f, 2.56f);
        container.GenerateBlocks(new Vector3(0, 0, 0));
    }

    public override void Update(int curFrame)
    {
        if (curFrame % 30 == 0)
        {
            BgSpriteObject spObj = CreateBgSpriteObject("Common/MapleLeaf1");
            float posX = Random.Range(80, 150);
            float posY = Random.Range(200, 225);
            spObj.SetToPos(posX, posY);
            float scale = Random.Range(0.2f, 1);
            spObj.SetScale(new Vector3(scale, scale));
            spObj.SetVelocity(Random.Range(1f, 3f), Random.Range(-150, -30));
            spObj.SetSelfRotateAngle(new Vector3(0, 0, Random.Range(1f, 2f)));
            spObj.DoFade(Random.Range(90, 180), Random.Range(180, 300));
        }
        if (_isCameraMoving)
        {
            MoveCamera();
        }
        base.Update(curFrame);
    }

    private void MoveCamera()
    {
        _bgCameraTf.rotation = Quaternion.Euler(MathUtil.GetEaseOutQuadInterpolation(94, 30, _cameraMoveTime, _cameraMoveDuration), 0, 0);
        _bgCamera.fieldOfView = MathUtil.GetLinearInterpolation(45, 20, _cameraMoveTime, _cameraMoveDuration);
        _bgCameraTf.localPosition = MathUtil.GetEaseOutQuadInterpolation(new Vector3(0, 2.8f, 0.3f), new Vector3(0, 3.43f, -4.33f), _cameraMoveTime, _cameraMoveDuration);
        _cameraMoveTime++;
        if (_cameraMoveTime >= _cameraMoveDuration)
        {
            _isCameraMoving = false;
        }
        if (_preFov != _bgCamera.fieldOfView)
        {
            _preFov = _bgCamera.fieldOfView;
            //float scale = 2.25f / (5 * Mathf.Tan(_preFov * Mathf.Deg2Rad));
            float scale = (5 * Mathf.Tan(_preFov / 2 * Mathf.Deg2Rad)) / 2.25f;
            _foregroundLayerTf.localScale = new Vector3(scale, scale, 1);
        }
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

    public override void Clear()
    {
        base.Clear();
    }
}
