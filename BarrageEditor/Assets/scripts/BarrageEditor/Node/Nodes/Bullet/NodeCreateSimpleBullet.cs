using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateSimpleBullet : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateSimpleBullet;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bulletcreatestraight");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // id
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.BulletId);
            nodeAttr.Init(this, "Bullet Id", null);
            attrs.Add(nodeAttr);
            // 初始x坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosX", null);
            attrs.Add(nodeAttr);
            // 初始有y坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosY", null);
            attrs.Add(nodeAttr);
            // 速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Velocity", null);
            attrs.Add(nodeAttr);
            // 角度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Angle", null);
            attrs.Add(nodeAttr);
            // 是否朝向玩家
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Aim to player", null);
            attrs.Add(nodeAttr);
            // 加速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Acceleration", null);
            attrs.Add(nodeAttr);
            // 加速度角度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Acc Angle", null);
            attrs.Add(nodeAttr);
            // 最大速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "MaxVelocity", null);
            attrs.Add(nodeAttr);
            // 自转速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Self Rotation", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create simple bullet";
        }

        public override string ToDesc()
        {
            return string.Format("create simple bullet with id {0} at ({1},{2}) v={3},angle={4}{5}",
                attrs[0].GetValueString(),
                attrs[1].GetValueString(), attrs[2].GetValueString(),
                attrs[3].GetValueString(), attrs[4].GetValueString(),
                attrs[5].GetValueString() == "true" ? " aim to player " : "");
        }

        public override string ToLuaHead()
        {
            string retStr = string.Format("last = CreateSimpleBulletById({0},{1},{2})\n",
                attrs[0].GetValueString(),
                attrs[1].GetValueString(),
                attrs[2].GetValueString());
            retStr = string.Format("{0}last:SetStraightParas({1},{2},{3},{4},{5})\n",
                retStr,
                attrs[3].GetValueString(), attrs[4].GetValueString(), attrs[5].GetValueString(),
                attrs[6].GetValueString(), attrs[7].GetValueString());
            if (attrs[8].GetValueString() != "")
            {
                retStr = string.Format("{0}last.maxV = {1}\n",
                retStr,
                attrs[8].GetValueString());
            }
            if (attrs[9].GetValueString() != "")
            {
                retStr = string.Format("{0}last:SetSelfRotation({1})\n",
                retStr,
                attrs[9].GetValueString());
            }
            return retStr;
        }
    }
}
