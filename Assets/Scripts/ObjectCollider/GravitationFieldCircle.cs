using UnityEngine;
using System.Collections;

public class GravitationFieldCircle : ColliderCircle, IGravitationField
{
    /// <summary>
    /// 赋予的速度
    /// </summary>
    private float _velocity;
    /// <summary>
    /// 速度偏移
    /// </summary>
    private float _velocityOffset;
    private float _angle;
    private float _angleOffset;
    private float _acce;
    private float _acceOffset;
    private float _accAngle;
    private float _accAngleOffset;

    protected override void CollidedByPlayer()
    {
        
    }

    protected override void CollidedByPlayerBullet(PlayerBulletBase bullet, int curColliderIndex)
    {
        
    }

    protected override void CollidedByEnemy(EnemyBase enemy)
    {
        
    }

    protected override void CollidedByEnemyBullet(EnemyBulletBase bullet, int curColliderIndex)
    {
        
    }

    public void Init(int type, float velocity, float velocityOffest, float angle, float angleOffest, float acce, float acceOffset, float accAngle, float accAngleOffset)
    {
        throw new System.NotImplementedException();
    }

    public CollisionDetectParas GetCollisionParas()
    {
        CollisionDetectParas paras = new CollisionDetectParas();
        paras.type = CollisionDetectType.Circle;
        paras.radius = _radius;
        paras.centerPos = _curPos;
        return paras;
    }
}
