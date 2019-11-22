using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrCollisionGroup : NodeAttrUneditableDropdown
    {
        public override void BindItem(RectTransform parentTf)
        {
            base.BindItem(parentTf);

            UIEventListener.Get(_editBtnGo).AddClick(OnEditBtnClickHandler);
        }

        private void OnEditBtnClickHandler()
        {
            OpenEditView();
        }

        public override void OpenEditView()
        {
            UIManager.GetInstance().OpenView(ViewID.AttrEditCollisionGroup, this);
        }

        public override void UnbindItem()
        {
            if (_itemGo == null)
                return;
            base.UnbindItem();
        }
    }
}
