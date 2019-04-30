using UnityEngine;
using System.Collections;

namespace YKEngine
{
    public class ViewBase
    {
        protected GameObject _view;
        protected Transform _viewTf;
        protected int _viewId;

        public virtual void Init(GameObject viewObj)
        {
            _view = viewObj;
            _viewTf = _view.transform;
            //UIManager.GetInstance().AddGoToLayer(viewObj, GetLayerId());
        }

        //public virtual void SetViewGO(GameObject viewObj)
        //{
        //    _view = viewObj;
        //    _viewTf = _view.transform;
        //    UIManager.GetInstance().AddGoToLayer(viewObj, GetLayerId());
        //}

        public void OnShowImpl(object data)
        {
            if (_view != null)
            {
                _view.SetActive(true);
                OnShow(data);
            }
        }

        public virtual void Update()
        {

        }

        public virtual void OnShow(object data)
        {

        }

        public virtual void Refresh(object data)
        {

        }

        public void Close()
        {
            UIManager.GetInstance().CloseView(_viewId);
        }

        public void OnCloseImpl()
        {
            OnClose();
            UIManager.GetInstance().UnregisterViewUpdate(this);
            UIEventListener.RemoveAllListeners(_view);
            GameObject.Destroy(_view);
            _view = null;
            _viewTf = null;
        }

        public virtual void OnClose()
        {

        }

        /// <summary>
        /// 适配
        /// </summary>
        public virtual void Adaptive()
        {

        }
    }
}
