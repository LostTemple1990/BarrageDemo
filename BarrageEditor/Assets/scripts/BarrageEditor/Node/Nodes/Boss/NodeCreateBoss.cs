using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateBoss : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateBoss;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bosscreate");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 敌机类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CustomizedType);
            nodeAttr.Init(this, "Type name", null);
            attrs.Add(nodeAttr);
            // 起始位置X
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosX", null);
            attrs.Add(nodeAttr);
            // 起始位置Y
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosY", null);
            attrs.Add(nodeAttr);
            // 变量名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Var mame", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create boss";
        }

        public override string ToDesc()
        {
            return string.Format("create boss of type \"{0}\" at ({1},{2}),assignment to var {3}",
                attrs[0].GetValueString(),
                attrs[1].GetValueString(), attrs[2].GetValueString(),
                attrs[3].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("local {0} = CreateBoss(\"{1}\",{2},{3})\n",
                GetAttrByIndex(3).GetValueString(),
                attrs[0].GetValueString(),
                GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString());
        }
    }
}
