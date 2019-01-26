using System.Collections.Generic;

public class STGReplayData
{
    /// <summary>
    /// 机签
    /// </summary>
    public string sign;
    /// <summary>
    /// 难度
    /// </summary>
    public int difficultMode;
    /// <summary>
    /// 具体的录像信息
    /// replayData[0] 为replay长度
    /// </summary>
    public List<List<int>> replayData;

    public STGReplayData()
    {
        replayData = new List<List<int>>();
        replayData[0] = new List<int>();
    }
}
