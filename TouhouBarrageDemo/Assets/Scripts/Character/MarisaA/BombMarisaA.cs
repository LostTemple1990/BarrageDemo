using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombMarisaA : BombBase
{
    private const int MasterSparkPosZ = -5;
    public const int MovementObjectCount = 5;
    /// <summary>
    /// 魔炮宽度
    /// </summary>
    private const float MasterSparkWidth = 128;
    /// <summary>
    /// 魔炮高度
    /// </summary>
    private const float MasterSparkHeight = 256;
    /// <summary>
    /// 魔炮本体相对于原图的缩放尺寸
    /// </summary>
    private const float MasterSparkScale = 2.5f;
    /// <summary>
    /// 魔炮判定盒的宽度
    /// </summary>
    private const float MasterSparkHitBoxWidth = MasterSparkWidth * MasterSparkScale * 0.95f;
    /// <summary>
    /// 魔炮判定盒的高度
    /// </summary>
    private const float MasterSpartHitBoxHeight = MasterSparkHeight * MasterSparkScale;

    private const float MasterSparkWaveSpeedY = 18f;

    /// <summary>
    /// 魔炮位置与自机位置的偏移量
    /// </summary>
    private Vector3 _posOffset;
    private GameObject _masterSparkObject;
    private Transform _masterSparkTf;
    /// <summary>
    /// 魔炮主体的容器Panel
    /// </summary>
    private Transform _sparkContainerTf;
    private Transform _movementBodyTf;
    private List<Transform> _movementObjectTfList;
    private List<MovementObjectData> _movementDataList;
    /// <summary>
    /// 移动的对象的数量
    /// </summary>
    private int _movementCount;

    struct MovementObjectData
    {
        public Transform tf;
        public int state;
        public int time;
        public int duration;
    };

    private int _curState;
    private int _time;
    private int _duration;
    private float _curScaleX;
    /// <summary>
    /// 下一个需要移动的环
    /// </summary>
    private int _nextMovementIndex;
    /// <summary>
    /// 魔炮时候的黑底SpriteEffect
    /// </summary>
    private STGSpriteEffect _effect;
    /// <summary>
    /// 魔炮的碰撞矩形
    /// </summary>
    private ColliderRect _colliderRect;

    public BombMarisaA()
    {
        _posOffset = new Vector3(0, 200, 0);
        _movementObjectTfList = new List<Transform>();
        _movementDataList = new List<MovementObjectData>();
    }

    public override void Start()
    {
        _masterSparkObject = ResourceManager.GetInstance().GetPrefab("Prefab/Character/MarisaA", "MasterSpark");
        UIManager.GetInstance().AddGoToLayer(_masterSparkObject, LayerId.PlayerBarage);
        _masterSparkTf = _masterSparkObject.transform;
        Vector2 playerPos = Global.PlayerPos;
        Vector3 pos = new Vector3(playerPos.x + _posOffset.x, playerPos.y + _posOffset.y, MasterSparkPosZ);
        _masterSparkTf.localPosition = pos;
        Transform tf;
        for (int i=0;i< MovementObjectCount; i++)
        {
            tf = _masterSparkTf.Find("SparkWave" + i);
            _movementObjectTfList.Add(tf);
        }
        _sparkContainerTf = _masterSparkTf.Find("SparkContainer");
        _movementBodyTf = _sparkContainerTf.Find("MovementBody");
        // 黑底
        _effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as STGSpriteEffect;
        _effect.SetSprite("STGCommonAtlas", "Circle",eBlendMode.Normal, LayerId.STGBottomEffect, false);
        _effect.SetScale(0, 0);
        _effect.SetSpriteColor(0, 0, 0, 1);
        _effect.ChangeWidthTo(512, 30, InterpolationMode.EaseInQuad);
        _effect.ChangeHeightTo(512, 30, InterpolationMode.EaseInQuad);
        _effect.SetPosition(playerPos.x, playerPos.y);
        // 基础属性
        _curState = 1;
        _time = 0;
        _duration = 30;
        _isFinish = false;
    }

    public override void Update()
    {
        // 魔炮跟随自机移动
        Vector2 playerPos = Global.PlayerPos;
        Vector3 pos = new Vector3(playerPos.x + _posOffset.x, playerPos.y + _posOffset.y, MasterSparkPosZ);
        _masterSparkTf.localPosition = pos;
        if ( true )
        {
            float posY = _movementBodyTf.localPosition.y;
            posY += Random.Range(-1f, 1f);
            posY = Mathf.Clamp(posY, 0, 20);
            _movementBodyTf.localPosition = new Vector3(0, posY, -1);
        }
        // 更新状态
        if (_curState == 1)
        {
            UpdateState1();
        }
        else if (_curState == 2)
        {
            UpdateState2();
        }
        else if ( _curState == 3 )
        {
            UpdateState3();
        }
        else
        {
            _isFinish = true;
        }
    }

    /// <summary>
    /// 魔炮张开的阶段
    /// </summary>
    public void UpdateState1()
    {
        _curScaleX = MathUtil.GetEaseInQuadInterpolation(0, 1, _time, _duration);
        _sparkContainerTf.localScale = new Vector3(_curScaleX, 1, 1);
        if (_time >= _duration)
        {
            _curState = 2;
            _nextMovementIndex = 0;
            _time = 0;
            // 设置碰撞
            _colliderRect = ColliderManager.GetInstance().CreateColliderByType(eColliderType.Rect) as ColliderRect;
            _colliderRect.SetSize(MasterSparkHitBoxWidth, MasterSpartHitBoxHeight);
            _colliderRect.SetColliderGroup(eColliderGroup.EnemyBullet | eColliderGroup.Enemy | eColliderGroup.Boss);
            _colliderRect.SetEliminateType(eEliminateDef.PlayerSpellCard);
            _colliderRect.SetHitEnemyDamage(3);
            _colliderRect.SetPosition(Global.PlayerPos.x + _posOffset.x, Global.PlayerPos.y + _posOffset.y);
            _duration = 240;
            SoundManager.GetInstance().Play("se_masterspark", 0.1f, false, true);
        }
        _time++;
    }

    /// <summary>
    /// 魔炮持续时间
    /// </summary>
    public void UpdateState2()
    {
        // 每10帧一个动画
        if ( _time % 10 == 0 )
        {
            MovementObjectData data = new MovementObjectData();
            data.tf = _movementObjectTfList[_nextMovementIndex];
            data.tf.localPosition = new Vector3(0, -336, -2);
            data.state = 1;
            data.time = 0;
            data.duration = 12;
            _movementDataList.Add(data);
            _movementCount++;
            // 计算下一个需要取的下标
            _nextMovementIndex = (_nextMovementIndex + 1) % MovementObjectCount;
        }
        UpdateSparkWaves();
        // 更新物体碰撞器的位置
        _colliderRect.SetPosition(Global.PlayerPos.x + _posOffset.x, Global.PlayerPos.y + _posOffset.y);
        if ( _time >= _duration )
        {
            // 黑底Sprite消失
            _effect.DoFade(20);
            _curState = 3;
            _time = 0;
            _duration = 30;
            _colliderRect.ClearSelf();
        }
        _time++;
    }

    private void UpdateSparkWaves()
    {
        float scale;
        Transform tf;
        Vector3 pos;
        for (int i = 0; i < _movementCount; i++)
        {
            MovementObjectData data = _movementDataList[i];
            tf = data.tf;
            if (data.state == 1)
            {
                //scale = data.time <= 15 ? 1.5f : (float)data.time / 10;
                //scale *= 100;
                scale = Mathf.Lerp(50, 150, (float)data.time / data.duration);
                tf.localScale = new Vector3(scale, scale, 1);
                if (data.time >= data.duration)
                {
                    data.state = 2;
                }
            }
            pos = tf.localPosition;
            pos.y += MasterSparkWaveSpeedY;
            if (pos.y >= 300)
            {
                // 将scale设置成0当做是隐藏
                tf.localScale = new Vector3(0, 0, 1);
                //  清空
                data.tf = null;
                _movementDataList.RemoveAt(i);
                _movementCount--;
                i--;
            }
            else
            {
                tf.localPosition = pos;
                data.time++;
                _movementDataList[i] = data;
            }
        }
    }

    public void UpdateState3()
    {
        _curScaleX = MathUtil.GetEaseInQuadInterpolation(1, 0, _time, _duration);
        _sparkContainerTf.localScale = new Vector3(_curScaleX, 1, 1);
        UpdateSparkWaves();
        if (_time >= _duration)
        {
            _curState = 4;
        }
        _time++;
    }

    public override void Clear()
    {
        _effect = null;
        _movementObjectTfList.Clear();
        _movementDataList.Clear();
        GameObject.Destroy(_masterSparkObject);
        _masterSparkObject = null;
        _masterSparkTf = null;
        _sparkContainerTf = null;
        _movementBodyTf = null;
        _movementCount = 0;
        _colliderRect = null;
    }
}
