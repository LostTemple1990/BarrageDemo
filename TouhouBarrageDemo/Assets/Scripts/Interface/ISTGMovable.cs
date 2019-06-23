

public interface ISTGMovable : IPosition
{
    void DoStraightMove(float v, float angle);
    void DoAcceleration(float acce, float accAngle);
    void DoAccelerationWithLimitation(float acce, float accAngle, float maxVelocity);

    void SetPolarParas(float radius, float angle, float deltaR, float omega);

    void MoveTo(float endX, float endY, int duration, InterpolationMode mode);
    void MoveTowards(float v, float angle, int duration);

    void Update();

    float velocity { get; }
    float vAngle { get; }
    float acce { get; }
    float accAngle { get; }
    float dx { get; }
    float dy { get; }
}