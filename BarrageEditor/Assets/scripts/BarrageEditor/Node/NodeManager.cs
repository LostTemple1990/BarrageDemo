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
                case NodeType.DefineBullet:
                    newNode = new NodeDefineBullet();
                    break;
                case NodeType.OnBulletCreate:
                    newNode = new NodeOnBulletCreate();
                    break;
                case NodeType.CreateBullet:
                    newNode = new NodeCreateBullet();
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
            for (i=0;i<node.attrs.Count;i++)
            {
                data.attrValues.Add(node.attrs[i].GetValueString());
            }
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
            List<object> list = new List<object>();
            for (i=0;i<data.attrValues.Count;i++)
            {
                list.Add(data.attrValues[i]);
            }
            node.SetAttrsValues(list);
            return node;
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
