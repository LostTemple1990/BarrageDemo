using System;
using System.Collections.Generic;
using UnityEngine;

namespace BarrageEditor
{
    public class NodeManager
    {
        private static RectTransform _nodeItemContainerTf;

        public static void SetNodeItemContainer(RectTransform containerTf)
        {
            _nodeItemContainerTf = containerTf;
        }

        public static BaseNode CreateNode(NodeType type,bool createDefaultChilds = true)
        {
            if ( _nodeItemContainerTf == null )
            {
                throw new Exception("Create node fail!NodeItemContainer is not set!");
            }
            BaseNode newNode = null;
            switch ( type )
            {
                case NodeType.Root:
                    newNode = new NodeRoot();
                    break;
                case NodeType.ProjectSetting:
                    newNode = new NodeProjectSettings();
                    break;
                case NodeType.Folder:
                    newNode = new NodeFolder();
                    break;
                case NodeType.CodeBlock:
                    newNode = new NodeCodeBlock();
                    break;
                case NodeType.If:
                    newNode = new NodeIf();
                    break;
                case NodeType.IfThen:
                    newNode = new NodeIfThen();
                    break;
                case NodeType.IfElse:
                    newNode = new NodeIfElse();
                    break;
                case NodeType.DefVar:
                    newNode = new NodeDefVar();
                    break;
                case NodeType.Repeat:
                    newNode = new NodeRepeat();
                    break;
                case NodeType.Code:
                    newNode = new NodeCode();
                    break;
                case NodeType.Comment:
                    newNode = new NodeComment();
                    break;

                case NodeType.StageGroup:
                    newNode = new NodeStageGroup();
                    break;
                case NodeType.Stage:
                    newNode = new NodeStage();
                    break;
                case NodeType.StartDialog:
                    newNode = new NodeStartDialog();
                    break;
                case NodeType.DialogWait:
                    newNode = new NodeDialogWait();
                    break;
                case NodeType.CreateDialogCG:
                    newNode = new NodeCreateDialogCG();
                    break;
                case NodeType.HighlightDialogCG:
                    newNode = new NodeHighlightDialogCG();
                    break;
                case NodeType.FadeoutDialogCG:
                    newNode = new NodeFadeOutDialogCG();
                    break;
                case NodeType.CreateSentence:
                    newNode = new NodeCreateDialogBox();
                    break;

                case NodeType.AddTask:
                    newNode = new NodeAddTask();
                    break;
                case NodeType.TaskWait:
                    newNode = new NodeTaskWait();
                    break;
                case NodeType.DefineEnemy:
                    newNode = new NodeDefineEnemy();
                    break;
                case NodeType.OnEnemyCreate:
                    newNode = new NodeOnEnemyCreate();
                    break;
                case NodeType.CreateCustomizedEnemy:
                    newNode = new NodeCreateEnemy();
                    break;
                case NodeType.CreateSimpleEnemy:
                    newNode = new NodeCreateSimpleEnemy();
                    break;
                case NodeType.SetDropItems:
                    newNode = new NodeEnemySetDropItems();
                    break;

                case NodeType.DefineBoss:
                    newNode = new NodeDefineBoss();
                    break;
                case NodeType.OnBossCreate:
                    newNode = new NodeOnBossCreate();
                    break;
                case NodeType.CreateBoss:
                    newNode = new NodeCreateBoss();
                    break;
                case NodeType.SetInvincible:
                    newNode = new NodeSetBossInvincible();
                    break;
                case NodeType.ShowBossBloodBar:
                    newNode = new NodeShowBossBloodBar();
                    break;
                case NodeType.BossSetWanderProps:
                    newNode = new NodeSetBossWanderProps();
                    break;
                case NodeType.BossWander:
                    newNode = new NodeBossWander();
                    break;
                case NodeType.DefineSpellCard:
                    newNode = new NodeDefineSpellCard();
                    break;
                case NodeType.SpellCardInit:
                    newNode = new NodeSpellCardInit();
                    break;
                case NodeType.SpellCardFinish:
                    newNode = new NodeSpellCardFinish();
                    break;
                case NodeType.StartSpellCard:
                    newNode = new NodeStartSpellCard();
                    break;
                case NodeType.SetBossPhaseData:
                    newNode = new NodeSetBossPhaseData();
                    break;

                case NodeType.DefineBullet:
                    newNode = new NodeDefineBullet();
                    break;
                case NodeType.OnBulletCreate:
                    newNode = new NodeOnBulletCreate();
                    break;
                case NodeType.CreateCustomizedBullet:
                    newNode = new NodeCreateBullet();
                    break;
                case NodeType.CreateSimpleBullet:
                    newNode = new NodeCreateSimpleBullet();
                    break;
                case NodeType.SetBulletStyle:
                    newNode = new NodeSetBulletStyle();
                    break;
                case NodeType.ChangeBulletProperty:
                    newNode = new NodeChangeBulletProperty();
                    break;

                case NodeType.DefineLaser:
                    newNode = new NodeDefineLaser();
                    break;
                case NodeType.OnLaserCreate:
                    newNode = new NodeOnLaserCreate();
                    break;
                case NodeType.CreateCustomizedLaser:
                    newNode = new NodeCreateLaser();
                    break;
                case NodeType.LaserTurnHalfOn:
                    newNode = new NodeLaserTurnHalfOn();
                    break;
                case NodeType.LaserTurnOn:
                    newNode = new NodeLaserTurnOn();
                    break;
                case NodeType.LaserTurnOff:
                    newNode = new NodeLaserTurnOff();
                    break;
                case NodeType.LaserChangeLengthTo:
                    newNode = new NodeChangeLengthTo();
                    break;

                case NodeType.DefineLinearLaser:
                    newNode = new NodeDefineLinearLaser();
                    break;
                case NodeType.OnLinearLaserCreate:
                    newNode = new NodeOnLinearLaserCreate();
                    break;
                case NodeType.CreateCustomizedLinearLaser:
                    newNode = new NodeCreateCustomizedLinearLaser();
                    break;
                case NodeType.DefineCurveLaser:
                    newNode = new NodeDefineCurveLaser();
                    break;
                case NodeType.OnCurveLaserCreate:
                    newNode = new NodeOnCurveLaserCreate();
                    break;
                case NodeType.CreateCustomizedCurveLaser:
                    newNode = new NodeCreateCustomizedCurveLaser();
                    break;

                case NodeType.DefineCollider:
                    newNode = new NodeDefineCollider();
                    break;
                case NodeType.OnColliderCreate:
                    newNode = new NodeOnColliderCreate();
                    break;
                case NodeType.CreateCustomizedCollider:
                    newNode = new NodeCreateCustomizedCollider();
                    break;
                case NodeType.CreateSimpleCollider:
                    newNode = new NodeCreateSimpleCollider();
                    break;
                case NodeType.ColliderTrigger:
                    newNode = new NodeColliderTrigger();
                    break;
                case NodeType.Rebound:
                    newNode = new NodeRebound();
                    break;
                case NodeType.DropItems:
                    newNode = new NodeDropItems();
                    break;
                case NodeType.CreateChargeEffect:
                    newNode = new NodeCreateChargeEffect();
                    break;
                case NodeType.CreateBurstEffect:
                    newNode = new NodeCreateBurstEffect();
                    break;
                case NodeType.ShakeScreen:
                    newNode = new NodeShakeScreen();
                    break;
                case NodeType.StopShakeScreen:
                    newNode = new NodeStopShakeScreen();
                    break;

                case NodeType.DefineSTGObject:
                    newNode = new NodeDefineSTGObject();
                    break;
                case NodeType.OnSTGObjectCreate:
                    newNode = new NodeOnSTGObjectCreate();
                    break;
                case NodeType.CreateCusomizedSTGObject:
                    newNode = new NodeCreateSTGObject();
                    break;
                case NodeType.SetSpriteForSTGObject:
                    newNode = new NodeSTGObjectSetSprite();
                    break;
                case NodeType.STGObjectSetColor:
                    newNode = new NodeSTGObjectSetColor();
                    break;
                case NodeType.STGObjectChangeAlphaTo:
                    newNode = new NodeSTGObjectChangeAlphaTo();
                    break;

                case NodeType.UnitSetV:
                    newNode = new NodeSetV();
                    break;
                case NodeType.UnitSetAcce:
                    newNode = new NodeSetAcce();
                    break;
                case NodeType.UnitMoveTo:
                    newNode = new NodeMoveTo();
                    break;
                case NodeType.UnitMoveTowards:
                    newNode = new NodeMoveTowards();
                    break;
                case NodeType.UnitSetStraightParas:
                    newNode = new NodeUnitSetStraightParas();
                    break;
                case NodeType.UnitSetPolarParas:
                    newNode = new NodeUnitSetPolarParas();
                    break;
                case NodeType.UnitSetResistEliminatedTypes:
                    newNode = new NodeUnitSetResistEliminatedTypes();
                    break;
                case NodeType.UnitAttachTo:
                    newNode = new NodeUnitAttachTo();
                    break;
                case NodeType.UnitSetRelativePos:
                    newNode = new NodeUnitSetRelativePos();
                    break;
                case NodeType.KillUnit:
                    newNode = new NodeKillUnit();
                    break;
                case NodeType.DelUnit:
                    newNode = new NodeDelUnit();
                    break;
                case NodeType.UnitEventTrigger:
                    newNode = new NodeUnitEventTrigger();
                    break;

                case NodeType.PlaySound:
                    newNode = new NodePlaySound();
                    break;
                case NodeType.StopSound:
                    newNode = new NodeStopSound();
                    break;
                case NodeType.PauseSound:
                    newNode = new NodePauseSound();
                    break;
                case NodeType.ResumeSound:
                    newNode = new NodeResumeSound();
                    break;
                case NodeType.LoadSound:
                    newNode = new NodeLoadSound();
                    break;
            }
            if (newNode != null)
            {
                newNode.Init(_nodeItemContainerTf);
                if ( createDefaultChilds )
                {
                    newNode.CreateDefualtChilds();
                }
                return newNode;
            }
            throw new Exception("Create node fail!Type " + type + " is not exist!");
        }

        public static BaseNodeAttr CreateNodeAttr(NodeAttrType type)
        {
            BaseNodeAttr nodeAttr = null;
            switch ( type )
            {
                case NodeAttrType.Any:
                    nodeAttr = new NodeAttrAny();
                    break;
                case NodeAttrType.Bool:
                    nodeAttr = new NodeAttrBool();
                    break;
                case NodeAttrType.BulletId:
                    nodeAttr = new NodeAttrBulletId();
                    break;
                case NodeAttrType.CustomizedType:
                    nodeAttr = new NodeAttrCustomizedBulletType();
                    break;
                case NodeAttrType.ParaList:
                    nodeAttr = new NodeAttrPara();
                    break;
                case NodeAttrType.MoveMode:
                    nodeAttr = new NodeAttrMoveMode();
                    break;
                case NodeAttrType.SCCondition:
                    nodeAttr = new NodeAttrSCCondition();
                    break;
                case NodeAttrType.CollisionGroups:
                    nodeAttr = new NodeAttrCollisionGroup();
                    break;
                case NodeAttrType.ResistEliminatedTypes:
                    nodeAttr = new NodeAttrResistEliminatedTypes();
                    break;
                case NodeAttrType.PropertyChangeMode:
                    nodeAttr = new NodeAttrChangeMode();
                    break;
                case NodeAttrType.PropertyType:
                    nodeAttr = new NodeAttrPropertyType();
                    break;
                case NodeAttrType.BlendMode:
                    nodeAttr = new NodeAttrBlendMode();
                    break;
                case NodeAttrType.Layer:
                    nodeAttr = new NodeAttrLayer();
                    break;
                case NodeAttrType.ShapeType:
                    nodeAttr = new NodeAttrShapeType();
                    break;
                case NodeAttrType.DirectionMode:
                    nodeAttr = new NodeAttrDirectionMode();
                    break;
                case NodeAttrType.UnitEventType:
                    nodeAttr = new NodeAttrUnitEventType();
                    break;
                case NodeAttrType.LaserId:
                    nodeAttr = new NodeAttrLaserId();
                    break;
                case NodeAttrType.EnemyStyle:
                    nodeAttr = new NodeAttrSelectEnemyStyle();
                    break;
                case NodeAttrType.ReboundBorder:
                    nodeAttr = new NodeAttrReboundBorder();
                    break;
                case NodeAttrType.DropItems:
                    nodeAttr = new NodeAttrDropItems();
                    break;
            }
            if (nodeAttr != null) return nodeAttr;
            throw new Exception("Create nodeAttr fail!Type " + type + " is not exist!");
        }

        /// <summary>
        /// 根据参数的字符串分离对应的参数列表
        /// </summary>
        /// <param name="paraStr"></param>
        /// <returns></returns>
        public static List<string> GetParaListByString(string paraStr)
        {
            List<string> paraList = new List<string>(); ;
            if ( paraStr != null )
            {
                string[] arr = paraStr.Split(',');
                for (int i=0;i<arr.Length;i++)
                {
                    paraList.Add(arr[i]);
                }
            }
            return paraList;
        }

        public static NodeData SaveAsNodeData(BaseNode node, bool childIncluded)
        {
            int i;
            NodeData data = new NodeData();
            data.type = (int)node.GetNodeType();
            data.attrValues = new List<string>();
            List<BaseNodeAttr> attrList = node.GetAttrs();
            for (i = 0; i < attrList.Count; i++)
            {
                data.attrValues.Add(attrList[i].GetValueString());
            }
            data.isExpand = node.IsExpand;
            if (childIncluded)
            {
                data.childs = new List<NodeData>();
                for (i = 0; i < node.childs.Count; i++)
                {
                    NodeData childData = SaveAsNodeData(node.childs[i], true);
                    data.childs.Add(childData);
                }
            }
            return data;
        }

        public static BaseNode CreateNodesByNodeDatas(NodeData data)
        {
            int i;
            BaseNode node = CreateNode((NodeType)data.type, false);
            for (i=0;i<data.childs.Count;i++)
            {
                BaseNode child = CreateNodesByNodeDatas(data.childs[i]);
                node.InsertChildNode(child, -1);
            }
            node.InitWithNodeData(data);
            return node;
        }

        /// <summary>
        /// 根据索引找到对应的节点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static BaseNode FindNodeByIndex(int index)
        {
            BaseNode root = BarrageProject.RootNode;
            return SearchNodeByIndex(root, ref index);
        }

        private static BaseNode SearchNodeByIndex(BaseNode node, ref int index)
        {
            if (index == 0)
                return node;
            int childCount = node.GetChildCount();
            BaseNode findNode = null;
            for (int i = 0; i < node.GetChildCount(); i++)
            {
                index--;
                findNode = SearchNodeByIndex(node.GetChildByIndex(i), ref index);
                if (findNode != null)
                    break;
            }
            return findNode;
        }

        /// <summary>
        /// 获得节点在树的中序遍历中的索引
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static int GetNodeIndex(BaseNode node)
        {
            int index = 0;
            BaseNode root = BarrageProject.RootNode;
            if (SearchNodeIndex(node, root, ref index))
                return index;
            return -1;
        }

        private static bool SearchNodeIndex(BaseNode searchNode, BaseNode curSearchNode, ref int index)
        {
            if (searchNode == curSearchNode)
                return true;
            int childCount = curSearchNode.GetChildCount();
            for (int i=0;i<childCount;i++)
            {
                index++;
                bool isFind = SearchNodeIndex(searchNode, curSearchNode.GetChildByIndex(i), ref index);
                if (isFind)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取在中序遍历中当前节点的下一个节点
        /// <para>若当前节点是树的最后一个节点，则返回自身</para>
        /// </summary>
        /// <param name="curNode"></param>
        /// <returns></returns>
        public static BaseNode GetNextNode(BaseNode curNode)
        {
            BaseNode child = curNode.GetChildByIndex(0);
            if (child != null)
                return child;
            child = curNode;
            BaseNode parent = child.parentNode;
            while (parent != null)
            {
                int index = parent.GetChildIndex(child);
                if (parent.GetChildByIndex(index + 1) != null)
                {
                    return parent.GetChildByIndex(index + 1);
                }
                child = parent;
                parent = child.parentNode;
            }
            return curNode;
        }

        /// <summary>
        /// 获取在中序遍历中当前节点的上一个节点
        /// <para>若当前节点是树的根节点，则返回自身</para>
        /// </summary>
        /// <param name="curNode"></param>
        /// <returns></returns>
        public static BaseNode GetPreNode(BaseNode curNode)
        {
            BaseNode preNode = curNode;
            BaseNode parent = curNode.parentNode;
            if (parent == null)
                return preNode;
            int index = parent.GetChildIndex(curNode);
            if (parent.GetChildByIndex(index - 1) != null)
            {
                preNode = parent.GetChildByIndex(index - 1);
                while (preNode.GetChildCount() > 0)
                {
                    preNode = preNode.GetChildByIndex(preNode.GetChildCount() - 1);
                }
                return preNode;
            }
            return parent;
        }

        public static void ApplyDepthForLua(int depth,ref string luaStr)
        {
            bool endWithLine = false;
            if ( luaStr.EndsWith("\n") )
            {
                endWithLine = true;
            }
            string tab = "";
            for (int i=0;i<depth;i++)
            {
                tab += "    ";
            }
            luaStr = luaStr.Replace("\n", "\n" + tab);
            if (endWithLine)
            {
                int length = luaStr.Length;
                luaStr = luaStr.Substring(0, length - depth * 4);
            }
            luaStr = tab + luaStr;
        }

    }
}
