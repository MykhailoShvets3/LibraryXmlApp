// Сервіс підгрузки даних: збирає з XML-файлу імена атрибутів і їхні унікальні значення для вибору в інтерфейсі.
using System.Xml.Linq;

namespace LibraryXmlApp.Services;

public static class AttributeLoader
{
    public static Dictionary<string, List<string>> Load(string filePath)
    {
        var doc = XDocument.Load(filePath);

        return doc.Root!.Descendants()
            .SelectMany(e => e.Attributes())
            .GroupBy(a => a.Name.LocalName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(a => a.Value).Distinct().OrderBy(v => v).ToList());
    }
}