using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeFolder : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.Folder;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "folder");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 文件夹名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Title", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "folder";
        }

        public override string ToDesc()
        {
            return attrs[0].GetValueString();
        }
    }
}
