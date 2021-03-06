﻿using UniLua;
using UnityEngine;

public class BossLuaInterface
{
    private static bool _isInit = false;

    private static LuaCsClosureValue _funcSetV;
    private static LuaCsClosureValue _funcSetAcce;
    private static LuaCsClosureValue _funcSetPolarParas;

    private static LuaCsClosureValue _funcMoveTo;
    private static LuaCsClosureValue _funcMoveTowards;
    private static LuaCsClosureValue _funcSetPos;
    private static LuaCsClosureValue _funcGetPos;

    private static LuaCsClosureValue _funcAttachTo;
    private static LuaCsClosureValue _funcSetRelativePos;

    private static LuaCsClosureValue _funcAddTask;

    private static LuaCsClosureValue _funcSetMaxHp;
    private static LuaCsClosureValue _funcSetCollisionSize;
    private static LuaCsClosureValue _funcSetAni;
    private static LuaCsClosureValue _funcSetWanderRange;
    private static LuaCsClosureValue _funcSetWanderAmplitude;
    private static LuaCsClosureValue _funcSetWanderMode;
    private static LuaCsClosureValue _funcWander;

    private static LuaCsClosureValue _funcSetInvincible;
    private static LuaCsClosureValue _funcSetInteractive;
    private static LuaCsClosureValue _funcPlayAni;

    private static LuaCsClosureValue _funcShowBloodBar;
    private static LuaCsClosureValue _funcSetPhaseData;
    private static LuaCsClosureValue _funcShowPosHint;
    private static LuaCsClosureValue _funcShowAura;
    private static LuaCsClosureValue _funcShowSCHpAura;

    public static void Init()
    {
        if (!_isInit)
        {
            _funcSetV = new LuaCsClosureValue(LuaLib.STGMovableDoStraightMove);
            _funcSetAcce = new LuaCsClosureValue(LuaLib.STGMovableDoAcceleration);
            _funcSetPolarParas = new LuaCsClosureValue(LuaLib.STGMovableDoCurvedMove);
            _funcMoveTo = new LuaCsClosureValue(LuaLib.STGMovableMoveTo);
            _funcMoveTowards = new LuaCsClosureValue(LuaLib.STGMovableMoveTowards);
            _funcSetPos = new LuaCsClosureValue(LuaLib.IPosition_SetPosition);
            _funcGetPos = new LuaCsClosureValue(LuaLib.IPosition_GetPosition);

            _funcAttachTo = new LuaCsClosureValue(LuaLib.AttachToMaster);
            _funcSetRelativePos = new LuaCsClosureValue(LuaLib.SetAttachmentRelativePos);

            _funcAddTask = new LuaCsClosureValue(LuaLib.AddTask);

            _funcSetMaxHp = new LuaCsClosureValue(LuaLib.SetEnemyMaxHp);
            _funcSetCollisionSize = new LuaCsClosureValue(LuaLib.SetEnemyCollisionParas);
            _funcSetAni = new LuaCsClosureValue(LuaLib.SetBossAni);
            _funcSetWanderRange = new LuaCsClosureValue(LuaLib.SetEnemyWanderRange);
            _funcSetWanderAmplitude = new LuaCsClosureValue(LuaLib.SetEnemyWanderAmplitude);
            _funcSetWanderMode = new LuaCsClosureValue(LuaLib.SetEnemyWanderMode);
            _funcWander = new LuaCsClosureValue(LuaLib.EnemyDoWander);
            _funcPlayAni = new LuaCsClosureValue(LuaLib.PlayEnemyAni);

            _funcSetInvincible = new LuaCsClosureValue(LuaLib.SetEnemyInvincible);
            _funcSetInteractive = new LuaCsClosureValue(LuaLib.SetEnemyInteractive);
            _funcShowBloodBar = new LuaCsClosureValue(LuaLib.ShowBossBloodBar);
            _funcSetPhaseData = new LuaCsClosureValue(LuaLib.SetBossCurPhaseData);
            _funcShowPosHint = new LuaCsClosureValue(LuaLib.ShowBossPosHint);
            _funcShowAura = new LuaCsClosureValue(LuaLib.ShowBossAura);
            _funcShowSCHpAura = new LuaCsClosureValue(LuaLib.ShowBossSpellCardHpAura);

            _isInit = true;
        }
    }

    public static bool Get(object o, TValue key, out TValue res)
    {
        Boss enemy = (Boss)o;
        res = new TValue();
        if (key.TtIsString())
        {
            switch (key.SValue())
            {
                #region 基础变量
                case "x":
                    {
                        res.SetNValue(enemy.GetPosition().x);
                        return true;
                    }
                case "y":
                    {
                        res.SetNValue(enemy.GetPosition().y);
                        return true;
                    }
                case "rot":
                    {
                        res.SetNValue(enemy.GetRotation());
                        return true;
                    }
                case "dx":
                    {
                        res.SetNValue(enemy.dx);
                        return true;
                    }
                case "dy":
                    {
                        res.SetNValue(enemy.dy);
                        return true;
                    }
                case "SetPos":
                    {
                        res.SetClCsValue(_funcSetPos);
                        return true;
                    }
                case "GetPos":
                    {
                        res.SetClCsValue(_funcGetPos);
                        return true;
                    }
                #endregion
                #region 运动相关变量
                case "v":
                    {
                        res.SetNValue(enemy.velocity);
                        return true;
                    }
                case "vx":
                    {
                        res.SetNValue(enemy.vx);
                        return true;
                    }
                case "vy":
                    {
                        res.SetNValue(enemy.vy);
                        return true;
                    }
                case "vAngle":
                    {
                        res.SetNValue(enemy.vAngle);
                        return true;
                    }
                case "maxV":
                    {
                        res.SetNValue(enemy.maxVelocity);
                        return true;
                    }
                case "acce":
                    {
                        res.SetNValue(enemy.acce);
                        return true;
                    }
                case "accAngle":
                    {
                        res.SetNValue(enemy.accAngle);
                        return true;
                    }
                #endregion
                #region 运动类专属方法 ISTGMovable
                case "SetV":
                    {
                        res.SetClCsValue(_funcSetV);
                        return true;
                    }
                case "SetAcce":
                    {
                        res.SetClCsValue(_funcSetAcce);
                        return true;
                    }
                case "SetPolarParas":
                    {
                        res.SetClCsValue(_funcSetPolarParas);
                        return true;
                    }
                case "MoveTo":
                    {
                        res.SetClCsValue(_funcMoveTo);
                        return true;
                    }
                case "MoveTowards":
                    {
                        res.SetClCsValue(_funcMoveTowards);
                        return true;
                    }
                #endregion
                #region IAttachable
                case "AttachTo":
                    {
                        res.SetClCsValue(_funcAttachTo);
                        return true;
                    }
                case "SetRelativePos":
                    {
                        res.SetClCsValue(_funcSetRelativePos);
                        return true;
                    }
                #endregion
                #region ITaskExecuter
                case "AddTask":
                    {
                        res.SetClCsValue(_funcAddTask);
                        return true;
                    }
                #endregion
                #region 敌机专属变量
                case "SetMaxHp":
                    res.SetClCsValue(_funcSetMaxHp);
                    return true;
                case "SetCollisionSize":
                    res.SetClCsValue(_funcSetCollisionSize);
                    return true;
                case "SetAni":
                    res.SetClCsValue(_funcSetAni);
                    return true;
                case "SetWanderRange":
                    res.SetClCsValue(_funcSetWanderRange);
                    return true;
                case "SetWanderAmplitude":
                    res.SetClCsValue(_funcSetWanderAmplitude);
                    return true;
                case "SetWanderMode":
                    res.SetClCsValue(_funcSetWanderMode);
                    return true;
                case "Wander":
                    res.SetClCsValue(_funcWander);
                    return true;
                case "SetInteractive":
                    res.SetClCsValue(_funcSetInteractive);
                    return true;
                case "PlayAni":
                    res.SetClCsValue(_funcPlayAni);
                    return true;
                #endregion
                #region Boss专属变量
                case "SetInvincible":
                    res.SetClCsValue(_funcSetInvincible);
                    return true;
                case "ShowBloodBar":
                    res.SetClCsValue(_funcShowBloodBar);
                    return true;
                case "SetPhaseData":
                    res.SetClCsValue(_funcSetPhaseData);
                    return true;
                case "ShowPosHint":
                    res.SetClCsValue(_funcShowPosHint);
                    return true;
                case "ShowAura":
                    res.SetClCsValue(_funcShowAura);
                    return true;
                case "ShowSpellCardHpAura":
                    res.SetClCsValue(_funcShowSCHpAura);
                    return true;
                    #endregion

            }
        }
        res.SetSValue(string.Format("GetField from userData fail!Invalid key {0} for type {1}", key, typeof(Boss).Name));
        return false;
    }

    public static bool Set(object o, TValue key, ref TValue value)
    {
        Boss enemy = (Boss)o;
        if (key.TtIsString())
        {
            switch (key.SValue())
            {
                #region 基础变量
                case "x":
                    {
                        Vector2 pos = enemy.GetPosition();
                        pos.x = (float)value.NValue;
                        enemy.SetPosition(pos);
                        return true;
                    }
                case "y":
                    {
                        Vector2 pos = enemy.GetPosition();
                        pos.y = (float)value.NValue;
                        enemy.SetPosition(pos);
                        return true;
                    }
                case "rot":
                    {
                        enemy.SetRotation((float)value.NValue);
                        return true;
                    }
                #endregion
                #region 运动相关变量
                case "v":
                    {
                        enemy.velocity = (float)value.NValue;
                        return true;
                    }
                case "vx":
                    {
                        enemy.vx = (float)value.NValue;
                        return true;
                    }
                case "vy":
                    {
                        enemy.vy = (float)value.NValue;
                        return true;
                    }
                case "vAngle":
                    {
                        enemy.vAngle = (float)value.NValue;
                        return true;
                    }
                case "maxV":
                    {
                        enemy.maxVelocity = (float)value.NValue;
                        return true;
                    }
                case "acce":
                    {
                        enemy.acce = (float)value.NValue;
                        return true;
                    }
                case "accAngle":
                    {
                        enemy.accAngle = (float)value.NValue;
                        return true;
                    }
                    #endregion
            }
        }
        value.SetSValue(string.Format("SetField of userData fail!Invalid key {0} for type {1}", key, typeof(Boss).Name));
        return false;
    }
}