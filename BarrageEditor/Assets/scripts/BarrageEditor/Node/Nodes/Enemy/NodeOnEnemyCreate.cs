using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeOnEnemyCreate : BaseNode
    {
        /// <summary>
        /// 参数列表的属性的索引
        /// </summary>
        public const int ParamListAttrIndex = 0;

        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.OnEnemyCreate;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "enemyinit");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Parameter list", null);
            attrs.Add(nodeAttr);
            // 敌机id
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "EnemyId", null);
            attrs.Add(nodeAttr);
            // 血量
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Hit point", null);
            attrs.Add(nodeAttr);
        }

        public override void OnAttributeValueChanged(BaseNodeAttr attr = null)
        {
            // 手动修改引起的参数变更
            // 则更新DefineList
            if (attr != null)
            {
                if (attr == GetAttrByIndex(ParamListAttrIndex))
                {
                    if ((parentNode as NodeDefineEnemy).IsWatchingData)
                    {
                        // 参数列表发生变化，修改缓存
                        string paraList = parentNode.GetAttrByIndex(1).GetValueString();
                        CustomDefine.ModifyDefineParaList(CustomDefineType.Enemy, paraList, attr.GetValueString());
                    }
                }
            }
            base.OnAttributeValueChanged(attr);
        }

        public override string GetNodeName()
        {
            return "on enemy create";
        }

        public override string ToDesc()
        {
            return string.Format("on create:({0})", attrs[ParamListAttrIndex].GetValueString());
        }

        public override string ToLuaHead()
        {
            string name = parentNode.GetAttrByIndex(0).GetValueString();
            return string.Format("CustomizedEnemyTable[\"{0}\"].Init = function(self{1})\n    self:Init({2})\n    self:SetMaxHp({3})\n",
                name,
                GetAttrByIndex(ParamListAttrIndex).GetValueString() == "" ? "" : ","+ GetAttrByIndex(ParamListAttrIndex).GetValueString(),  //不带参数的话self后不带任何参数了，因此不加分隔符','
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString());
        }

        public override string ToLuaFoot()
        {
            return string.Format("end\n");
        }
    }
}
