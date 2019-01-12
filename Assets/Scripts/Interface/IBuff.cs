using UnityEngine;
using System.Collections;

public interface IBuff
{
    /// <summary>
    /// 总持续时间
    /// </summary>
    /// <returns></returns>
    int GetTotalDuration();
    /// <summary>
    /// 该buff被添加到目标身上的时间
    /// </summary>
    /// <returns></returns>
    int GetTimeSinceAdded();
    /// <summary>
    /// 判断该buff当前是否还有效
    /// </summary>
    /// <returns></returns>
    bool IsValid();
    void OnLogic();
    /// <summary>
    /// 添加buff
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    bool AddTo(IBuffContainer container);
    /// <summary>
    /// 当buff被添加的时候调用
    /// </summary>
    /// <param name="container"></param>
    void OnAdded(IBuffContainer container);
    bool Remove();
    /// <summary>
    /// 获得持有该buff的对象
    /// </summary>
    /// <returns></returns>
    IBuffContainer GetOwner();
    /// <summary>
    /// 叠加层数
    /// </summary>
    /// <returns></returns>
    int GetOverlay();
    /// <summary>
    /// 设置叠加层数
    /// </summary>
    /// <returns></returns>
    int SetOverlay(int value);
}
