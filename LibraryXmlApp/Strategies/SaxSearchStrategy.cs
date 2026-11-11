// SAX-стратегія: послідовне читання файлу через XmlReader без побудови дерева в пам'яті.
using System.Xml;
using LibraryXmlApp.Models;

namespace LibraryXmlApp.Strategies;

public class SaxSearchStrategy : ISearchStrategy
{
    public string Name => "SAX";

    public List<Book> Search(string filePath, string attribute, string value)
    {
        var result = new List<Book>();
        Book? current = null;

        using var xml = XmlReader.Create(filePath);
        while (xml.Read())
        {
            if (xml.NodeType == XmlNodeType.Element && xml.Name == "book")
            {
                current = new Book();
                ReadAttributes(xml, current.Attributes);
            }
            else if (xml.NodeType == XmlNodeType.Element && xml.Name == "reader" && current != null)
            {
                var reader = new Reader();
                ReadAttributes(xml, reader.Attributes);
                current.Readers.Add(reader);
            }
            else if (xml.NodeType == XmlNodeType.EndElement && xml.Name == "book" && current != null)
            {
                if (current.Matches(attribute, value))
                    result.Add(current);
                current = null;
            }
        }
        return result;
    }

    // Копіює всі атрибути поточного елемента у словник
    private static void ReadAttributes(XmlReader xml, Dictionary<string, string> target)
    {
        while (xml.MoveToNextAttribute())
            target[xml.Name] = xml.Value;
        xml.MoveToElement();
    }
}