using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeOnSTGObjectCreate : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.OnSTGObjectCreate;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "objectinit");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Parameter list", null);
            _attrs.Add(nodeAttr);
            // 图集名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Atlas name", null);
            _attrs.Add(nodeAttr);
            // 精灵名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Sprite name", null);
            _attrs.Add(nodeAttr);
            // 混合模式
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.BlendMode);
            nodeAttr.Init(this, "Blend mode", null);
            _attrs.Add(nodeAttr);
            // 层级
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Layer);
            nodeAttr.Init(this, "Layer", null);
            _attrs.Add(nodeAttr);
            // 是否缓存
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Cached", null);
            _attrs.Add(nodeAttr);
        }

        public override void OnAttributeValueChanged(BaseNodeAttr attr = null)
        {
            // 手动修改引起的参数变更
            // 则更新DefineList
            if (attr != null && attr == GetAttrByIndex(0))
            {
                if ((parentNode as NodeDefineSTGObject).IsWatchingData)
                {
                    // 参数列表发生变化，修改缓存
                    string name = parentNode.GetAttrByIndex(0).GetValueString();
                    CustomDefine.ModifyDefineParaList(CustomDefineType.STGObject, name, attr.GetValueString());
                }
            }
            base.OnAttributeValueChanged(attr);
        }

        public override string GetNodeName()
        {
            return "on object create";
        }

        public override string ToDesc()
        {
            return string.Format("on create:({0})", _attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            string name = parentNode.GetAttrByIndex(0).GetValueString();
            string ret = string.Format("CustomizedSTGObjectTable[\"{0}\"].Init = function(self{1})\n",
                name,
                _attrs[0].GetValueString() == "" ? "" : "," + _attrs[0].GetValueString()  //不带参数的话self后不带任何参数了，因此不加分隔符','
                );
            ret = string.Format("{0}    {1}:SetSprite(\"{2}\",\"{3}\",{4},{5},{6})\n",
                ret,
                "self",
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString(),
                GetAttrByIndex(5).GetValueString());
            return ret;
        }

        public override string ToLuaFoot()
        {
            return string.Format("end\n");
        }
    }
}
