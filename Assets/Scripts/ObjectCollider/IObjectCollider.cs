
public interface IObjectCollider : IPosition
{
    void SetSize(float arg0, float arg1);
    void SetColliderGroup(eColliderGroup groups);
    void ScaleToSize(float toArg0, float toArg1, int duration);
    void SetExistDuration(int existDuration);
}