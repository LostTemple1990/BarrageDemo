using UnityEngine;
using UnityEngine.UI;
using YKEngine;
using System.Collections.Generic;

namespace BarrageEditor
{
    public class MainView : ViewBase,IEventReciver
    {
        private GameObject _fileToggle;
        private RectTransform _fileToggleTf;
        private GameObject _editToggle;
        private GameObject _helpToggle;

        private RectTransform _projectContentTf;
        /// <summary>
        /// 节点详细信息的面板
        /// </summary>
        private RectTransform _nodeInfoPanelTf;

        struct NodeAttrItem
        {
            public GameObject itemGo;
            public Text attrNameText;
            public InputField valueText;
            public GameObject dropdownArrowGo;
            public Dropdown dropdown;
            public GameObject editBtn;
        }
        /// <summary>
        /// 节点属性的item结构
        /// </summary>
        private List<NodeAttrItem> _nodeAttrItems;
        /// <summary>
        /// 节点名称的text
        /// </summary>
        private Text _nodeNameText;

        /// <summary>
        /// 当前选择的节点
        /// </summary>
        private BaseNode _curSelectedNode;

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

        protected override void Init()
        {
            Transform menuBarTf = _view.transform.Find("MenuBar");
            _fileToggleTf = menuBarTf.Find("File") as RectTransform;
            _fileToggle = _fileToggleTf.gameObject;
            _editToggle = menuBarTf.Find("Edit").gameObject;
            _helpToggle = menuBarTf.Find("Help").gameObject;

            InitProjectShortcutBar();

            InitNodeInfoPanel();
            InitProjectPanel();

            AddListeners();
        }

        #region projectShortcut
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

            // 在之后插入
            _projectShortcutInsertAfterGo = tf.Find("InsertModeToggleGroup/InsertAfter/SelectImg").gameObject;
            UIEventListener.Get(_projectShortcutInsertAfterGo).AddPointerEnter(OnProjectShortcutInsertAfterPointerEnter);
            UIEventListener.Get(_projectShortcutInsertAfterGo).AddPointerExit(OnProjectShortcutPointerExit);
            // 在之前插入
            _projectShortcutInsertBeforeGo = tf.Find("InsertModeToggleGroup/InsertBefore/SelectImg").gameObject;
            UIEventListener.Get(_projectShortcutInsertBeforeGo).AddPointerEnter(OnProjectShortcutInsertBeforePointerEnter);
            UIEventListener.Get(_projectShortcutInsertBeforeGo).AddPointerExit(OnProjectShortcutPointerExit);
            // 作为子节点插入
            _projectShortcutInsertAsChildGo = tf.Find("InsertModeToggleGroup/InsertAsChild/SelectImg").gameObject;
            UIEventListener.Get(_projectShortcutInsertAsChildGo).AddPointerEnter(OnProjectShortcutInsertAsChildPointerEnter);
            UIEventListener.Get(_projectShortcutInsertAsChildGo).AddPointerExit(OnProjectShortcutPointerExit);
        }

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

        private void InitNodeInfoPanel()
        {
            _nodeInfoPanelTf = _viewTf.Find("NodeInfoPanel").GetComponent<RectTransform>();
            _nodeAttrItems = new List<NodeAttrItem>();
            _nodeNameText = _viewTf.Find("NodeInfoPanel/NodeTypeTextItem/NodeTypeText").GetComponent<Text>();

            ResetNodeAttrInfo();
        }

        private void InitProjectPanel()
        {
            _projectContentTf = _viewTf.Find("ProjectPanel/Scroll View/Viewport/Content").GetComponent<RectTransform>();
            _curSelectedNode = null;
            EventManager.GetInstance().Register(EditorEvents.NodeSelected, this);
            TestProjectNodeItems();
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

        private void UpdateAttrPanel()
        {

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

        public void Execute(int eventId, object data)
        {
            switch ( eventId )
            {
                case EditorEvents.NodeSelected:
                    OnNodeSelected(data as BaseNode);
                    break;
            }
        }

        private void OnNodeSelected(BaseNode node)
        {
            if (node == _curSelectedNode) return;
            if ( _curSelectedNode != null )
            {
                ResetNodeAttrInfo();
                _curSelectedNode.OnSelected(false);
            }
            _curSelectedNode = node;
            UpdateNodeNameItem();
            UpdateNodeAttrItems();
        }

        private void ResetNodeAttrInfo()
        {
            // 节点解除绑定
            if ( _curSelectedNode != null )
            {
                List<BaseNodeAttr> _attributeList = _curSelectedNode.GetAttrs();
                for (int i=0;i<_attributeList.Count;i++)
                {
                    _attributeList[i].UnbindItem();
                }
            }
            _nodeNameText.text = "please select a node to show attributes";
            for (int i=0;i<_nodeAttrItems.Count;i++)
            {
                NodeAttrItem item = _nodeAttrItems[i];
                item.itemGo.SetActive(false);
                item.dropdown.options.Clear();
                item.dropdownArrowGo.SetActive(true);
            }
        }

        private void UpdateNodeNameItem()
        {
            _nodeNameText.text = "Node type: " + _curSelectedNode.GetNodeName();
        }

        private void UpdateNodeAttrItems()
        {
            List<BaseNodeAttr> _attributeList = _curSelectedNode.GetAttrs();
            int attrCount = _attributeList.Count;
            int i;
            BaseNodeAttr attribute;
            if ( attrCount > _nodeAttrItems.Count )
            {
                for (i = _nodeAttrItems.Count; i < attrCount; i++)
                {
                    NodeAttrItem item = CreateNodeAttrItem();
                    _nodeAttrItems.Add(item);
                }
            }
            for (i = 0; i < attrCount; i++)
            {
                attribute = _attributeList[i];
                _nodeAttrItems[i].itemGo.SetActive(true);
                attribute.BindItem(_nodeAttrItems[i].itemGo);
            }
        }

        private NodeAttrItem CreateNodeAttrItem()
        {
            GameObject itemGo = ResourceManager.GetInstance().GetPrefab("Prefabs/Views", "MainView/NodeAttributeItem");
            RectTransform tf = itemGo.GetComponent<RectTransform>();
            tf.SetParent(_nodeInfoPanelTf, false);
            NodeAttrItem item = new NodeAttrItem();
            item.itemGo = itemGo;
            item.attrNameText = tf.Find("AttrNameText").GetComponent<Text>();
            item.valueText = tf.Find("Dropdown/Label").GetComponent<InputField>();
            item.dropdownArrowGo = tf.Find("Dropdown/Arrow").gameObject;
            item.dropdown = tf.Find("Dropdown").GetComponent<Dropdown>();
            item.editBtn = tf.Find("EditBtn").gameObject;
            return item;
        }
    }
}
