using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ConvertLib
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var log = File.ReadAllText("Log1.txt");

            //var converter = new XmlToBean();
            //XElement root = XElement.Parse(log);
            //root = converter.BuildBeanElement(root);

            //File.WriteAllText("Result.xml", root.ToString());
            var jLog = new JLog<JsonNode, JContainer>();
            jLog.Analysis(log);
            File.WriteAllText("tmp.txt",jLog.Root.ToString());
            //var jLog = new JLog<XmlNode, XElement>();
            //jLog.Analysis(log);
            //File.WriteAllText("Result.xml", jLog.Root.ToString());
            //Console.ReadLine();
        }
    }
}
