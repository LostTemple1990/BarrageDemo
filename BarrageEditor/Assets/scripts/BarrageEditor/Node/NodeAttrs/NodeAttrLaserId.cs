using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrLaserId : BaseNodeAttr
    {
        public override void BindItem(GameObject item)
        {
            base.BindItem(item);
            _arrowImg.gameObject.SetActive(false);

            UIEventListener.Get(_editBtnGo).AddClick(OnEditBtnClickHandler);
        }

        private void OnEditBtnClickHandler()
        {
            OpenEditView();
        }

        public override void OpenEditView()
        {
            List<object> datas = new List<object>();
            if (_node.GetNodeType() == NodeType.OnLaserCreate)
                datas.Add(BulletType.Laser);
            else if (_node.GetNodeType() == NodeType.OnLinearLaserCreate)
                datas.Add(BulletType.LinearLaser);
            else if (_node.GetNodeType() == NodeType.OnCurveLaserCreate)
                datas.Add(BulletType.CurveLaser);
            datas.Add(this);
            UIManager.GetInstance().OpenView(ViewID.AttrEditLaserIdView, datas);
        }

        public override void UnbindItem()
        {
            base.UnbindItem();
        }
    }
}
