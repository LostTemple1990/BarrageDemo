using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrBulletId : BaseNodeAttr
    {
        public override void BindItem(GameObject item)
        {
            base.BindItem(item);
            _arrowImg.gameObject.SetActive(false);

            UIEventListener.Get(_editBtnGo).AddClick(OnEditBtnClickHandler);
        }

        private void OnEditBtnClickHandler()
        {
            UIManager.GetInstance().OpenView(ViewID.AttrEditBulletIdView, this);
        }

        public override void UnbindItem()
        {
            base.UnbindItem();
        }
    }
}
