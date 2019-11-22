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
            _extraDepth = 0;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "folder");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 文件夹名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Title", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "folder";
        }

        public override string ToDesc()
        {
            return _attrs[0].GetValueString();
        }
    }
}
