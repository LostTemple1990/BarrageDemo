using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateDialogCG : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateDialogCG;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "createdialogcg");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 自定义标识名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Name", null);
            _attrs.Add(nodeAttr);
            // 图片名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "SpriteName", null);
            _attrs.Add(nodeAttr);
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosX", null);
            _attrs.Add(nodeAttr);
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosY", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create dialog CG";
        }

        public override string ToDesc()
        {
            return string.Format("create dialog CG with name \"{0}\" at ({1},{2})",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(2).GetValueString(), GetAttrByIndex(3).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("CreateDialogCG(\"{0}\",\"{1}\",{2},{3})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(), GetAttrByIndex(3).GetValueString());
        }
    }
}
