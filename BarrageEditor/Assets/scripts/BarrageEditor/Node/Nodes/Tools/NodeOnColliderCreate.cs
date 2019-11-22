using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeOnColliderCreate : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.OnColliderCreate;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "colliderinit");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Parameter list", null);
            _attrs.Add(nodeAttr);
            // 长度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Size x", null);
            _attrs.Add(nodeAttr);
            // 宽度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Size y", null);
            _attrs.Add(nodeAttr);
            // 碰撞组
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CollisionGroups);
            nodeAttr.Init(this, "Collision Groups", null);
            _attrs.Add(nodeAttr);
        }

        public override void OnAttributeValueChanged(BaseNodeAttr attr = null)
        {
            // 手动修改引起的参数变更
            // 则更新DefineList
            if (attr != null && attr == GetAttrByIndex(0))
            {
                if ((parentNode as NodeDefineCollider).IsWatchingData)
                {
                    // 参数列表发生变化，修改缓存
                    string name = parentNode.GetAttrByIndex(0).GetValueString();
                    CustomDefine.ModifyDefineParaList(CustomDefineType.Collider, name, attr.GetValueString());
                }
            }
            base.OnAttributeValueChanged(attr);
        }

        public override string GetNodeName()
        {
            return "on collider create";
        }

        public override string ToDesc()
        {
            return string.Format("on create:({0})", _attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            string name = parentNode.GetAttrByIndex(0).GetValueString();
            string ret = string.Format("CustomizedColliderTable[\"{0}\"].Init = function(self{1})\n",
                name,
                _attrs[0].GetValueString() == "" ? "" : "," + _attrs[0].GetValueString()  //不带参数的话self后不带任何参数了，因此不加分隔符','
                );
            ret = string.Format("{0}    {1}:SetSize({2},{3})\n    {1}:SetCollisionGroup({4})\n",
                ret,
                "self",
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString());
            return ret;
        }

        public override string ToLuaFoot()
        {
            return string.Format("end\n");
        }
    }
}
