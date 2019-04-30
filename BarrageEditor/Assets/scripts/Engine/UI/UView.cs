using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YKEngine
{
    public class UView : Graphic
    {
        public bool focusWhenOpen;

        protected bool _isFocus;

        protected ViewBase _viewBase;

        public UView()
           : base()
        {
            _isFocus = false;
        }

        protected override void Start()
        {
            base.Start();
            color = new Color(1, 1, 1, 0);
        }

        internal void Init(ViewBase viewBase)
        {
            _viewBase = viewBase;
        }

        /// <summary>
        /// 当前是否为焦点窗体
        /// </summary>
        public bool Focus
        {
            get { return _isFocus; }
            internal set { _isFocus = value; }
        }

        public ViewBase GetView()
        {
            return _viewBase;
        }
    }
}
