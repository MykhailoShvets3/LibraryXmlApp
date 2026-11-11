// LINQ to XML-стратегія: XDocument + LINQ-запит, що відбирає книги за атрибутом книги або читача.
using System.Xml.Linq;
using LibraryXmlApp.Models;

namespace LibraryXmlApp.Strategies;

public class LinqSearchStrategy : ISearchStrategy
{
    public string Name => "LINQ to XML";

    public List<Book> Search(string filePath, string attribute, string value)
    {
        var doc = XDocument.Load(filePath);

        var foundBooks = from b in doc.Root!.Elements("book")
                         where (string?)b.Attribute(attribute) == value
                            || b.Elements("reader").Any(r => (string?)r.Attribute(attribute) == value)
                         select b;

        var result = new List<Book>();
        foreach (var b in foundBooks)
        {
            var book = new Book();
            CopyAttributes(b, book.Attributes);

            foreach (var r in b.Elements("reader"))
            {
                var reader = new Reader();
                CopyAttributes(r, reader.Attributes);
                book.Readers.Add(reader);
            }
            result.Add(book);
        }
        return result;
    }

    private static void CopyAttributes(XElement element, Dictionary<string, string> target)
    {
        foreach (var a in element.Attributes())
            target[a.Name.LocalName] = a.Value;
    }
}