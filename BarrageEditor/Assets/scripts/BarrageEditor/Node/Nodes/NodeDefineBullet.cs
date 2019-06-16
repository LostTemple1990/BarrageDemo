using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeDefineBullet : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.DefineBullet;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bulletinit");
            isExpand = true;
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 定义的子弹类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Type name", null);
            attrs.Add(nodeAttr);
        }

        public override void CreateDefualtChilds()
        {
            // 添加OnCreate子节点
            BaseNode onCreateNode = NodeManager.CreateNode(NodeType.OnBulletCreate);
            onCreateNode.SetAttrsDefaultValues();
            InsertChildNode(onCreateNode, -1);
        }

        public override string GetNodeName()
        {
            return "define bullet";
        }

        public override void OnAttributeValueChanged(BaseNodeAttr attr=null)
        {
            if ( attr != null )
            {
                if ( attr.GetPreValue() == null )
                {
                    string newTypeName = attr.GetValueString();
                    if (newTypeName != "")
                    {
                        BaseNode onCreteNode = GetChildByType(NodeType.OnBulletCreate);
                        CustomDefine.AddData(CustomDefineType.SimpleBullet, newTypeName, onCreteNode.attrs[0].GetValueString());
                    }
                }
                else
                {
                    string fromName = attr.GetPreValue().ToString();
                    if ( fromName != "" )
                    {
                        CustomDefine.ModifyDefineName(CustomDefineType.SimpleBullet, fromName, attr.GetValueString());
                    }
                    else
                    {
                        string newTypeName = attr.GetValueString();
                        if (newTypeName != "")
                        {
                            BaseNode onCreteNode = GetChildByType(NodeType.OnBulletCreate);
                            CustomDefine.AddData(CustomDefineType.SimpleBullet, newTypeName, onCreteNode.attrs[0].GetValueString());
                        }
                    }
                }
            }
            base.OnAttributeValueChanged(attr);
        }

        public override string ToDesc()
        {
            return string.Format("define bullet type \"{0}\"", attrs[0].GetValueString());
        }
    }
}
