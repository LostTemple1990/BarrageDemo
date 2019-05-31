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
    }
}
