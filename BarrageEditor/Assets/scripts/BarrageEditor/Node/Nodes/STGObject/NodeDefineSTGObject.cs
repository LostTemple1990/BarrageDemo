using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeDefineSTGObject : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.DefineSTGObject;
            _extraDepth = 0;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "objectdefine");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 定义的object类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Type name", null);
            attrs.Add(nodeAttr);
        }

        public override void CreateDefualtChilds()
        {
            // 添加OnCreate子节点
            BaseNode onCreateNode = NodeManager.CreateNode(NodeType.OnSTGObjectCreate);
            onCreateNode.SetAttrsDefaultValues();
            InsertChildNode(onCreateNode, -1);
        }

        public override string GetNodeName()
        {
            return "define object";
        }

        public override void OnAttributeValueChanged(BaseNodeAttr attr = null)
        {
            if (attr != null)
            {
                if (attr.GetPreValue() == null)
                {
                    string newTypeName = attr.GetValueString();
                    if (newTypeName != "")
                    {
                        BaseNode onCreateNode = GetChildByType(NodeType.OnSTGObjectCreate);
                        CustomDefine.AddData(CustomDefineType.Enemy, newTypeName, onCreateNode.attrs[0].GetValueString());
                    }
                }
                else
                {
                    string fromName = attr.GetPreValue().ToString();
                    if (fromName != "")
                    {
                        CustomDefine.ModifyDefineName(CustomDefineType.STGObject, fromName, attr.GetValueString());
                    }
                    else
                    {
                        string newTypeName = attr.GetValueString();
                        if (newTypeName != "")
                        {
                            BaseNode onCreateNode = GetChildByType(NodeType.OnSTGObjectCreate);
                            CustomDefine.AddData(CustomDefineType.STGObject, newTypeName, onCreateNode.attrs[0].GetValueString());
                        }
                    }
                }
            }
            else // 载入节点数据or设置节点默认值时
            {
                string typeName = GetAttrByIndex(0).GetValueString();
                if (typeName != "")
                {
                    BaseNode onCreteNode = GetChildByType(NodeType.OnSTGObjectCreate);
                    string paraList = onCreteNode.GetAttrByIndex(0).GetValueString();
                    CustomDefine.AddData(CustomDefineType.STGObject, typeName, paraList);
                }
            }
            base.OnAttributeValueChanged(attr);
        }

        public override string ToDesc()
        {
            return string.Format("define object type \"{0}\"", attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("CustomizedSTGObjectTable[\"{0}\"] = {{}}\n", attrs[0].GetValueString());
        }
    }
}
