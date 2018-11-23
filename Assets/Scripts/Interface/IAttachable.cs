using UnityEngine;
using System.Collections;


public interface IAttachable : IPosition
{
    void AddAttachment(IAttachment attachment);
    /// <summary>
    /// 当被依附的物体被销毁时调用
    /// </summary>
    /// <param name="attachment"></param>
    void OnAttachmentEliminated(IAttachment attachment);
}
