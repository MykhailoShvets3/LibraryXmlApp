// Модель читача. Атрибути зберігаються у словнику, бо їх кількість може відрізнятися.
namespace LibraryXmlApp.Models;

public class Reader
{
    public Dictionary<string, string> Attributes { get; } = new();
}