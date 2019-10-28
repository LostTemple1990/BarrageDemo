using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgCameraDistortEffect : MonoBehaviour ,ICommand
{

    private Material _distortMat;
	// Use this for initialization
    private bool _isInit;
	void Start () 
    {
        _isInit = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
		if ( !_isInit )
        {
            Init();
        }
	}

    private void Init()
    {
        CommandManager.GetInstance().Register(CommandConsts.CreateBgDistortEffect, this);
        CommandManager.GetInstance().Register(CommandConsts.UpdateBgDistortEffectProps, this);
        CommandManager.GetInstance().Register(CommandConsts.RemoveBgDistortEffect, this);
        _isInit = true;
    }

    void OnRenderImage(RenderTexture source,RenderTexture dest)
    {
        if ( _distortMat != null )
        {
            Graphics.Blit(source, dest, _distortMat);
        }
        else
        {
            Graphics.Blit(source, dest);
        }
    }

    public void Execute(int cmd, object data)
    {
        switch ( cmd )
        {
            case CommandConsts.UpdateBgDistortEffectProps:
                UpdateDistortMatProps(data);
                break;
            case CommandConsts.RemoveBgDistortEffect:
                ClearDistortMat();
                break;
        }
    }

    private void UpdateDistortMatProps(object data)
    {
        object[] datas = data as object[];
        if ( _distortMat == null )
        {
            _distortMat = Resources.Load<Material>("Materials/DistortMat");
        }
        float circleCenterX = (float)datas[0];
        float circleCenterY = (float)datas[1];
        // 坐标换算
        circleCenterX += Consts.GameWidth / 2;
        circleCenterY += Consts.GameHeight / 2;
        float radius = (float)datas[2];
        float distortFactor = (float)datas[3];
        Color effectColor = (Color)datas[4];
        _distortMat.SetFloat("_CircleCenterX", circleCenterX);
        _distortMat.SetFloat("_CircleCenterY", circleCenterY);
        _distortMat.SetFloat("_CircleRadius", radius);
        _distortMat.SetFloat("_DistortFactor", distortFactor);
        _distortMat.SetColor("_EffectColor", effectColor);
        // 设置游戏默认宽高
        _distortMat.SetFloat("_STGWidth", Consts.GameWidth);
        _distortMat.SetFloat("_STGHeight", Consts.GameHeight);
    }

    private void ClearDistortMat()
    {
        GameObject.Destroy(_distortMat);
        _distortMat = null;
    }
}