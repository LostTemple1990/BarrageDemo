using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgCameraDistortEffect : MonoBehaviour ,ICommand
{

    private Material _distortMat;
	// Use this for initialization
	void Start () 
    {
        CommandManager.GetInstance().Register(CommandConsts.CreateBgDistortEffect, this);
        CommandManager.GetInstance().Register(CommandConsts.UpdateBgDistortEffectProps, this);
        CommandManager.GetInstance().Register(CommandConsts.RemoveBgDistortEffect, this);
	}
	
	// Update is called once per frame
	void Update () 
    {
		
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

    public void Execute(int cmd, object[] datas)
    {
        switch ( cmd )
        {
            case CommandConsts.UpdateBgDistortEffectProps:
                UpdateDistortMatProps(datas);
                break;
            case CommandConsts.RemoveBgDistortEffect:
                ClearDistortMat();
                break;
        }
    }

    private void UpdateDistortMatProps(object[] datas)
    {
        if ( _distortMat == null )
        {
            _distortMat = Resources.Load<Material>("Materials/DistortMat");
        }
        float circleCenterX = (float)datas[0];
        float circleCenterY = (float)datas[1];
        float widthRatio = (float)datas[2];
        float heightRatio = (float)datas[3];
        float distortFactor = (float)datas[4];
        _distortMat.SetFloat("_CircleCenterX", circleCenterX);
        _distortMat.SetFloat("_CircleCenterY", circleCenterY);
        _distortMat.SetFloat("_CircleRadiusRatioWithWidth", widthRatio);
        _distortMat.SetFloat("_CircleRadiusRatioWithHeight", heightRatio);
        _distortMat.SetFloat("_DistortFactor", distortFactor);
    }

    private void ClearDistortMat()
    {
        GameObject.Destroy(_distortMat);
        _distortMat = null;
    }
}
