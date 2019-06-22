using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public class EditorEvents
    {
        /// <summary>
        /// 显示提示文本
        /// </summary>
        public const int ShowTooltip = 1001;
        public const int Log = 1011;
        public const int LogWarning = 1012;
        public const int LogError = 1013;
        /// <summary>
        /// 工程文件改变
        /// </summary>
        public const int BeforeProjectChanged = 2001;
        public const int AfterProjectChanged = 2002;
        /// <summary>
        /// 选择了一个节点
        /// <para>param1 : BaseNode</para>
        /// </summary>
        public const int NodeSelected = 3001;
        /// <summary>
        /// 展开/收起一个节点
        /// </summary>
        public const int NodeExpanded = 3002;
        public const int FocusOnNode = 3003;
    }
}
