﻿using UnityEngine;
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
            _attrs.Add(nodeAttr);
            // 初始x坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosX", null);
            _attrs.Add(nodeAttr);
            // 初始有y坐标
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "PosY", null);
            _attrs.Add(nodeAttr);
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.ParaList);
            nodeAttr.Init(this, "Parameter list", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create enemy";
        }

        public override string ToDesc()
        {
            return string.Format("create enemy of type\"{0}\" at ({1},{2}) with parameter {3}",
                _attrs[0].GetValueString(), _attrs[1].GetValueString(), _attrs[2].GetValueString(), _attrs[3].GetValueString());
        }

        public override string ToLuaHead()
        {
            string typeName = _attrs[0].GetValueString();
            CustomDefineData data = CustomDefine.GetDataByTypeAndName(CustomDefineType.Enemy, typeName);
            int paraCount = 0;
            if (data != null)
            {
                if ( data.paraListStr.IndexOf(',') != -1 )
                {
                    paraCount = data.paraListStr.Split(',').Length;
                }
            }
            return string.Format("last = CreateCustomizedEnemy(\"{0}\",{1},{2}{3})\n",
                typeName,
                _attrs[1].GetValueString(),
                _attrs[2].GetValueString(),
                _attrs[3].GetValueString() == "" ? "" : "," + _attrs[3].GetValueString());
        }
    }
}
