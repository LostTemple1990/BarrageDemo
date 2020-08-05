using UnityEngine;
using UnityEngine.UI;
using YKEngine;
using UnityEngine.EventSystems;
using System.Diagnostics;

namespace BarrageEditor
{
    public partial class MainView : ViewBase, IEventReciver
    {
        private bool _isFocusOnMainView;
        private bool _isFocusOnProjectPanel;
        private Camera _uiCamera;

        private void InitFocus()
        {
            _isFocusOnMainView = false;
            _isFocusOnProjectPanel = false;
            _uiCamera = UIManager.GetInstance().GetUICamera();
        }

        private void CheckFocus()
        {
            // 当前焦点是否为主界面
            _isFocusOnMainView = UIManager.GetInstance().GetFocusViewId() == ViewID.MainView;
            if (!_isFocusOnMainView)
            {
                FocusOnProjectPanel(false);
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(_projectPanelTf, Input.mousePosition, _uiCamera))
                {
                    FocusOnProjectPanel(true);
                }
                else
                {
                    FocusOnProjectPanel(false);
                }
            }
        }

        private void FocusOnProjectPanel(bool value)
        {
            _isFocusOnProjectPanel = value;
            if (_curSelectedNode != null)
            {
                _curSelectedNode.FocusOn(value);
            }
        }

        private void CheckHotKeys()
        {
            if (!_isFocusOnProjectPanel)
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
                CheckHotKeySave();
            }
        }

        [Conditional("Release")]
        private void CheckHotKeySave()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                OnSaveClickHandler();
            }
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