using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateSimpleEnemy : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateSimpleEnemy;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "enemysimple");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // id
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.EnemyStyle);
            nodeAttr.Init(this, "Enemy Id", null);
            attrs.Add(nodeAttr);
            // hp
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Hit point", null);
            attrs.Add(nodeAttr);
            // 初始x坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosX", null);
            attrs.Add(nodeAttr);
            // 初始有y坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosY", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create simple enemy";
        }

        public override string ToDesc()
        {
            return string.Format("create enemy with id {0} at ({1},{2})",
                attrs[0].GetValueString(), 
                attrs[2].GetValueString(), attrs[3].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("last = CreateNormalEnemyById({0},{1},{2},{3})\n",
                attrs[0].GetValueString(),
                attrs[1].GetValueString(),
                attrs[2].GetValueString(),
                attrs[3].GetValueString());
        }
    }
}
