using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : PlayerBulletBase,ICommand
{
    /// <summary>
    /// 碰撞线段组的半径
    /// 即碰撞线段长度的一半
    /// </summary>
    private const float CollisionSegmentRadius = 5;
    /// <summary>
    /// 射线发射速度
    /// <para>即，laser的长度会以该速度拉长，直到达到最大长度</para>
    /// </summary>
    private const float LaserSpeedPerFrame = 20f;

    /// <summary>
    /// 是否已经生成贴图
    /// </summary>
    private bool _isCached;
    private Transform _laserTf;
    private SpriteRenderer _laserSr;

    private float _texWidth;
    private float _repeatCount;
    /// <summary>
    /// 射线的总长度，即贴图长度*重复次数
    /// </summary>
    private float _laserTotalLen;
    /// <summary>
    /// 当前射线长度
    /// </summary>
    private float _curLaserLen;
    /// <summary>
    /// 上一帧射线的长度，未赋值之前为-1
    /// </summary>
    private float _preLaserLen;
    /// <summary>
    /// 射线的方向向量
    /// </summary>
    private Vector2 _dirVec;
    /// <summary>
    /// 是否已经缓存了碰撞线段组
    /// </summary>
    private bool _isCachedCollisionSegments;
    /// <summary>
    /// 线段碰撞组
    /// </summary>
    private List<Vector2> _collisionSegments;
    /// <summary>
    /// 线段碰撞组的数量
    /// </summary>
    private int _collisionSegmentsCount;
    /// <summary>
    /// 射线从创建开始经过的帧数
    /// </summary>
    private int _frameCountSinceCreate;
    /// <summary>
    /// 当前是否击中某些物体
    /// </summary>
    private bool _hitObject;

    public PlayerLaser()
    {
        _type = BulletType.Player_Laser;
        _prefabName = "PlayerLaser";
        _collisionSegments = new List<Vector2>();
    }

    public override void Init()
    {
        base.Init();
        BulletsManager.GetInstance().RegisterPlayerBullet(this);
        _isCached = false;
        _isCachedCollisionSegments = false;
        _curLaserLen = 0;
        _frameCountSinceCreate = 0;
        CommandManager.GetInstance().Register(CommandConsts.STGFrameStart, this);
    }

    public override void ChangeStyleById(string id)
    {
        if (_bullet != null)
        {
            UIManager.GetInstance().HideGo(_bullet);
            ObjectsPool.GetInstance().RestorePrefabToPool(_bulletCfg.textureName, _bullet);
        }
        _bulletCfg = BulletsManager.GetInstance().GetPlayerBulletCfgById(id);
        _prefabName = id;
        _bullet = BulletsManager.GetInstance().CreateBulletGameObject(BulletType.Player_Laser, id);
        _trans = _bullet.transform;
        _laserTf = _trans.Find("Laser");
        _laserSr = _laserTf.GetComponent<SpriteRenderer>();
        _laserSr.sprite = ResourceManager.GetInstance().GetSprite(_bulletCfg.packName, _bulletCfg.textureName);
        UIManager.GetInstance().AddGoToLayer(_bullet, LayerId.PlayerBarage);
        _texWidth = 256;
        _repeatCount = 2f;
        _laserTf.localPosition = new Vector3(_texWidth * _repeatCount / 2, 0, 0);
    }

    /// <summary>
    /// 设置激光贴图的属性
    /// </summary>
    /// <param name="packName">包名</param>
    /// <param name="texName">贴图名称</param>
    /// <param name="texWidth">贴图的宽度，代表激光的长度</param>
    /// <param name="repeatCount">平铺的次数</param>
    public void SetTextureProps(string packName,string texName,float texWidth,float repeatCount)
    {

        _trans.localRotation = Quaternion.Euler(0f, 0f, 90f);
        _laserTf = _trans.Find("Laser");
        _laserSr = _laserTf.GetComponent<SpriteRenderer>();
        UIManager.GetInstance().AddGoToLayer(_bullet, LayerId.PlayerBarage);
        _laserSr.sprite = ResourceManager.GetInstance().GetSprite(packName, texName);
        _texWidth = texWidth;
        _repeatCount = repeatCount;
    }

    public void SetAngle(float angle)
    {
        _curVAngle = angle;
        // 角度
        _trans.rotation = Quaternion.Euler(0, 0, _curVAngle);
        _dirVec = new Vector2(Mathf.Cos(_curVAngle * Mathf.Deg2Rad), Mathf.Sin(_curVAngle * Mathf.Deg2Rad));
    }

    public override void Update()
    {
        if (!_isCached) Cache();
        base.Update();
        _frameCountSinceCreate++;
        if ( _detectCollision )
        {
            CalLaserLen();
        }
        RenderLaser();
        UpdatePos();
    }

    private void Cache()
    {
        _laserTotalLen = _texWidth * _repeatCount;
        _preLaserLen = -1;
        _curLaserLen = 0;
        // shader赋值
        _laserSr.material.SetFloat("_LaserTexWidth", _texWidth);
        _laserSr.material.SetFloat("_RepeatCount", _repeatCount);
        _isCached = true;
    }

    /// <summary>
    /// 计算射线的长度
    /// </summary>
    private void CalLaserLen()
    {
        List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
        int enemyCount = enemyList.Count;
        EnemyBase enemy,hitEnemy = null;
        CollisionDetectParas collDetParas;
        // verticalDis 圆心到射线的垂直距离
        // angle 两项链的夹角
        // dis 从射线起始点指向敌机中心的向量的长度
        // minDis 击中敌人的最小距离,即射线第一个击中的敌人
        float verticalDis,angle,dis,minDis,horizonDis;
        minDis = _curLaserLen;
        // vecA为射线发射点到敌机的向量
        Vector2 vecA;
        // 遍历所有敌机，检测激光最先碰到的敌机
        for (int i=0;i<enemyCount;i++)
        {
            enemy = enemyList[i];
            if ( enemy != null && enemy.CanHit() )
            {
                collDetParas = enemy.GetCollisionDetectParas();
                // 计算vecA
                vecA = collDetParas.centerPos - new Vector2(_curPos.x,_curPos.y);
                // 计算夹角
                angle = Vector2.Angle(_dirVec, vecA);
                if (angle > 90) continue;
                dis = vecA.magnitude;
                verticalDis = dis * Mathf.Sin(Mathf.Deg2Rad * angle);
                // 判断垂直距离
                if ( verticalDis <= collDetParas.halfWidth )
                {
                    horizonDis = dis * Mathf.Cos(Mathf.Deg2Rad * angle);
                    if ( horizonDis < minDis )
                    {
                        _hitObject = true;
                        hitEnemy = enemy;
                        minDis = horizonDis;
                    }
                }
            }
        }
        _curLaserLen = minDis;
        // 如果有击中敌机
        if ( _hitObject )
        {
            if ( hitEnemy != null )
            {
                hitEnemy.TakeDamage(GetDamage());
            }
            // 击中特效
            Vector2 effectPos = _curPos + _dirVec * minDis;
            STGSpriteEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as STGSpriteEffect;
            effect.SetSprite(_bulletCfg.eliminateEffectAtlas, _bulletCfg.elminaateEffectSprite, eBlendMode.Normal, LayerId.STGNormalEffect, true);
            effect.SetSpriteColor(_bulletCfg.eliminateColor.r, _bulletCfg.eliminateColor.g, _bulletCfg.eliminateColor.b, 1);
            float offsetX = Random.Range(-5, 5);
            float offsetY = Random.Range(-5, 5);
            effect.SetToPosition(effectPos.x+offsetX, effectPos.y+offsetY);
            effect.DoFade(5);
            _trans.localScale = new Vector3(1, 0.5f, 1);
        }
        else
        {
            _trans.localScale = new Vector3(1, 1, 1);
        }
    }

    private void RenderLaser()
    {
        if ( _preLaserLen != _curLaserLen )
        {
            _laserSr.material.SetFloat("_CurLaserLen", _curLaserLen);
            _preLaserLen = _curLaserLen;
        }
        _laserSr.material.SetFloat("_FrameSinceCreate", _frameCountSinceCreate);
    }

    protected override int GetDamage()
    {
        return 2;
    }

    public override bool CheckBoundingBoxesIntersect(Vector2 lbPos, Vector2 rtPos)
    {
        // 计算对角的坐标
        Vector2 diagonalPos = _curPos + _dirVec * _curLaserLen;
        float laserLBPosX = _curPos.x < diagonalPos.x ? _curPos.x : diagonalPos.x;
        float laserLBPosY = _curPos.y < diagonalPos.y ? _curPos.y : diagonalPos.y;
        float laserRTPosX = _curPos.x > diagonalPos.x ? _curPos.x : diagonalPos.x;
        float laserRTPosY = _curPos.y > diagonalPos.y ? _curPos.y : diagonalPos.y;
        // 快速排斥试验
        if ( laserLBPosX >= rtPos.x || laserLBPosY >= rtPos.y || laserRTPosX <= lbPos.x || laserRTPosY <= lbPos.y )
        {
            return false;
        }
        return true;
    }

    public override CollisionDetectParas GetCollisionDetectParas(int index = 0)
    {
        if (!_isCachedCollisionSegments) CacheCollisionSegments();
        CollisionDetectParas paras = new CollisionDetectParas();
        if ( index >= _collisionSegmentsCount )
        {
            paras.type = CollisionDetectType.Null;
            paras.nextIndex = -1;
        }
        else
        {
            int nextIndex = index + 1 >= _collisionSegmentsCount ? -1 : index + 1;
            paras.type = CollisionDetectType.Circle;
            paras.radius = CollisionSegmentRadius;
            paras.centerPos = _collisionSegments[index];
            paras.nextIndex = nextIndex;
        }
        return paras;
    }

    /// <summary>
    /// 缓存碰撞线段组
    /// </summary>
    private void CacheCollisionSegments()
    {
        _collisionSegments.Clear();
        _collisionSegmentsCount = 0;
        float tmpLen = 0;
        Vector2 centerPos;
        Vector2 startPos = _curPos + _dirVec * CollisionSegmentRadius;
        for (int i=0; ;i++)
        {
            tmpLen = (i * 2 + 1) * CollisionSegmentRadius;
            // 检测长度已经超过实际长度
            if ( tmpLen > _curLaserLen )
            {
                if ( i != 0 )
                {
                    centerPos = _curPos + (_curLaserLen - CollisionSegmentRadius) * _dirVec;
                    _collisionSegments.Add(centerPos);
                    _collisionSegmentsCount++;
                }
                break;
            }
            centerPos = _curPos + tmpLen * _dirVec;
            _collisionSegments.Add(centerPos);
            _collisionSegmentsCount++;
        }
        _isCachedCollisionSegments = true;
    }

    public override void CollidedByObject(int n = 0, eEliminateDef eliminateDef = eEliminateDef.HitObjectCollider)
    {
        if ( _collisionSegmentsCount > n )
        {
            _curLaserLen = (_collisionSegments[n] - _curPos).magnitude;
            // 截取掉之后的碰撞组
            _collisionSegmentsCount = n;
            _hitObject = true;
        }
    }

    public void Execute(int cmd, object[] data)
    {
        if ( cmd == CommandConsts.STGFrameStart )
        {
            _isCachedCollisionSegments = false;
            // 计算该帧开始之后laser的起始长度
            _curLaserLen = _curLaserLen + LaserSpeedPerFrame > _laserTotalLen ? _laserTotalLen : _curLaserLen + LaserSpeedPerFrame;
            _hitObject = false;
        }
    }

    public override void Clear()
    {
        CommandManager.GetInstance().Remove(CommandConsts.STGFrameStart, this);
        UIManager.GetInstance().HideGo(_bullet);
        ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _bullet);
        _laserSr.sprite = null;
        _laserSr = null;
        _laserTf = null;
        base.Clear();
    }
}
