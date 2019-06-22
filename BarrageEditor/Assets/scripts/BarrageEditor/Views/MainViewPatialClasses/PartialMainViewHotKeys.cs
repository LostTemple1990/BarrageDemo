using UnityEngine;
using UnityEngine.UI;
using YKEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace BarrageEditor
{
    public partial class MainView : ViewBase, IEventReciver
    {
        private void InitHotKeys()
        {

        }

        private void CheckHotKeys()
        {
            // 当前焦点是否为主界面
            bool isFocusOnMainView = UIManager.GetInstance().GetFocusViewId() == ViewID.MainView;
            if (!isFocusOnMainView)
                return;
            // 检测基本的操作
            // 检测project面板的按键操作
            // todo 看是否需要做一次只检测一个操作
            GameObject curSelectedGo = EventSystem.current.currentSelectedGameObject;
            if (curSelectedGo != _projectPanelGo && curSelectedGo != null)
                return;
            CheckHotKeyUp();
            CheckHotKeyDown();
            CheckHotKeyDelete();
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                CheckHotKeyUndo();
                CheckHotKeyRedo();
                CheckHotKeyCopy();
                CheckHotKeyCut();
                CheckHotKeyPaste();
            }
        }

        private void CheckSave()
        {

        }

        private void CheckHotKeyUndo()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Undo.PerformUndo();
            }
        }

        private void CheckHotKeyRedo()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Undo.PerformUndo();
            }
        }

        private void CheckHotKeyDelete()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                OnDeleteClickHandler();
            }
        }

        private void CheckHotKeyCopy()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                OnCopyClickHandler();
            }
        }

        private void CheckHotKeyCut()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                OnCutClickHandler();
            }
        }

        private void CheckHotKeyPaste()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                OnPasteClickHandler();
            }
        }

        private void CheckHotKeyUp()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //Debug.Log(EventSystem.current.currentSelectedGameObject);
                OnKeyUpArrowDownHandler();
            }
        }

        private void OnKeyUpArrowDownHandler()
        {
            if (_curSelectedNode == null)
                return;
            BaseNode preNode = NodeManager.GetPreNode(_curSelectedNode);
            if (preNode != _curSelectedNode)
                preNode.OnSelected(true);
        }

        private void CheckHotKeyDown()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //Debug.Log(EventSystem.current.currentSelectedGameObject);
                OnKeyDownArrowDownHandler();
            }
        }

        private void OnKeyDownArrowDownHandler()
        {
            if (_curSelectedNode == null)
                return;
            BaseNode nextNode = NodeManager.GetNextNode(_curSelectedNode);
            if (nextNode != _curSelectedNode)
                nextNode.OnSelected(true);
        }
    }
}