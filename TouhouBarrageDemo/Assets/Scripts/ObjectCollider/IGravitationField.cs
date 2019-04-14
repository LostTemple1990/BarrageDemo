
public interface IGravitationField : IObjectCollider
{
    void Init(int type, float velocity, float velocityOffest, float angle, float angleOffest, float acce, float acceOffset, float accAngle, float accAngleOffset);
    //void CollidedByObject(IAffectedMovableObject affectedObject);
    //GravitationParas CreateGravitationParas(IAffectedMovableObject affectedObj);
}