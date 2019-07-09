using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeSetBulletIgnoreCollisionGroups : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.SetBulletIgnoreCollisionGroup;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "ignorecollisiongroup");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            attrs.Add(nodeAttr);
            // 忽略的碰撞组
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CollisionGroups);
            nodeAttr.Init(this, "Ignore Groups", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "ignore collision groups";
        }

        public override string ToDesc()
        {
            return string.Format("set {0}'s ignore collision groups", GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:SetInvincible({1})\n", GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString());
        }
    }
}
