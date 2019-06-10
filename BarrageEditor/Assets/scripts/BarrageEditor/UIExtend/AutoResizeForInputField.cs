using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoResizeForInputField : MonoBehaviour
{
    /// <summary>
    /// 适应宽度
    /// </summary>
    public bool fitWidth;
    /// <summary>
    /// 适应高度
    /// </summary>
    public bool fitHeight;
    public float minHeight;
    public float minWidth;

    private InputField _field;

    /// <summary>
    /// 当前物体的go
    /// </summary>
    private RectTransform _rectTf;
    private Vector2 _originalSize;

    private Text _text;
    // Use this for initialization
    void Awake()
    {
        _rectTf = GetComponent<RectTransform>();
        _originalSize = _rectTf.sizeDelta;
        _field = GetComponent<InputField>();
        // 注册事件
        _field.onValueChanged.AddListener(OnTextChanged);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTextChanged(string text)
    {
        float width = fitWidth ? _field.preferredWidth+20 : _originalSize.x;
        if (minWidth > 0 && minWidth > width) width = minWidth;
        float height = fitHeight ? _field.preferredHeight+13 : _originalSize.y;
        if (minHeight > 0 && minHeight > height) height = minHeight;
        _rectTf.sizeDelta = new Vector2(width, height);
    }
}
