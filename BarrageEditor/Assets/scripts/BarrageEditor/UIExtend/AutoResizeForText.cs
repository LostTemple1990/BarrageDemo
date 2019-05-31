using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoResizeForText : MonoBehaviour
{
    /// <summary>
    /// 适应宽度
    /// </summary>
    public bool fitWidth;
    /// <summary>
    /// 适应高度
    /// </summary>
    public bool fitHeight;
    public Text text;

    /// <summary>
    /// 当前物体的go
    /// </summary>
    private RectTransform _rectTf;
    private Vector2 _originalSize;
	// Use this for initialization
	void Awake ()
    {
        _rectTf = GetComponent<RectTransform>();
        _originalSize = _rectTf.sizeDelta;
        Debug.Log(_originalSize);
        // 注册事件
        text.RegisterDirtyLayoutCallback(DirtyLayoutCallback);
	}

    private int a = 0;
	
	// Update is called once per frame
	void Update ()
    {

    }

    private void DirtyLayoutCallback()
    {
        //Debug.Log("width = " + text.preferredWidth + "    height = " + text.preferredHeight);
        float width = fitWidth ? text.preferredWidth : _originalSize.x;
        float height = fitHeight ? text.preferredHeight : _originalSize.y;
        _rectTf.sizeDelta = new Vector2(width, height);
    }
}
