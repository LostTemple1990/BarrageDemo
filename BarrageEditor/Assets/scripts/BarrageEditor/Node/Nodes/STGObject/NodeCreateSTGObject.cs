using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateSTGObject : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateCusomizedSTGObject;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "objectcreate");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 自定义物体名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CustomizedType);
            nodeAttr.Init(this, "Type name", null);
            _attrs.Add(nodeAttr);
            // posX
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CustomizedType);
            nodeAttr.Init(this, "PosX", null);
            _attrs.Add(nodeAttr);
            // posY
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CustomizedType);
            nodeAttr.Init(this, "PosY", null);
            _attrs.Add(nodeAttr);
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.ParaList);
            nodeAttr.Init(this, "Parameter list", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create object";
        }

        public override string ToDesc()
        {
            string ret = string.Format("create object with of type\"{0}\" at ({1},{2})",
                _attrs[0].GetValueString(),
                _attrs[1].GetValueString(), _attrs[2].GetValueString());
            if (GetAttrByIndex(1).GetValueString() != "")
            {
                ret = ret + string.Format(" with parameter {0}", _attrs[3].GetValueString());
            }
            return ret;
        }

        public override string ToLuaHead()
        {
            string typeName = _attrs[0].GetValueString();
            return string.Format("last = CreateCustomizedSTGObject(\"{0}\",{1},{2}{3})\n",
                typeName,
                _attrs[1].GetValueString(),
                _attrs[2].GetValueString(),
                _attrs[3].GetValueString() == "" ? "" : "," + _attrs[3].GetValueString());
        }
    }
}
