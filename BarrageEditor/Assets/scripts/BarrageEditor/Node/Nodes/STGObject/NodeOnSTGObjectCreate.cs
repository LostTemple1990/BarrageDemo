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
            attrs.Add(nodeAttr);
        }

        public override void OnAttributeValueChanged(BaseNodeAttr attr = null)
        {
            // 手动修改引起的参数变更
            // 则更新DefineList
            if (attr != null)
            {
                // 参数列表发生变化，修改缓存
                string name = parentNode.GetAttrByIndex(0).GetValueString();
                CustomDefine.ModifyDefineParaList(CustomDefineType.STGObject, name, attr.GetValueString());
            }
            base.OnAttributeValueChanged(attr);
        }

        public override string GetNodeName()
        {
            return "on object create";
        }

        public override string ToDesc()
        {
            return string.Format("on create:({0})", attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            string name = parentNode.GetAttrByIndex(0).GetValueString();
            return string.Format("CustomizedSTGObjectTable[\"{0}\"].Init = function(self{1})\n",
                name,
                attrs[1].GetValueString() == "" ? "" : "," + attrs[1].GetValueString()  //不带参数的话self后不带任何参数了，因此不加分隔符','
                );
        }

        public override string ToLuaFoot()
        {
            return string.Format("end\n");
        }
    }
}
