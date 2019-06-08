using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollInputField : MonoBehaviour
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

    private Scrollbar _horizonScrollBar;
    private Scrollbar _verticalScrollBar;

    public InputField inputField;

    /// <summary>
    /// 当前物体的go
    /// </summary>
    private RectTransform _inputFieldTf;

    private Vector2 _originalSize;
    private Vector2 _preSize;

    private Vector2 _textComponentSizeDelta;

    private bool _isExpandingWidth;
    private bool _isExpandingHeight;

    // Use this for initialization
    void Awake()
    {
        if (inputField == null) return;
        // 滑块
        ScrollRect sr = GetComponent<ScrollRect>();
        _horizonScrollBar = sr.horizontalScrollbar;
        _verticalScrollBar = sr.verticalScrollbar;
        // 输入框
        _inputFieldTf = inputField.GetComponent<RectTransform>();
        _originalSize = _inputFieldTf.sizeDelta;
        // 注册事件
        inputField.onValueChanged.AddListener(OnTextChanged);
        _textComponentSizeDelta = inputField.textComponent.GetComponent<RectTransform>().sizeDelta;
    }

    private void OnTextChanged(string text)
    {
        _preSize = _inputFieldTf.sizeDelta;
        float width = fitWidth ? inputField.preferredWidth -_textComponentSizeDelta.x : _originalSize.x;
        if (minWidth > 0 && minWidth > width) width = minWidth;
        float height = fitHeight ? inputField.preferredHeight -_textComponentSizeDelta.y : _originalSize.y;
        if (minHeight > 0 && minHeight > height) height = minHeight;
        _inputFieldTf.sizeDelta = new Vector2(width, height);
        // 滑动条的位置
        if ( _horizonScrollBar != null && width > _preSize.x )
        {
            StartCoroutine(SetWidthValue());
        }
        if ( _verticalScrollBar != null && height > _preSize.y )
        {
            StartCoroutine(SetHeightValue());
        }
    }

    private IEnumerator SetWidthValue()
    {
        yield return new WaitForEndOfFrame();
        _horizonScrollBar.value = 1;
    }

    private IEnumerator SetHeightValue()
    {
        yield return new WaitForEndOfFrame();
        _verticalScrollBar.value = 0;
    }
}
