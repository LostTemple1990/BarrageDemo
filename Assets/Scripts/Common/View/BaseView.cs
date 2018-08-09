using UnityEngine;
using System.Collections;

public class ViewBase
{
    protected GameObject _view;
    protected Transform _viewTf;

    public virtual void Init(GameObject viewObj)
    {
        _view = viewObj;
        _viewTf = _view.transform;
        UIManager.GetInstance().AddGoToLayer(viewObj, GetLayerId());
    }

    //public virtual void SetViewGO(GameObject viewObj)
    //{
    //    _view = viewObj;
    //    _viewTf = _view.transform;
    //    UIManager.GetInstance().AddGoToLayer(viewObj, GetLayerId());
    //}

    public void Show(object[] data)
    {
        if ( _view != null )
        {
            _view.SetActive(true);
            OnShow(data);
        }
    }

    public virtual void Update()
    {

    }

    public virtual void OnShow(object[] data)
    {

    }

    public void Hide()
    {
        OnHide();
        _view.SetActive(false);
    }

    public virtual void OnHide()
    {

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
