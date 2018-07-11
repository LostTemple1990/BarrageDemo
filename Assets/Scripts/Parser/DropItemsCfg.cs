using System.Xml;

public class DropItemsCfg : IParser
{
    public string id;
    public int pPointN;
    public int pPointB;
    public int score;

    public IParser CreateNewInstance()
    {
        return new DropItemsCfg();
    }

    public void parse(XmlElement xmlElement)
    {
        id = xmlElement.GetAttribute("id");
        pPointN = int.Parse(xmlElement.GetAttribute("pPointN"));
        pPointB = int.Parse(xmlElement.GetAttribute("pPointB"));
    }
}
