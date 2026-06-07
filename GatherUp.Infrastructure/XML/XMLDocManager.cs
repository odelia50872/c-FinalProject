using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace GatherUp.Infrastructure.XML
{
    public static class XMLDocManager
    {
        public static void SaveElements(string filePath, string rootName, IEnumerable<XElement> elements)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(rootName, elements)
            );
            doc.Save(filePath);
        }

        public static IEnumerable<XElement> LoadElements(string filePath, string elementName)
        {
            if (!System.IO.File.Exists(filePath))
                return Enumerable.Empty<XElement>();

            XDocument doc = XDocument.Load(filePath);
            return doc.Descendants(elementName);
        }
    }
}