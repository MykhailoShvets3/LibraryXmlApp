// DOM-стратегія: файл повністю завантажується в пам'ять (XmlDocument), пошук виконується XPath-запитом.
using System.Xml;
using LibraryXmlApp.Models;

namespace LibraryXmlApp.Strategies;

public class DomSearchStrategy : ISearchStrategy
{
    public string Name => "DOM";

    public List<Book> Search(string filePath, string attribute, string value)
    {
        var doc = new XmlDocument();
        doc.Load(filePath);

        // Динамічна генерація XPath-запиту за обраним атрибутом і значенням
        string q = value.Contains('\'') ? "\"" : "'";
        string xpath = $"/library/book[@{attribute}={q}{value}{q} or reader[@{attribute}={q}{value}{q}]]";

        var result = new List<Book>();
        foreach (XmlNode bookNode in doc.SelectNodes(xpath)!)
        {
            var book = new Book();
            CopyAttributes(bookNode, book.Attributes);

            foreach (XmlNode readerNode in bookNode.SelectNodes("reader")!)
            {
                var reader = new Reader();
                CopyAttributes(readerNode, reader.Attributes);
                book.Readers.Add(reader);
            }
            result.Add(book);
        }
        return result;
    }

    private static void CopyAttributes(XmlNode node, Dictionary<string, string> target)
    {
        foreach (XmlAttribute a in node.Attributes!)
            target[a.Name] = a.Value;
    }
}