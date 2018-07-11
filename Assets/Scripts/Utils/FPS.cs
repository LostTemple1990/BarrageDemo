using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    protected float _measuringTime = 2f;

    protected int _frameCount;

    protected float _timePassed;
    protected double _curTime;
    protected double _lastTime;

    protected double _fps = -1;
	// Use this for initialization
	void Start () {
        _frameCount = 0;
        _curTime = 0;
        _lastTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
        _frameCount++;
        _curTime = 0.0001d * System.DateTime.Now.Ticks;
        if ( _lastTime == 0 )
        {
            _lastTime = 0.0001d * System.DateTime.Now.Ticks;
        }
        if ( _curTime - _lastTime > 2000 )
        {
            _fps = _frameCount * 1000d / (_curTime-_lastTime);
            _frameCount = 0;
            _curTime = 0;
            _lastTime = 0;
        }
	}

    private void OnGUI()
    {
        GUIStyle bb = new GUIStyle();
        bb.normal.background = null;    //这是设置背景填充的
        bb.normal.textColor = new Color(1.0f, 0.5f, 0.0f);   //设置字体颜色的
        bb.fontSize = 40;       //当然，这是字体大小

        //居中显示FPS
        GUI.Label(new Rect((Screen.width / 2) - 40, 0, 200, 200), "FPS: " + _fps, bb);
    }
}
