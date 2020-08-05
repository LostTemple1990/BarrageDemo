using UnityEngine;
using System.Collections;

public class ViewBase
{
    protected GameObject _view;
    protected RectTransform _viewTf;
    protected bool _isShow;

    public virtual void Init(GameObject viewObj)
    {
        _view = viewObj;
        _viewTf = _view.GetComponent<RectTransform>();
        _isShow = false;
        UIManager.GetInstance().AddGoToLayer(viewObj, GetLayerId());
    }

    //public virtual void SetViewGO(GameObject viewObj)
    //{
    //    _view = viewObj;
    //    _viewTf = _view.transform;
    //    UIManager.GetInstance().AddGoToLayer(viewObj, GetLayerId());
    //}

    public void Show(object data)
    {
        if ( _view != null )
        {
            _isShow = true;
            _view.SetActive(true);
            Adaptive();
            OnShow(data);
        }
    }

    public virtual void Update()
    {

    }

    protected virtual void OnShow(object data)
    {

    }

    public void Hide()
    {
        if (!_isShow)
            return;
        OnHide();
        _isShow = false;
        UIManager.GetInstance().UnregisterViewUpdate(this);
        _view.SetActive(false);
    }

    protected virtual void OnHide()
    {
        
    }

    public void Destroy()
    {
        Hide();
        OnDestroy();
    }

    protected virtual void OnDestroy()
    {
        GameObject.Destroy(_view);
        _view = null;
        _viewTf = null;
    }

    /// <summary>
    /// 适配
    /// </summary>
    public virtual void Adaptive()
    {

    }

    public virtual LayerId GetLayerId()
    {
        return LayerId.UI;
    }
}
