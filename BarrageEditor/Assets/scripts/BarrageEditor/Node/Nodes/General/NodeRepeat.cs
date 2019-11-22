using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeRepeat : BaseNode
    {
        private const int VarCount = 3;

        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.Repeat;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "repeat");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 次数
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Number of times", null);
            _attrs.Add(nodeAttr);
            // 变量1
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Var 1 name", null);
            _attrs.Add(nodeAttr);
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Var 1 init value", null);
            _attrs.Add(nodeAttr);
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Var 1 increment", null);
            _attrs.Add(nodeAttr);
            // 变量2
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Var 2 name", null);
            _attrs.Add(nodeAttr);
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Var 2 init value", null);
            _attrs.Add(nodeAttr);
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Var 2 increment", null);
            _attrs.Add(nodeAttr);
            // 变量3
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Var 3 name", null);
            _attrs.Add(nodeAttr);
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Var 3 init value", null);
            _attrs.Add(nodeAttr);
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Var 3 increment", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "repeat";
        }

        public override string ToDesc()
        {
            int endIndex = 1 + VarCount * 3;
            BaseNodeAttr nodeAttr = GetAttrByIndex(0);
            string ret = string.Format("repeat {0} times", nodeAttr.GetValueString());
            for (int i = 1; i < endIndex; i = i + 3)
            {
                nodeAttr = GetAttrByIndex(i);
                if ( nodeAttr.GetValueString() != "" )
                {
                    ret = string.Format("{0} ({1}={2},increment {3})", 
                        ret, GetAttrByIndex(i).GetValueString(), GetAttrByIndex(i+1).GetValueString(), GetAttrByIndex(i+2).GetValueString());
                }
            }
            return ret;
        }

        public override string ToLuaHead()
        {
            int endIndex = 1 + VarCount * 3;
            BaseNodeAttr nodeAttr;
            string ret = "do";
            for (int i = 1; i < endIndex; i = i + 3)
            {
                nodeAttr = GetAttrByIndex(i);
                if (nodeAttr.GetValueString() != "")
                {
                    ret = string.Format("{0} local {1},{2}=({3}),({4})",
                        ret,
                        GetAttrByIndex(i).GetValueString(), "_d_" + GetAttrByIndex(i).GetValueString(),
                        GetAttrByIndex(i + 1).GetValueString(), GetAttrByIndex(i + 2).GetValueString());
                }
            }
            ret = string.Format("{0} for _=1,{1} do\n", ret, GetAttrByIndex(0).GetValueString());
            return ret;
        }

        public override string ToLuaFoot()
        {
            int endIndex = 1 + VarCount * 3;
            BaseNodeAttr nodeAttr;
            string ret = "";
            for (int i = 1; i < endIndex; i = i + 3)
            {
                nodeAttr = GetAttrByIndex(i);
                if (nodeAttr.GetValueString() != "")
                {
                    ret = string.Format("{0}{1}={2}+{3} ",
                        ret,
                        GetAttrByIndex(i).GetValueString(),
                        GetAttrByIndex(i).GetValueString(), "_d_" + GetAttrByIndex(i).GetValueString());
                }
            }
            ret = string.Format("{0}end end\n", ret);
            return ret;
        }
    }
}
