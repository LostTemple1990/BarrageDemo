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

        public static BaseNode CreateNode(NodeType type)
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
            }
            if (nodeAttr != null) return nodeAttr;
            throw new Exception("Create nodeAttr fail!Type " + type + " is not exist!");
        }
    }
}
