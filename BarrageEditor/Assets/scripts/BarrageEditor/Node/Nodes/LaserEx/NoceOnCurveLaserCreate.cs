using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeOnCurveLaserCreate : BaseNode
    {
        /// <summary>
        /// 参数列表的属性的索引
        /// </summary>
        public const int ParamListAttrIndex = 0;

        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.OnCurveLaserCreate;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "laserbentinit");
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
            // width
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Width", null);
            _attrs.Add(nodeAttr);
        }

        public override void OnAttributeValueChanged(BaseNodeAttr attr = null)
        {
            // 手动修改引起的参数变更
            // 则更新DefineList
            if (attr != null && attr == GetAttrByIndex(ParamListAttrIndex))
            {
                if ((parentNode as NodeDefineCurveLaser).IsWatchingData)
                {
                    // 参数列表发生变化，修改缓存
                    string name = parentNode.GetAttrByIndex(ParamListAttrIndex).GetValueString();
                    CustomDefine.ModifyDefineParaList(CustomDefineType.CurveLaser, name, attr.GetValueString());
                }
            }
            base.OnAttributeValueChanged(attr);
        }

        public override string GetNodeName()
        {
            return "on curve laser create";
        }

        public override string ToDesc()
        {
            return string.Format("on create:({0})", _attrs[ParamListAttrIndex].GetValueString());
        }

        public override string ToLuaHead()
        {
            string name = parentNode.GetAttrs()[0].GetValueString();
            string ret = string.Format("CustomizedTable[\"{0}\"].Init = function(self{1})\n",
                name,
                _attrs[ParamListAttrIndex].GetValueString() == "" ? "" : "," + _attrs[ParamListAttrIndex].GetValueString()
                );
            ret += string.Format("    self:SetStyleById({0})\n    self:SetLength({1})\n    self:SetWidth({2})\n",
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString()
                );
            return ret;
        }

        public override string ToLuaFoot()
        {
            return string.Format("end\n");
        }
    }
}
