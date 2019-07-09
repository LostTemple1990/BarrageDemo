
public interface IObjectCollider : IPosition
{
    void SetTag(string tag);
    string GetTag();
    void SetSize(float arg0, float arg1);
    void SetColliderGroup(eColliderGroup groups);
    eColliderGroup GetColliderGroup();
    void ScaleToSize(float toArg0, float toArg1, int duration);
    void SetExistDuration(int existDuration);
}