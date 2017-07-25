using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ConvertLib
{
    public class XmlToBean
    {
        public  void Test()
        {
            XDocument xDoc = XDocument.Load(new FileStream("xmlToModify.xml", FileMode.Open));


            var e = BuildBeanElement(xDoc.Root);
            XDocument newDoc = XDocument.Parse(e.ToString());

            newDoc.Save(new FileStream("convert.xml", FileMode.Create));

        }

        public XElement BuildBeanElement(XElement ele)
        {

            if (ele.Elements().Any(e => e.Name.LocalName == "entry"))
            {

                return new XElement("map",
                    from e1 in ele.Elements()
                    select
                        e1.Elements().Count() != 2 ? null :
                        new XElement("entry",
                            new XElement("key",
                                new XElement("value", e1.Elements().ElementAt(0).Name.LocalName == "string" ? e1.Elements().ElementAt(0).Value : (dynamic)BuildBeanElement(e1.Elements().ElementAt(0)))),
                            new XElement("value", e1.Elements().ElementAt(1).Name.LocalName == "string" ? e1.Elements().ElementAt(1).Value : (dynamic)BuildBeanElement(e1.Elements().ElementAt(1)))
                        ));


            }

            if (ele.Name.LocalName == "entry")
            {
                return new XElement("set", from e1 in ele.Elements() select BuildBeanElement(e1));
            }

            if (ele.Elements().Any(e1 => e1.Name.LocalName == "string" || e1.Name.LocalName == "value"))
            {
                return new XElement("list", from e1 in ele.Elements() select BuildBeanElement(e1));
            }

            //if (ele.Elements().Any(e1 => e1.Name.LocalName == "value"))
            //{
            //    return new XElement("bean",
            //                new XElement("property",
            //                    new XAttribute("name", ele.Name),
            //                        new XElement("list", from e1 in ele.Elements() select BuildBeanElement(e1))));
            //}

            if (ele.Name.LocalName == "string" || ele.Name.LocalName == "value")
            {
                return new XElement("value", ele.Value);
            }

            bool isList = true;
            if (ele.Elements().Count() == 0)
            {
                isList = false;
            }
            foreach (var e1 in ele.Elements())
            {
                if (e1.Elements().Count() == 0)
                {
                    isList = false;
                    break;
                }
            }
            var group = ele.Elements().GroupBy(e => e.Name).Select(g => g.Key).ToList();
            if (group.Count > 1)
            {
                isList = false;
            }
            if (ele.Elements().Any(e => e.Elements().Any(e1 => e1.Name.LocalName == "value")))
            {
                isList = false;
            }
            


            if (isList)
            {
                return new XElement("list",
                    from e1 in ele.Elements()
                    select BuildBeanElement(e1));
            }

            return new XElement("bean",
                                    ele.Name.LocalName.Contains(".") ?
                                    new XAttribute("class", ele.Name) : null,
                                    from e1 in ele.Elements()
                                    select
                                        buildProperty(e1)  
                                       
                                    );

            
            XObject buildProperty(XElement e)
            {
                if (string.IsNullOrEmpty(e.Value))
                {
                    return null;
                }
                else
                {
                    return new XElement("property",
                            new XAttribute("name", e.Name),
                                e.Elements().Count() == 0 ?
                                    new XAttribute("value", e.Value) : (dynamic)BuildBeanElement(e));
                }
            }
        }
    }
}
