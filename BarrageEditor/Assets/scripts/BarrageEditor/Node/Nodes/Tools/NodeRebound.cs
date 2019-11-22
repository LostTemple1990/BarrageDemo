using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeRebound : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.Rebound;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "rebound");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 添加碰撞触发器的对象
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 反弹参数
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.ReboundBorder);
            nodeAttr.Init(this, "ReboundPara", null);
            _attrs.Add(nodeAttr);
            // 反弹次数
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "ReboundCount", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "add rebound";
        }

        public override string ToDesc()
        {
            return string.Format("add rebound for {0}",
                _attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:AddRebound({1},{2})\n",
                _attrs[0].GetValueString(),
                _attrs[1].GetValueString(), _attrs[2].GetValueString());
        }
    }
}
