using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateSimpleCollider : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateSimpleCollider;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "simplecollider");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 形状
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.ShapeType);
            nodeAttr.Init(this, "Shape", null);
            _attrs.Add(nodeAttr);
            // 初始x坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosX", null);
            _attrs.Add(nodeAttr);
            // 初始有y坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosY", null);
            _attrs.Add(nodeAttr);
            // 宽度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Size x", null);
            _attrs.Add(nodeAttr);
            // 高度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Size y", null);
            _attrs.Add(nodeAttr);
            // CollisionGroup 碰撞组
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CollisionGroups);
            nodeAttr.Init(this, "Collision Group", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create simple collider";
        }

        public override string ToDesc()
        {
            return string.Format("create simple collider in shape {0} at ({1},{2}) with size({3},{4})",
                _attrs[0].GetValueString(),
                _attrs[1].GetValueString(), _attrs[2].GetValueString(),
                _attrs[3].GetValueString(), _attrs[4].GetValueString());
        }

        public override string ToLuaHead()
        {
            string retStr = string.Format("last = CreateSimpleCollider({0})\n", GetAttrByIndex(0).GetValueString());
            retStr = string.Format("{0}last:SetPos({1},{2})\n",retStr,_attrs[1].GetValueString(), _attrs[2].GetValueString());
            retStr = string.Format("{0}last:SetSize({1},{2})\n",retStr,_attrs[3].GetValueString(), _attrs[4].GetValueString());
            retStr = string.Format("{0}last:SetCollisionGroup({1})\n", retStr, GetAttrByIndex(5).GetValueString());
            return retStr;
        }
    }
}
