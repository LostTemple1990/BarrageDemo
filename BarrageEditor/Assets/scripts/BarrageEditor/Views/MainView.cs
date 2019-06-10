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

        private RectTransform _projectScrollViewTf;
        private RectTransform _projectContentTf;
        /// <summary>
        /// project节点信息面板最小高度
        /// </summary>
        private const float ProjectContentMinHeight = 521;
        /// <summary>
        /// project面板当前高度
        /// </summary>
        private float _projectContentCurHeight;
        /// <summary>
        /// 节点详细信息的面板
        /// </summary>
        private RectTransform _nodeInfoPanelTf;
        private Scrollbar _projectContentScrollbar;

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
        /// <summary>
        /// 当前插入节点的方式
        /// </summary>
        private NodeInsertMode _curInsertMode;

        protected override void Init()
        {
            Transform menuBarTf = _view.transform.Find("MenuBar");
            _fileToggleTf = menuBarTf.Find("File") as RectTransform;
            _fileToggle = _fileToggleTf.gameObject;
            _editToggle = menuBarTf.Find("Edit").gameObject;
            _helpToggle = menuBarTf.Find("Help").gameObject;

            InitProjectShortcutBar();
            InitNodeShortcutBar();

            InitNodeInfoPanel();
            InitProjectPanel();

            AddListeners();
        }

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

        #region nodeShortcuts
        private GameObject _nodeShortcutTabGeneral;
        private GameObject _nodeShortcutTabStage;
        private GameObject _nodeShortcutTabTask;
        private GameObject _nodeShortcutTabEnemy;
        private GameObject _nodeShortcutTabBoss;
        private GameObject _nodeShortcutTabBullet;
        private GameObject _nodeShortcutTabTools;
        private GameObject _nodeShortcutTabLaser;

        /// <summary>
        /// 节点快捷图标的容器
        /// </summary>
        private RectTransform _nodeShortcutsContainerTf;
        /// <summary>
        /// 当前选中的节点tab中拥有的所有节点gameObject
        /// </summary>
        private List<GameObject> _nodeShortcutList; 

        enum NodeShortcutTab
        {
            General = 0,
            Stage = 1,
            Task = 2,
            Enemy = 3,
            Boss = 4,
            Bullet = 5,
            Count = 6,
        };

        private Dictionary<NodeShortcutTab, List<NodeType>> _nodeTabShortcutDic;

        private void InitNodeShortcutBar()
        {
            // 每个tab拥有的节点信息
            _nodeTabShortcutDic = new Dictionary<NodeShortcutTab, List<NodeType>>();
            List<NodeType> typeList;
            // general
            typeList = new List<NodeType> { NodeType.Folder, NodeType.CodeBlock };
            _nodeTabShortcutDic.Add(NodeShortcutTab.General, typeList);
            //Bullet
            typeList = new List<NodeType> { NodeType.DefineBullet };
            _nodeTabShortcutDic.Add(NodeShortcutTab.Bullet, typeList);

            RectTransform barTf = _viewTf.Find("NodeShortcutBar").GetComponent<RectTransform>();

            _nodeShortcutsContainerTf = barTf.Find("NodeShortcuts").GetComponent<RectTransform>();
            _nodeShortcutList = new List<GameObject>();

            _nodeShortcutTabGeneral = barTf.Find("Gernal").gameObject;
            UIEventListener.Get(_nodeShortcutTabGeneral).AddClick(OnTabGeneralClickHandler);

            _nodeShortcutTabStage = barTf.Find("Stage").gameObject;
            UIEventListener.Get(_nodeShortcutTabStage).AddClick(OnTabStageClickHandler);

            _nodeShortcutTabBullet = barTf.Find("Bullet").gameObject;
            UIEventListener.Get(_nodeShortcutTabBullet).AddClick(OnTabBulletClickHandler);

            // 默认选中general
            _nodeShortcutTabGeneral.GetComponent<Toggle>().isOn = true;
            OnTabGeneralClickHandler();
        }

        private void OnTabGeneralClickHandler()
        {
            AddShortcutsByTab(NodeShortcutTab.General);
        }

        private void OnTabStageClickHandler()
        {
            AddShortcutsByTab(NodeShortcutTab.Stage);
        }

        private void OnTabBulletClickHandler()
        {
            AddShortcutsByTab(NodeShortcutTab.Bullet);
        }

        private void AddShortcutsByTab(NodeShortcutTab tab)
        {
            RemoveAllNodeShortcuts();
            List<NodeType> typeList;
            if (!_nodeTabShortcutDic.TryGetValue(tab, out typeList))
            {
                return;
            }
            for (int i = 0; i < typeList.Count; i++)
            {
                NodeConfig nodeCfg = DatabaseManager.NodeDatabase.GetNodeCfgByNodeType(typeList[i]);
                if (nodeCfg == null)
                {
                    Logger.LogError("Node Config is not exist!! NodeType = " + typeList[i]);
                }
                GameObject shortcutGo = ResourceManager.GetInstance().GetPrefab("Prefabs/Views", "MainView/NodeShortcut");
                RectTransform shortcutTf = shortcutGo.GetComponent<RectTransform>();
                shortcutTf.SetParent(_nodeShortcutsContainerTf, false);
                GameObject btnGo = shortcutTf.Find("BgBtn").gameObject;
                // 添加监听事件
                UIEventListener.Get(btnGo).AddPointerEnter(() =>
                {
                    UIManager.GetInstance().OpenView(ViewID.TooltipView, nodeCfg.shortcutTip);
                });
                UIEventListener.Get(btnGo).AddPointerExit(() =>
                {
                    UIManager.GetInstance().CloseView(ViewID.TooltipView);
                });
                UIEventListener.Get(btnGo).AddClick(() =>
                {
                    TryInsertNode(nodeCfg.type);
                });
                Image img = shortcutTf.Find("Image").GetComponent<Image>();
                img.sprite = ResourceManager.GetInstance().GetSprite("NodeShortcutsAtlas", nodeCfg.shortcutPath);
                // 添加到列表中
                _nodeShortcutList.Add(shortcutGo);
            }
        }

        private void TryInsertNode(NodeType nodeType)
        {
            if ( _curInsertMode == NodeInsertMode.InsertAfter )
            {
                BaseNode parentNode = _curSelectedNode.parentNode;
                if (parentNode != null)
                {
                    BaseNode node = NodeManager.CreateNode(nodeType);
                    node.SetAttrsDefaultValues();
                    int index = parentNode.GetChildIndex(_curSelectedNode);
                    parentNode.InsertChildNode(node, index + 1);
                    parentNode.Expand(true);
                    node.OnSelected(true);
                    FocusOnNode(node);
                }
            }
            else if (_curInsertMode == NodeInsertMode.InsertBefore)
            {
                BaseNode parentNode = _curSelectedNode.parentNode;
                if (parentNode != null)
                {
                    BaseNode node = NodeManager.CreateNode(nodeType);
                    node.SetAttrsDefaultValues();
                    int index = parentNode.GetChildIndex(_curSelectedNode);
                    parentNode.InsertChildNode(node, index);
                    parentNode.Expand(true);
                    node.OnSelected(true);
                    FocusOnNode(node);
                }
            }
            else if (_curInsertMode == NodeInsertMode.InsertAsChild)
            {
                BaseNode node = NodeManager.CreateNode(nodeType);
                node.SetAttrsDefaultValues();
                _curSelectedNode.InsertChildNode(node, -1);
                _curSelectedNode.Expand(true);
                node.OnSelected(true);
                FocusOnNode(node);
            }
        }

        private void RemoveAllNodeShortcuts()
        {
            GameObject item;
            for (int i=0;i<_nodeShortcutList.Count;i++)
            {
                item = _nodeShortcutList[i];
                UIEventListener.Get(item.transform.Find("BgBtn").gameObject).RemoveAllEvents();
                GameObject.Destroy(item);
            }
            _nodeShortcutList.Clear();
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
            _projectScrollViewTf = _viewTf.Find("ProjectPanel/Scroll View").GetComponent<RectTransform>();
            _projectContentTf = _projectScrollViewTf.Find("Viewport/Content").GetComponent<RectTransform>();
            _projectContentScrollbar = _projectScrollViewTf.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
            _curSelectedNode = null;
            EventManager.GetInstance().Register(EditorEvents.NodeSelected, this);
            EventManager.GetInstance().Register(EditorEvents.NodeExpanded, this);
            //EventManager.GetInstance().Register(EditorEvents.FocusOnNode, this);
            NodeManager.SetNodeItemContainer(_projectContentTf);
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
            root.Init( _projectContentTf);
            root.SetParent(null);
            root.SetAttrsDefaultValues();
            root.UpdateDesc();
            NodeProjectSettings ps = new NodeProjectSettings();
            ps.Init(_projectContentTf);
            ps.SetAttrsDefaultValues();
            ps.UpdateDesc();
            root.InsertChildNode(ps, -1);
            root.Expand(true);
        }

        /// <summary>
        /// 将视角切换到node处
        /// </summary>
        /// <param name="node"></param>
        private void FocusOnNode(BaseNode node)
        {
            int showIndex = node.GetShowIndex();
            float toY = showIndex * BaseNode.NodeHeight;
            float max = _projectContentCurHeight - ProjectContentMinHeight;
            float toValue = 1;
            // 当前内容没有填满content，
            if (max <= 0)
            {
                toValue = 1;
            }
            // 节点之下的内容不足整个content的高度，则默认拉到最底下
            else if (toY > max)
            {
                toValue = 0;
            }
            else
            {
                toValue = 1 - toY / max;
            }
            // 根据content的最大高度进行校正
            //Logger.Log("posY = " + toY + " max = " + max);
            //Logger.Log("toValue = " + toValue);
            _projectContentScrollbar.value = toValue;
            //_projectContentTf.anchoredPosition = new Vector2(0, toY);
        }

        public void Execute(int eventId, object data)
        {
            switch ( eventId )
            {
                case EditorEvents.NodeSelected:
                    OnNodeSelected(data as BaseNode);
                    break;
                case EditorEvents.NodeExpanded:
                    OnNodeExpanded((int)data);
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

        private void OnNodeExpanded(int lastIndex)
        {
            // index是从0开始计算的，因此算高度的时候要+1
            float preferredHeight = (lastIndex + 1) * BaseNode.NodeHeight;
            if (preferredHeight < ProjectContentMinHeight)
            {
                preferredHeight = ProjectContentMinHeight;
            }
            _projectContentCurHeight = preferredHeight;
            _projectContentTf.sizeDelta = new Vector2(0, preferredHeight);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_projectScrollViewTf);
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
