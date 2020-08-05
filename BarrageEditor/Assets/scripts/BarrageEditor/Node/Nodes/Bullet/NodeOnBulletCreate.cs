using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeOnBulletCreate : BaseNode
    {
        /// <summary>
        /// 参数列表的属性的索引
        /// </summary>
        public const int ParamListAttrIndex = 0;

        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.OnBulletCreate;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bulletinit");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Parameter list", null);
            _attrs.Add(nodeAttr);
            // id
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.BulletId);
            nodeAttr.Init(this, "Bullet Id", null);
            _attrs.Add(nodeAttr);
        }

        public override void OnAttributeValueChanged(BaseNodeAttr attr = null)
        {
            // 手动修改引起的参数变更
            // 则更新DefineList
            if ( attr != null && attr == GetAttrByIndex(ParamListAttrIndex) )
            {
                if ((_parentNode as NodeDefineBullet).IsWatchingData)
                {
                    // 参数列表发生变化，修改缓存
                    string name = _parentNode.GetAttrByIndex(ParamListAttrIndex).GetValueString();
                    CustomDefine.ModifyDefineParaList(CustomDefineType.SimpleBullet, name, attr.GetValueString());
                }
            }
            base.OnAttributeValueChanged(attr);
        }

        public override string GetNodeName()
        {
            return "on bullet create";
        }

        public override string ToDesc()
        {
            return string.Format("on create:({0})", _attrs[ParamListAttrIndex].GetValueString());
        }

        public override string ToLuaHead()
        {
            string name = _parentNode.GetAttrs()[0].GetValueString();
            return string.Format("CustomizedTable[\"{0}\"].Init = function(self{1})\n    self:SetStyleById({2})\n",
                name,
                _attrs[ParamListAttrIndex].GetValueString() == "" ? "" : "," + _attrs[ParamListAttrIndex].GetValueString(),
                GetAttrByIndex(1).GetValueString()
                );
        }

        public override string ToLuaFoot()
        {
            return string.Format("end\n");
        }
    }
}
