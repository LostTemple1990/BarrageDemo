
public interface IGravitationField
{
    void Init(int type, float velocity, float velocityOffest, float angle, float angleOffest, float acce, float acceOffset, float accAngle, float accAngleOffset);
    CollisionDetectParas GetCollisionParas();
}