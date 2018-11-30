using UnityEngine;
using System.Collections;

public interface IAttachment
{
    /// <summary>
    /// 附加到对应的物体上
    /// </summary>
    /// <param name="attachableObject"></param>
    /// <param name="elimnatedWithMaster">是否跟随本体一同销毁</param>
    void AttachTo(IAttachable master,bool eliminatedWithMaster);
    /// <summary>
    /// 设置相对坐标
    /// </summary>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    /// <param name="rotation"></param>
    /// <param name="followMasterRotation"></param>
    /// <param name="isFollowingMasterContinuously">是否持续跟随本体一起移动</param>
    void SetRelativePos(float offsetX, float offsetY, float rotation, bool followMasterRotation,bool isFollowingMasterContinuously);
    /// <summary>
    /// 当被绑定的物体被消除时调用
    /// </summary>
    void OnMasterEliminated(eEliminateDef eliminateType);
}