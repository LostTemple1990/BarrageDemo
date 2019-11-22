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
            _attrs.Add(nodeAttr);
            // 初始x坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosX", null);
            _attrs.Add(nodeAttr);
            // 初始有y坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosY", null);
            _attrs.Add(nodeAttr);
            // 速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Velocity", null);
            _attrs.Add(nodeAttr);
            // 角度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Angle", null);
            _attrs.Add(nodeAttr);
            // 是否朝向玩家
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Aim to player", null);
            _attrs.Add(nodeAttr);
            // 加速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Acceleration", null);
            _attrs.Add(nodeAttr);
            // 加速度角度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Acc Angle", null);
            _attrs.Add(nodeAttr);
            // 最大速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "MaxVelocity", null);
            _attrs.Add(nodeAttr);
            // 自转速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Self Rotation", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create simple bullet";
        }

        public override string ToDesc()
        {
            return string.Format("create simple bullet with id {0} at ({1},{2}) v={3},angle={4}{5}",
                _attrs[0].GetValueString(),
                _attrs[1].GetValueString(), _attrs[2].GetValueString(),
                _attrs[3].GetValueString(), _attrs[4].GetValueString(),
                _attrs[5].GetValueString() == "true" ? " aim to player " : "");
        }

        public override string ToLuaHead()
        {
            string retStr = string.Format("last = CreateSimpleBulletById({0},{1},{2})\n",
                _attrs[0].GetValueString(),
                _attrs[1].GetValueString(),
                _attrs[2].GetValueString());
            string acce = "0";
            string accAngle = "0";
            if (_attrs[6].GetValueString() != "")
            {
                acce = _attrs[6].GetValueString();
                accAngle = _attrs[7].GetValueString();
            }
            retStr = string.Format("{0}last:SetStraightParas({1},{2},{3},{4},{5})\n",
                retStr,
                _attrs[3].GetValueString(), _attrs[4].GetValueString(), _attrs[5].GetValueString(),
                acce, accAngle);
            if (_attrs[8].GetValueString() != "")
            {
                retStr = string.Format("{0}last.maxV = {1}\n",
                retStr,
                _attrs[8].GetValueString());
            }
            if (_attrs[9].GetValueString() != "")
            {
                retStr = string.Format("{0}last:SetSelfRotation({1})\n",
                retStr,
                _attrs[9].GetValueString());
            }
            return retStr;
        }
    }
}
