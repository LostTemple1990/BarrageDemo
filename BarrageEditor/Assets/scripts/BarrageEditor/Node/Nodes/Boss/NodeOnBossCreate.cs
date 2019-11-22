using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeOnBossCreate : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.OnBossCreate;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bossinit");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // Boss动画id
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Ani Id", null);
            _attrs.Add(nodeAttr);
            // 碰撞半径
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Collision Size", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "on boss init";
        }

        public override string ToDesc()
        {
            return string.Format("init()");
        }

        public override string ToLuaHead()
        {
            string name = parentNode.GetAttrByIndex(0).GetValueString();
            string ret = string.Format("BossTable[\"{0}\"].Init = function(self)\n",name);
            ret += string.Format("    self:SetAni({0})\n", GetAttrByIndex(0).GetValueString());
            //ret += string.Format("    self:SetPos({0},{1})\n", GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString());
            ret += string.Format("    self:SetCollisionSize({0})\n", GetAttrByIndex(1).GetValueString());
            return ret;
        }

        public override string ToLuaFoot()
        {
            return string.Format("end\n");
        }
    }
}
