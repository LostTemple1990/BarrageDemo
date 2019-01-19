

public interface ISTGMovable : IPosition
{
    void DoStraightMove(float v, float angle);
    void DoStraightMoveWithLimitation(float v, float angle, int duration);
    void DoAcceleration(float acce, float accAngle);
    void DoAccelerationWithLimitation(float acce, float accAngle, float maxVelocity);
    void DoMoveTo(float endX, float endY, int duration, InterpolationMode mode);
    void DoCurvedMove(float radius, float angle, float deltaR, float omega);

    void Update();

    float Velocity { get; }
    float VAngle { get; }
    float Acce { get; }
    float AccAngle { get; }
}