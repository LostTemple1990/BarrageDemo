using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeChangeBulletProperty : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.ChangeBulletProperty;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bulletchangeprops");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 参数
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.PropertyType);
            nodeAttr.Init(this, "Property type", null);
            _attrs.Add(nodeAttr);
            // 改变方式
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.PropertyChangeMode);
            nodeAttr.Init(this, "Change mode", null);
            _attrs.Add(nodeAttr);
            // 参数类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Value type", null);
            _attrs.Add(nodeAttr);
            // 参数0
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Arg0", null);
            _attrs.Add(nodeAttr);
            // 参数1
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Arg1", null);
            _attrs.Add(nodeAttr);
            // 随机偏移量
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Value offset", null);
            _attrs.Add(nodeAttr);
            // 起始延迟
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Delay", null);
            _attrs.Add(nodeAttr);
            // 持续时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Duration", null);
            _attrs.Add(nodeAttr);
            // 插值方式
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.MoveMode);
            nodeAttr.Init(this, "Interpolation mode", null);
            _attrs.Add(nodeAttr);
            // 执行次数
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Repeat count", null);
            _attrs.Add(nodeAttr);
            // 重复时间间隔
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Repeat interval(frame)", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "change property";
        }

        public override string ToDesc()
        {
            return string.Format("change property {0} of {1} by mode {2} during {3} frame(s) with {4} frame(s) delay",
                _attrs[1].GetValueString(),
                _attrs[0].GetValueString(), 
                _attrs[2].GetValueString(),
                _attrs[8].GetValueString(), _attrs[7].GetValueString());
        }

        public override string ToLuaHead()
        {
            string retStr = string.Format("{0}:ChangeProperty({1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11})\n",
                _attrs[0].GetValueString(),
                _attrs[1].GetValueString(),
                _attrs[2].GetValueString(),
                _attrs[3].GetValueString(),
                _attrs[4].GetValueString(),
                _attrs[5].GetValueString(),
                _attrs[6].GetValueString(),
                _attrs[7].GetValueString(),
                _attrs[8].GetValueString(),
                _attrs[9].GetValueString(),
                _attrs[10].GetValueString(),
                _attrs[11].GetValueString());
            return retStr;
        }
    }
}
