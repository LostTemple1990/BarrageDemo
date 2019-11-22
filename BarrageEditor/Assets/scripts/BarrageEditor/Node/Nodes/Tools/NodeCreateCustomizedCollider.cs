using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateCustomizedCollider : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateCustomizedCollider;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "collidercreate");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // collider类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CustomizedType);
            nodeAttr.Init(this, "Type name", null);
            _attrs.Add(nodeAttr);
            // collider形状
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.ShapeType);
            nodeAttr.Init(this, "Shape", null);
            _attrs.Add(nodeAttr);
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.ParaList);
            nodeAttr.Init(this, "Parameter list", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create customized collider";
        }

        public override string ToDesc()
        {
            return string.Format("create collider in shape \"{0}\" of type \"{1}\" with parameter {2}",
                _attrs[1].GetValueString(), 
                _attrs[0].GetValueString(), 
                _attrs[2].GetValueString());
        }

        public override string ToLuaHead()
        {
            string typeName = _attrs[0].GetValueString();
            CustomDefineData data = CustomDefine.GetDataByTypeAndName(CustomDefineType.Collider, typeName);
            int paraCount = 0;
            if (data != null)
            {
                if (data.paraListStr.IndexOf(',') != -1)
                {
                    paraCount = data.paraListStr.Split(',').Length;
                }
            }
            return string.Format("last = CreateCustomizedCollider(\"{0}\",{1}{2})\n",
                typeName,
                _attrs[1].GetValueString(),
                _attrs[2].GetValueString() == "" ? "" : _attrs[4].GetValueString() + ",");
        }
    }
}
