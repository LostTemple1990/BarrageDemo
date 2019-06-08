using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public class NodeFactory
    {
        public static BaseNode CreateNode(NodeType type)
        {
            BaseNode newNode = null;
            switch ( type )
            {
                case NodeType.Root:
                    newNode = new NodeRoot();
                    break;
            }
            return newNode;
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
            }
            return nodeAttr;
        }
    }
}
