
public interface IParaChangable : ISTGObject
{
    bool GetParaValue(STGObjectParaType paraType, out float value);
    bool SetParaValue(STGObjectParaType paraType, float value);
}
