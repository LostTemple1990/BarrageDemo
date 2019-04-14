using System.Xml;

public interface IParser
{
    void parse(XmlElement xmlElement);
    IParser CreateNewInstance();
}