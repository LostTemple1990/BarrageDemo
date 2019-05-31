using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class MainView : ViewBase
    {
        private GameObject _fileToggle;
        private RectTransform _fileToggleTf;
        private GameObject _editToggle;
        private GameObject _helpToggle;

        private RectTransform _projectContentTf;

        protected override void Init()
        {
            Transform menuBarTf = _view.transform.Find("MenuBar");
            _fileToggleTf = menuBarTf.Find("File") as RectTransform;
            _fileToggle = _fileToggleTf.gameObject;
            _editToggle = menuBarTf.Find("Edit").gameObject;
            _helpToggle = menuBarTf.Find("Help").gameObject;

            _projectContentTf = _viewTf.Find("ProjectPanel/Scroll View/Viewport/Content").GetComponent<RectTransform>();
            TestProjectNodeItems();

            AddListeners();
        }

        public void AddListeners()
        {
            UIEventListener.Get(_fileToggle).AddClick(OnFileTabClick);
        }

        private void OnFileTabClick()
        {
            Vector2 uiPos = UIManager.GetInstance().GetUIPosition(_fileToggleTf);
            Vector2 viewPos = new Vector2(uiPos.x - _fileToggleTf.rect.width / 2, uiPos.y - _fileToggleTf.rect.height / 2);
            UIManager.GetInstance().OpenView(ViewID.MenuBarListView, 1, viewPos);
        }

        private void TestProjectNodeItems()
        {
            BaseNode root = new NodeRoot();
            root.Init(null, _projectContentTf);
            root.UpdateDesc();
            NodeProjectSettings ps = new NodeProjectSettings();
            ps.Init(root, _projectContentTf);
            ps.UpdateDesc();
            root.Expand(true);
        }
    }
}
