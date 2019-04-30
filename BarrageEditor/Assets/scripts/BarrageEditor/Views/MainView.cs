using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class MainView : ViewBase
    {
        public GameObject _fileToggle;
        public RectTransform _fileToggleTf;
        public GameObject _editToggle;
        public GameObject _helpToggle;

        public override void Init(GameObject viewObj)
        {
            base.Init(viewObj);
            Transform menuBarTf = viewObj.transform.Find("MenuBar");
            _fileToggleTf = menuBarTf.Find("File") as RectTransform;
            _fileToggle = _fileToggleTf.gameObject;
            _editToggle = menuBarTf.Find("Edit").gameObject;
            _helpToggle = menuBarTf.Find("Help").gameObject;

            AddListeners();
        }

        public void AddListeners()
        {
            UIEventListener.Get(_fileToggle).AddClick(OnFileTabClick);
        }

        private void OnFileTabClick()
        {
            Vector2 pos = _fileToggleTf.position;
            pos.y = pos.y - _fileToggleTf.rect.height / 2;
            UIManager.GetInstance().OpenView(ViewID.MenuBarListView, 1, pos);
        }
    }
}
