using BE;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

public static class CarritoXml
{
    public static byte[] GuardarCarrito(List<BEItemPedido> items)
    {
        using (var archivo = new MemoryStream())
        {
            var opciones = new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false) };
            using (XmlWriter writer = XmlWriter.Create(archivo, opciones))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Carrito");
                foreach (var item in items)
                {
                    writer.WriteStartElement("Producto");
                    writer.WriteElementString("IdProducto", XmlConvert.ToString(item.IdProducto));
                    writer.WriteElementString("Cantidad", XmlConvert.ToString(item.Cantidad));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            return archivo.ToArray();
        }
    }

    public static List<BEItemPedido> LeerCarrito(Stream archivo)
    {
        var items = new List<BEItemPedido>();
        var opciones = new XmlReaderSettings
        {
            IgnoreWhitespace = true, IgnoreComments = true,
            DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null,
            MaxCharactersInDocument = 65536
        };
        using (XmlReader reader = XmlReader.Create(archivo, opciones))
        {
            reader.MoveToContent();
            bool vacio = reader.IsEmptyElement;
            reader.ReadStartElement("Carrito", "");
            if (!vacio)
            {
                while (reader.IsStartElement("Producto", ""))
                {
                    reader.ReadStartElement("Producto", "");
                    int id = reader.ReadElementContentAsInt("IdProducto", "");
                    int cantidad = reader.ReadElementContentAsInt("Cantidad", "");
                    reader.ReadEndElement();
                    items.Add(new BEItemPedido { IdProducto = id, Cantidad = cantidad });
                }
                reader.ReadEndElement();
            }
            if (reader.MoveToContent() != XmlNodeType.None)
                throw new XmlException("Contenido inesperado después del carrito.");
        }
        return items;
    }
}
