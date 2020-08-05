
public interface IParaChangable
{
    bool GetParaValue(BulletParaType paraType, out float value);
    bool SetParaValue(BulletParaType paraType, float value);
}
