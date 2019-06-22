using System;
using System.Collections.Generic;

namespace BarrageEditor
{
    public class Undo
    {
        private static List<HistoryMemento> redoList = new List<HistoryMemento>();
        private static List<HistoryMemento> undoList = new List<HistoryMemento>();
        private static int undoPos = -1;
        private static int redoPos = -1;

        private const int MaxUndoCount = 5;

        public static void PerformUndo()
        {
            if (undoPos < 0)
                return;
            HistoryMemento undo = undoList[undoPos];
            HistoryMemento redo = undo.PerformUndo();
            AddToRedoStack(redo);
            undoPos--;
        }

        public static void PerformRedo()
        {
            if (redoPos < 0)
                return;
            HistoryMemento redo = redoList[redoPos];
            redo.PerformUndo();
            redoPos--;
            undoPos++;
        }

        private static void AddToRedoStack(HistoryMemento redo)
        {
            // 当前恢复位置
            redoPos++;
            if (redoPos >= redoList.Count)
            {
                redoList.Add(redo);
            }
            else
            {
                redoList[redoPos] = redo;
            }
        }

        public static void AddToUndoTask(HistoryMemento undo)
        {
            undoPos++;
            if (undoPos>=undoList.Count)
            {
                undoList.Add(undo);
            }
            else
            {
                undoList[undoPos] = undo;
            }
            if (undoPos >= MaxUndoCount)
            {
                undoList.RemoveAt(0);
                undoPos--;
            }
            // 清空redo列表
            redoPos = -1;
            redoList.Clear();
        }
    }

    public abstract class HistoryMemento
    {
        protected abstract HistoryMemento OnUndo();

        public HistoryMemento PerformUndo()
        {
            return OnUndo();
        }
    }

    public class OpDeleleHM : HistoryMemento
    {
        public int parentIndex;
        public int childIndex;
        public NodeData delNodeData;
        
        protected override HistoryMemento OnUndo()
        {
            BaseNode parent = NodeManager.FindNodeByIndex(parentIndex);
            NodeType childType = (NodeType)delNodeData.type;
            if (parent.CheckCanInsertChildNode(childType))
            {
                BaseNode child = NodeManager.CreateNodesByNodeDatas(delNodeData);
                parent.InsertChildNode(child, childIndex);
                parent.Expand(true);
                child.OnSelected(true);
                var hm = new OpPasteHM
                {
                    parentIndex = parentIndex,
                    childIndex = childIndex,
                    pasteNodeData = delNodeData,
                };
                return hm;
            }
            return null;
        }
    }

    public class OpPasteHM : HistoryMemento
    {
        public int parentIndex;
        public int childIndex;
        public NodeData pasteNodeData;

        protected override HistoryMemento OnUndo()
        {
            BaseNode parent = NodeManager.FindNodeByIndex(parentIndex);
            BaseNode child = parent.GetChildByIndex(childIndex);
            if (parent.RemoveChildNode(child))
            {
                parent.Expand(true);
                parent.OnSelected(true);
                var hm = new OpDeleleHM
                {
                    parentIndex = parentIndex,
                    childIndex = childIndex,
                    delNodeData = pasteNodeData,
                };
                return hm;
            }
            return null;
        }
    }

    public class OpInsertHM : HistoryMemento
    {
        public int parentIndex;
        public int childIndex;
        public NodeData insertNodeData;

        protected override HistoryMemento OnUndo()
        {
            BaseNode parent = NodeManager.FindNodeByIndex(parentIndex);
            BaseNode child = parent.GetChildByIndex(childIndex);
            if (parent.RemoveChildNode(child))
            {
                parent.Expand(true);
                parent.OnSelected(true);
                var hm = new OpDeleleHM
                {
                    parentIndex = parentIndex,
                    childIndex = childIndex,
                    delNodeData = insertNodeData,
                };
                return hm;
            }
            return null;
        }
    }

    public class OpNodeAttrValuesModificationHM : HistoryMemento
    {
        public int nodeIndex;
        public List<object> preValues;
        public List<object> curValues;

        protected override HistoryMemento OnUndo()
        {
            BaseNode curNode = NodeManager.FindNodeByIndex(nodeIndex);
            curNode.SetAttrsValues(preValues);
            var hm = new OpNodeAttrValuesModificationHM
            {
                nodeIndex = nodeIndex,
                preValues = curValues,
                curValues = preValues,
            };
            return hm;
        }
    }
}
