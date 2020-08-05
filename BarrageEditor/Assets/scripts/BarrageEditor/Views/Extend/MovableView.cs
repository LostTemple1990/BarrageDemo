using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YKEngine;

public class MovableView : ViewBase
{
    private GameObject _titleGo;
    private RectTransform _windowTf;
    private Vector2 _dragPos;
    /// <summary>
    /// 窗体初始位置
    /// </summary>
    private Vector2 _originalPos;
    /// <summary>
    /// 窗体一半的大小，用于防止窗体被拖拽出屏幕外
    /// </summary>
    private Vector2 _halfSize;
    private Vector2 _uiRootSize;

    protected override void Init()
    {
        _titleGo = GetTitleGo();
        GameObject windowGo = GetWindowGo();
        _windowTf = windowGo.GetComponent<RectTransform>();
        AddDragEvent();
    }

    private void AddDragEvent()
    {
        if (_titleGo == null)
            return;
        if (_windowTf == null)
            return;
        _uiRootSize = UIManager.GetInstance().GetUIRootSize();
        _originalPos = UIManager.GetInstance().GetUIPosition(_windowTf);
        _halfSize = _windowTf.sizeDelta / 2;
        UIEventListener.Get(_titleGo).AddDragBegin(OnDragBegin);
        UIEventListener.Get(_titleGo).AddDrag(OnDrag);
    }

    private void OnDragBegin()
    {
        _dragPos = UIManager.GetInstance().GetUIPosition(_windowTf);
    }

    private void OnDrag(Vector2 delta)
    {
        Vector2 newPos = _dragPos + delta;
        // check if out of border after drag
        if (newPos.x + _halfSize.x > _uiRootSize.x ||
            newPos.y + _halfSize.y > _uiRootSize.y ||
            newPos.x - _halfSize.x < 0 ||
            newPos.y - _halfSize.y < 0
            )
            return;
        _dragPos = newPos;
        UIManager.GetInstance().SetUIPosition(_windowTf, newPos);
    }

    public override void OnClose()
    {
        if (_titleGo != null && _windowTf != null)
        {
            UIEventListener.Get(_titleGo).RemoveAllEvents();
        }
        _titleGo = null;
        _windowTf = null;
        base.OnClose();
    }

    protected virtual GameObject GetTitleGo()
    {
        throw new System.Exception("You should implement method \"GetTitleGo\" first");
    }

    protected virtual GameObject GetWindowGo()
    {
        throw new System.Exception("You should implement method \"GetWindowGo\" first");
    }
}
