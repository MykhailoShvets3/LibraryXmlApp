// Інтерфейс шаблону Strategy: кожна реалізація читає XML своїм способом (SAX, DOM, LINQ).
using LibraryXmlApp.Models;

namespace LibraryXmlApp.Strategies;

public interface ISearchStrategy
{
    string Name { get; }
    List<Book> Search(string filePath, string attribute, string value);
}