using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateEnemy : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateCustomizedEnemy;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "enemycreate");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 敌机类型
            // todo attribute of enemyId
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CustomizedType);
            nodeAttr.Init(this, "Type name", null);
            attrs.Add(nodeAttr);
            // id
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Enemy Id", null);
            attrs.Add(nodeAttr);
            // 初始x坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosX", null);
            attrs.Add(nodeAttr);
            // 初始有y坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosY", null);
            attrs.Add(nodeAttr);
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.ParaList);
            nodeAttr.Init(this, "Parameter list", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create enemy";
        }

        public override string ToDesc()
        {
            return string.Format("create enemy with id {0} of type\"{1}\" at ({2},{3}) with parameter {4}",
                attrs[1].GetValueString(), attrs[0].GetValueString(), attrs[2].GetValueString(), attrs[3].GetValueString(), attrs[4].GetValueString());
        }

        public override string ToLuaHead()
        {
            string typeName = attrs[0].GetValueString();
            CustomDefineData data = CustomDefine.GetDataByTypeAndName(CustomDefineType.Enemy, typeName);
            int paraCount = 0;
            if (data != null)
            {
                if ( data.paraListStr.IndexOf(',') != -1 )
                {
                    paraCount = data.paraListStr.Split(',').Length;
                }
            }
            return string.Format("last = lib.CreateCustomizedEnemy(\"{0}\",{1},{2},{3},{4}{5})\n",
                typeName,
                attrs[1].GetValueString(),
                attrs[2].GetValueString(),
                attrs[3].GetValueString(),
                attrs[4].GetValueString() == "" ? "" : attrs[4].GetValueString() + ",",
                paraCount);
        }
    }
}
