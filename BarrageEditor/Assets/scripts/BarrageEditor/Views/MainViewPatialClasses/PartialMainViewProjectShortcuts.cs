using UnityEngine;
using UnityEngine.UI;
using YKEngine;
using System.Collections.Generic;

namespace BarrageEditor
{
    public partial class MainView : ViewBase, IEventReciver
    {
        #region projectShortcut

        #region 项目快捷按钮
        /// <summary>
        /// 创建项目
        /// </summary>
        private GameObject _projectShortcutCreateGo;
        /// <summary>
        /// 打开项目
        /// </summary>
        private GameObject _projectShortcutOpenGo;
        /// <summary>
        /// 保存项目
        /// </summary>
        private GameObject _projectShortcutSaveGo;

        /// <summary>
        /// 撤销
        /// </summary>
        private GameObject _projectShortcutUndoGo;
        /// <summary>
        /// 取消撤销
        /// </summary>
        private GameObject _projectShortcutRedoGo;

        /// <summary>
        /// 删除节点
        /// </summary>
        private GameObject _projectShortcutDeleteGo;
        /// <summary>
        /// 复制节点
        /// </summary>
        private GameObject _projectShortcutCopyGo;
        /// <summary>
        /// 剪切节点
        /// </summary>
        private GameObject _projectShortcutCutGo;
        /// <summary>
        /// 粘贴节点
        /// </summary>
        private GameObject _projectShortcutPasteGo;

        /// <summary>
        /// 生成lua代码
        /// </summary>
        private GameObject _projectShortcutPackGo;

        /// <summary>
        /// 在所选项目之后插入
        /// </summary>
        private GameObject _projectShortcutInsertAfterGo;
        /// <summary>
        /// 在所选项目之前插入
        /// </summary>
        private GameObject _projectShortcutInsertBeforeGo;
        /// <summary>
        /// 作为所选项目的子节点插入
        /// </summary>
        private GameObject _projectShortcutInsertAsChildGo;
        #endregion

        private void InitProjectShortcutBar()
        {
            RectTransform tf = _view.transform.Find("ProjectShortcutBar").GetComponent<RectTransform>();
            // 创建新项目
            _projectShortcutCreateGo = tf.Find("Create").gameObject;
            UIEventListener.Get(_projectShortcutCreateGo).AddPointerEnter(OnProjectShortcutCreatePointerEnter);
            UIEventListener.Get(_projectShortcutCreateGo).AddPointerExit(OnProjectShortcutPointerExit);
            //打开项目
            _projectShortcutCreateGo = tf.Find("Open").gameObject;
            UIEventListener.Get(_projectShortcutCreateGo).AddPointerEnter(OnProjectShortcutOpenPointerEnter);
            UIEventListener.Get(_projectShortcutCreateGo).AddPointerExit(OnProjectShortcutPointerExit);
            // 保存项目
            _projectShortcutCreateGo = tf.Find("Save").gameObject;
            UIEventListener.Get(_projectShortcutCreateGo).AddPointerEnter(OnProjectShortcutSavePointerEnter);
            UIEventListener.Get(_projectShortcutCreateGo).AddPointerExit(OnProjectShortcutPointerExit);

            // 撤销
            _projectShortcutUndoGo = tf.Find("Undo").gameObject;
            UIEventListener.Get(_projectShortcutUndoGo).AddPointerEnter(OnProjectShortcutUndoPointerEnter);
            UIEventListener.Get(_projectShortcutUndoGo).AddPointerExit(OnProjectShortcutPointerExit);
            UIEventListener.Get(_projectShortcutUndoGo).AddClick(OnUndoClickHandler);
            // 取消撤销
            _projectShortcutRedoGo = tf.Find("Redo").gameObject;
            UIEventListener.Get(_projectShortcutRedoGo).AddPointerEnter(OnProjectShortcutRedoPointerEnter);
            UIEventListener.Get(_projectShortcutRedoGo).AddPointerExit(OnProjectShortcutPointerExit);
            UIEventListener.Get(_projectShortcutRedoGo).AddClick(OnRedoClickHandler);

            // 删除节点
            _projectShortcutDeleteGo = tf.Find("Delete").gameObject;
            UIEventListener.Get(_projectShortcutDeleteGo).AddPointerEnter(OnProjectShortcutDeletePointerEnter);
            UIEventListener.Get(_projectShortcutDeleteGo).AddPointerExit(OnProjectShortcutPointerExit);
            UIEventListener.Get(_projectShortcutDeleteGo).AddClick(OnDeleteClickHandler);
            // 复制节点
            _projectShortcutCopyGo = tf.Find("Copy").gameObject;
            UIEventListener.Get(_projectShortcutCopyGo).AddPointerEnter(OnProjectShortcutCopyPointerEnter);
            UIEventListener.Get(_projectShortcutCopyGo).AddPointerExit(OnProjectShortcutPointerExit);
            UIEventListener.Get(_projectShortcutCopyGo).AddClick(OnCopyClickHandler);
            // 剪切节点
            _projectShortcutCutGo = tf.Find("Cut").gameObject;
            UIEventListener.Get(_projectShortcutCutGo).AddPointerEnter(OnProjectShortcutCutPointerEnter);
            UIEventListener.Get(_projectShortcutCutGo).AddPointerExit(OnProjectShortcutPointerExit);
            UIEventListener.Get(_projectShortcutCutGo).AddClick(OnCutClickHandler);
            // 粘贴节点
            _projectShortcutPasteGo = tf.Find("Paste").gameObject;
            UIEventListener.Get(_projectShortcutPasteGo).AddPointerEnter(OnProjectShortcutPastePointerEnter);
            UIEventListener.Get(_projectShortcutPasteGo).AddPointerExit(OnProjectShortcutPointerExit);
            UIEventListener.Get(_projectShortcutPasteGo).AddClick(OnPasteClickHandler);

            // 生成
            _projectShortcutPackGo = tf.Find("Pack").gameObject;
            UIEventListener.Get(_projectShortcutPackGo).AddPointerEnter(OnProjectShortcutPackPointerEnter);
            UIEventListener.Get(_projectShortcutPackGo).AddPointerExit(OnProjectShortcutPointerExit);

            // 在之后插入
            _projectShortcutInsertAfterGo = tf.Find("InsertModeToggleGroup/InsertAfter").gameObject;
            UIEventListener.Get(_projectShortcutInsertAfterGo).AddPointerEnter(OnProjectShortcutInsertAfterPointerEnter);
            UIEventListener.Get(_projectShortcutInsertAfterGo).AddPointerExit(OnProjectShortcutPointerExit);
            UIEventListener.Get(_projectShortcutInsertAfterGo).AddClick(() => { _curInsertMode = NodeInsertMode.InsertAfter; });
            // 在之前插入
            _projectShortcutInsertBeforeGo = tf.Find("InsertModeToggleGroup/InsertBefore").gameObject;
            UIEventListener.Get(_projectShortcutInsertBeforeGo).AddPointerEnter(OnProjectShortcutInsertBeforePointerEnter);
            UIEventListener.Get(_projectShortcutInsertBeforeGo).AddPointerExit(OnProjectShortcutPointerExit);
            UIEventListener.Get(_projectShortcutInsertBeforeGo).AddClick(() => { _curInsertMode = NodeInsertMode.InsertBefore; });
            // 作为子节点插入
            _projectShortcutInsertAsChildGo = tf.Find("InsertModeToggleGroup/InsertAsChild").gameObject;
            UIEventListener.Get(_projectShortcutInsertAsChildGo).AddPointerEnter(OnProjectShortcutInsertAsChildPointerEnter);
            UIEventListener.Get(_projectShortcutInsertAsChildGo).AddPointerExit(OnProjectShortcutPointerExit);
            UIEventListener.Get(_projectShortcutInsertAsChildGo).AddClick(() => { _curInsertMode = NodeInsertMode.InsertAsChild; });
        }

        #region tip
        private void OnProjectShortcutPointerExit()
        {
            UIManager.GetInstance().CloseView(ViewID.TooltipView);
        }

        private void OnProjectShortcutCreatePointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Create a new project");
        }

        private void OnProjectShortcutOpenPointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Open a project");
        }

        private void OnProjectShortcutSavePointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Save the project");
        }

        private void OnProjectShortcutUndoPointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Undo(Ctrl+Z)");
        }

        private void OnProjectShortcutRedoPointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Redo(Ctrl+Y)");
        }

        private void OnProjectShortcutDeletePointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Delete(Del)");
        }

        private void OnProjectShortcutCopyPointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Cpoy(Ctrl+C)");
        }

        private void OnProjectShortcutCutPointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Cut(Ctrl+X)");
        }

        private void OnProjectShortcutPastePointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Paste(Ctrl+V)");
        }

        private void OnProjectShortcutPackPointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Build current project");
        }

        private void OnProjectShortcutInsertAfterPointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Insert after");
        }

        private void OnProjectShortcutInsertBeforePointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Insert before");
        }

        private void OnProjectShortcutInsertAsChildPointerEnter()
        {
            UIManager.GetInstance().OpenView(ViewID.TooltipView, "Insert as child");
        }
        #endregion

        private void OnUndoClickHandler()
        {
            Undo.PerformUndo();
        }

        private void OnRedoClickHandler()
        {
            Undo.PerformRedo();
        }

        private void OnDeleteClickHandler()
        {
            if (_curSelectedNode == null)
            {
                BarrageProject.LogWarning("Delete fail!Please select a node first");
                return;
            }
            BaseNode parent = _curSelectedNode.parentNode;
            if (parent == null)
            {
                BarrageProject.LogWarning("Delete fail!Node root is not deletable");
                return;
            }
            if (!_curSelectedNode.IsDeletable)
            {
                BarrageProject.LogWarning(string.Format("Delete fail!Node {0} is not deletable", _curSelectedNode.GetNodeName()));
                return;
            }
            int indexInParent = parent.GetChildIndex(_curSelectedNode);
            NodeData nd = NodeManager.SaveAsNodeData(_curSelectedNode, true);
            // 删除完成之后，默认选中下一个节点
            if (parent.RemoveChildNode(_curSelectedNode))
            {
                BaseNode newSelectNode = parent.GetChildByIndex(indexInParent);
                if (newSelectNode == null)
                {
                    newSelectNode = parent.GetChildByIndex(indexInParent - 1);
                    if (newSelectNode == null)
                    {
                        newSelectNode = parent;
                    }
                }
                newSelectNode.OnSelected(true);
                OpDeleleHM hm = new OpDeleleHM
                {
                    parentIndex = NodeManager.GetNodeIndex(parent),
                    childIndex = indexInParent,
                    delNodeData = nd,
                };
                Undo.AddToUndoTask(hm);
            }
            //else
            //{
            //    BarrageProject.LogWarning(string.Format("Delete fail!Node {0} is not deletable", _curSelectedNode.GetNodeName()));
            //}
        }

        private void OnCopyClickHandler()
        {
            if (_curSelectedNode == null)
            {
                BarrageProject.LogWarning("Copy fail!Please select a node first");
                return;
            }
            NodeData nd = NodeManager.SaveAsNodeData(_curSelectedNode, true);
            Clipboard.SetDataObject(nd);
        }

        private void OnCutClickHandler()
        {
            if (_curSelectedNode == null)
            {
                BarrageProject.LogWarning("Cut fail!Please select a node first");
                return;
            }
            BaseNode parent = _curSelectedNode.parentNode;
            if (parent == null)
            {
                BarrageProject.LogWarning("Cut fail!Node root is not deletable");
                return;
            }
            if (!_curSelectedNode.IsDeletable)
            {
                BarrageProject.LogWarning(string.Format("Cut fail!Node {0} is not deletable", _curSelectedNode.GetNodeName()));
                return;
            }
            // 保存该节点的数据
            NodeData nd = NodeManager.SaveAsNodeData(_curSelectedNode, true);
            Clipboard.SetDataObject(nd);
            // 删除节点
            int index = parent.GetChildIndex(_curSelectedNode);
            // 删除完成之后，默认选中下一个节点
            if (parent.RemoveChildNode(_curSelectedNode))
            {
                BaseNode newSelectNode = parent.GetChildByIndex(index);
                if (newSelectNode == null)
                {
                    newSelectNode = parent.GetChildByIndex(index - 1);
                    if (newSelectNode == null)
                    {
                        newSelectNode = parent;
                    }
                }
                newSelectNode.OnSelected(true);
                OpDeleleHM hm = new OpDeleleHM
                {
                    parentIndex = NodeManager.GetNodeIndex(parent),
                    childIndex = index,
                    delNodeData = nd,
                };
                Undo.AddToUndoTask(hm);
            }
        }

        public void OnPasteClickHandler()
        {
            if (_curSelectedNode == null)
            {
                BarrageProject.LogWarning("Paste fail!Please select a node first");
                return;
            }
            NodeData nd = Clipboard.GetDataObject() as NodeData;
            if (nd == null)
            {
                BarrageProject.LogWarning("Paste fail!There is no data in clipboard");
                return;
            }
            BaseNode parent;
            int insertIndex;
            GetParentNodeAndInsertIndexByInsertMode(_curInsertMode, _curSelectedNode, out parent, out insertIndex);
            NodeType childType = (NodeType)nd.type;
            if (!parent.CheckCanInsertChildNode(childType))
            {
                string msg = string.Format("can not insert {0} as child of {1}", childType, parent.GetNodeType());
                BarrageProject.LogError(msg);
                return;
            }
            BaseNode childNode = NodeManager.CreateNodesByNodeDatas(nd);
            if (parent.InsertChildNode(childNode, insertIndex))
            {
                parent.Expand(true);
                childNode.OnSelected(true);
                OpPasteHM hm = new OpPasteHM
                {
                    parentIndex = NodeManager.GetNodeIndex(parent),
                    childIndex = parent.GetChildIndex(childNode),
                    pasteNodeData = nd,
                };
                Undo.AddToUndoTask(hm);
            }
        }

        #endregion
    }
}