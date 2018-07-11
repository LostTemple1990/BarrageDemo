using UnityEngine;
using System.Collections;

public class SpellCard
{
    public int type;
    public string name;
    public float curHp;
    public float maxHp;
    /// <summary>
    /// 符卡时间，单位：秒
    /// </summary>
    public float spellDuration;
    public float spellTime;
    public int taskRef;
    public float invincibleDuration;
    public bool isFinish;
    public int finishFuncRef;

    public SpellCard()
    {
        finishFuncRef = 0;
    }
}
