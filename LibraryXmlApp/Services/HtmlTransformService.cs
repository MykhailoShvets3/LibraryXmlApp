// Сервіс перетворення XML у HTML за допомогою XSL-таблиці стилів (XslCompiledTransform).
using System.Xml;
using System.Xml.Xsl;

namespace LibraryXmlApp.Services;

public static class HtmlTransformService
{
    // Створює library.html поруч з обраним XML-файлом і повертає шлях до нього
    public static async Task<string> TransformAsync(string xmlPath)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("library.xsl");
        using var xslReader = XmlReader.Create(stream);

        var xslt = new XslCompiledTransform();
        xslt.Load(xslReader);

        string htmlPath = Path.Combine(Path.GetDirectoryName(xmlPath)!, "library.html");
        xslt.Transform(xmlPath, htmlPath);
        return htmlPath;
    }
}