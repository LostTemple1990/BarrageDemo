using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateDialogBox : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateSentence;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "sentence");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 对话框类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Style", null);
            _attrs.Add(nodeAttr);
            // 对话框文本
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Text", null);
            _attrs.Add(nodeAttr);
            // 位置
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosX", null);
            _attrs.Add(nodeAttr);
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosY", null);
            _attrs.Add(nodeAttr);
            // 持续时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Duration", null);
            _attrs.Add(nodeAttr);
            // 高度缩放
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Scale", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create dialog box";
        }

        public override string ToDesc()
        {
            return string.Format("say \"{0}\" at ({1},{2}) during {3} frame(s)",
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(), GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("CreateDialogBox({0},\"{1}\",{2},{3},{4},{5})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(), GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString(),
                GetAttrByIndex(5).GetValueString());
        }
    }
}
