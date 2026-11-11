// Модель книги: атрибути, список читачів і перевірка збігу з критерієм пошуку.
using System.Text;

namespace LibraryXmlApp.Models;

public class Book
{
    public Dictionary<string, string> Attributes { get; } = new();
    public List<Reader> Readers { get; } = new();

    public bool Matches(string attribute, string value) =>
        (Attributes.TryGetValue(attribute, out var v) && v == value) ||
        Readers.Any(r => r.Attributes.TryGetValue(attribute, out var rv) && rv == value);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Книга: " + string.Join("; ", Attributes.Select(a => $"{a.Key}={a.Value}")));
        foreach (var r in Readers)
            sb.AppendLine("    Читач: " + string.Join("; ", r.Attributes.Select(a => $"{a.Key}={a.Value}")));
        return sb.ToString();
    }
}