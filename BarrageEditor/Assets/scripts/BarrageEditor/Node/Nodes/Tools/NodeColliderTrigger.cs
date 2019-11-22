using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeColliderTrigger : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.ColliderTrigger;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "collidertrigger");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 添加碰撞触发器的对象
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 检测碰撞的类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CollisionGroups);
            nodeAttr.Init(this, "CollisionGroup", null);
            _attrs.Add(nodeAttr);
        }

        public override void CreateDefualtChilds()
        {
            // comment
            BaseNode commentNode = NodeManager.CreateNode(NodeType.Comment);
            commentNode.GetAttrByIndex(0).SetValue("input trigger event here");
            InsertChildNode(commentNode, -1);
        }

        public override string GetNodeName()
        {
            return "collider trigger";
        }

        public override string ToDesc()
        {
            return string.Format("add collider trigger for {0}",
                _attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:AddColliderTrigger({1},function(collider,collIndex)\n",
                _attrs[0].GetValueString(),
                _attrs[1].GetValueString());
        }

        public override string ToLuaFoot()
        {
            return string.Format("end)\n");
        }
    }
}
