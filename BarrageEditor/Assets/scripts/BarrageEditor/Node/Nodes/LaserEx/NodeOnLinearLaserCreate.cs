using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeOnLinearLaserCreate : BaseNode
    {
        /// <summary>
        /// 参数列表的属性的索引
        /// </summary>
        public const int ParamListAttrIndex = 0;

        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.OnLinearLaserCreate;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "laserinit");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Parameter list", null);
            _attrs.Add(nodeAttr);
            // laserId
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.LaserId);
            nodeAttr.Init(this, "Laser Id", null);
            _attrs.Add(nodeAttr);
            // length
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Length", null);
            _attrs.Add(nodeAttr);
            // SourceEnable
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Show Source", null);
            _attrs.Add(nodeAttr);
            // HeadEnable
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Show Head", null);
            _attrs.Add(nodeAttr);
        }

        public override void OnAttributeValueChanged(BaseNodeAttr attr = null)
        {
            // 手动修改引起的参数变更
            // 则更新DefineList
            if (attr != null && attr == GetAttrByIndex(ParamListAttrIndex))
            {
                if ((_parent as NodeDefineLinearLaser).IsWatchingData)
                {
                    // 参数列表发生变化，修改缓存
                    string name = _parent.GetAttrByIndex(ParamListAttrIndex).GetValueString();
                    CustomDefine.ModifyDefineParaList(CustomDefineType.LinearLaser, name, attr.GetValueString());
                }
            }
            base.OnAttributeValueChanged(attr);
        }

        public override string GetNodeName()
        {
            return "on linear laser create";
        }

        public override string ToDesc()
        {
            return string.Format("on create:({0})", _attrs[ParamListAttrIndex].GetValueString());
        }

        public override string ToLuaHead()
        {
            string name = _parent.GetAttrs()[0].GetValueString();
            string ret = string.Format("CustomizedTable[\"{0}\"].Init = function(self{1})\n",
                name,
                _attrs[ParamListAttrIndex].GetValueString() == "" ? "" : "," + _attrs[ParamListAttrIndex].GetValueString()
                );
            ret += string.Format("    self:SetStyleById({0})\n    self:SetLength({1})\n    self:SetSourceEnable({2})\n    self:SetHeadEnable({3})\n",
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString()
                );
            return ret;
        }

        public override string ToLuaFoot()
        {
            return string.Format("end\n");
        }
    }
}
