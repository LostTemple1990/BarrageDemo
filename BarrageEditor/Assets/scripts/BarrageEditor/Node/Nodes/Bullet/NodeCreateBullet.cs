using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateBullet : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateCustomizedBullet;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bulletcreate");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 子弹类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CustomizedType);
            nodeAttr.Init(this, "Type name", null);
            attrs.Add(nodeAttr);
            // 初始x坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosX", null);
            attrs.Add(nodeAttr);
            // 初始有y坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosY", null);
            attrs.Add(nodeAttr);
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.ParaList);
            nodeAttr.Init(this, "Parameter list", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create bullet";
        }

        public override string ToDesc()
        {
            return string.Format("create bullet of type\"{0}\" at ({1},{2}) with parameter {3}",
                attrs[0].GetValueString(), attrs[1].GetValueString(), attrs[2].GetValueString(), attrs[3].GetValueString());
        }

        public override string ToLuaHead()
        {
            string typeName = attrs[0].GetValueString();
            CustomDefineData data = CustomDefine.GetDataByTypeAndName(CustomDefineType.SimpleBullet, typeName);
            return string.Format("last = CreateCustomizedBullet(\"{0}\",{1},{2}{3})\n",
                typeName,
                attrs[1].GetValueString(),
                attrs[2].GetValueString(),
                attrs[3].GetValueString() == "" ? "" : "," + attrs[3].GetValueString());
        }
    }
}
