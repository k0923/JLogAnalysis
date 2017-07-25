using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ConvertLib
{
    public interface INode<T>
    {
        T GetRoot(string name);

        Tuple<T, int> AddRootNode(T container, string name);

        void AddField(T container, string name, object value, bool isMap);

        void AddSimpleField(T container, object value);

        T GetParent(T container);

        Tuple<T, int> AddArray(T container, string name);

        Tuple<T, int> AddNode(T container, string left, string right);

        Tuple<T, int> AddArrayNode(T container, string name);

        Tuple<T, int> AddMapNode(T container, string name);

        T Merge(IEnumerable<T> elements);
    }

    public class XmlNode : INode<XElement>
    {
        public Tuple<XElement, int> AddArray(XElement container, string name)
        {
            XElement array = new XElement(name);
            container.Add(array);
            return new Tuple<XElement, int>(array, 1);
        }

        public Tuple<XElement, int> AddArrayNode(XElement container, string name)
        {
            XElement node = new XElement(name);
            container.Add(node);
            return new Tuple<XElement, int>(node, 1);
        }

        public void AddField(XElement container, string name, object value, bool isMap)
        {
            if (value == null)
            {
                container.Add(new XElement(name));
            }
            else
            {
                if (isMap)
                {
                    var e = new XElement("entry",
                                    new XElement("string", name),
                                    new XElement("string", value));
                    container.Add(e);
                }
                else
                {
                    container.Add(new XElement(name, value));
                }

            }

        }

        public Tuple<XElement, int> AddMapNode(XElement container, string name)
        {
            XElement node = new XElement(name);
            container.Add(node);
            return new Tuple<XElement, int>(node, 1);
        }

        public Tuple<XElement, int> AddNode(XElement container, string left, string right)
        {
            XElement node = new XElement(left);
            container.Add(node);
            return new Tuple<XElement, int>(node, 1);
            //if (right.Contains("@"))
            //{
            //    right = right.Split('@')[0];
            //}
            //XElement node = new XElement(right);
            //container.Add(new XElement(left, node));
            //return new Tuple<XElement, int>(node, 2);
        }

        public Tuple<XElement, int> AddRootNode(XElement container, string name)
        {
            return new Tuple<XElement, int>(container, 1);
        }

        public void AddSimpleField(XElement container, object value)
        {
            container.Add(new XElement("value", value));
        }

        public XElement GetParent(XElement container)
        {
            return container.Parent;
        }



        public XElement GetRoot(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new XElement("Array");
            }
            if (name.Contains("="))
            {
                name = name.Split('=')[0];
            }

            return new XElement(name);

        }

        public XElement Merge(IEnumerable<XElement> elements)
        {
            XElement e = new XElement("Unknown");
            foreach (var e1 in elements)
            {
                e.Add(e1);
            }
            return e;
        }
    }

    public class JsonNode : INode<JContainer>
    {
        public Tuple<JContainer, JContainer, int> GetRoot(string name)
        {

            var container = new JObject();
            var root = new JObject();
            root.Add(new JProperty(name, container));
            return new Tuple<JContainer, JContainer, int>(root, container, 2);
        }

        public JContainer GetParent(JContainer container)
        {
            return container.Parent;
        }

        public Tuple<JContainer, int> AddArray(JContainer container, string name)
        {
            var array = new JArray();
            container.Add(new JProperty(name, array));
            return new Tuple<JContainer, int>(array, 2);
        }

        public Tuple<JContainer, int> AddNode(JContainer container, string left, string right)
        {
            var node = new JObject();
            container.Add(new JProperty(left, node));
            return new Tuple<JContainer, int>(node, 2);
        }

        public Tuple<JContainer, int> AddArrayNode(JContainer container, string name)
        {
            var obj = new JObject();
            container.Add(obj);
            return new Tuple<JContainer, int>(obj, 1);
        }

        public Tuple<JContainer, int> AddMapNode(JContainer container, string name)
        {
            var obj = new JObject();
            container.Add(new JProperty(name, obj));
            return new Tuple<JContainer, int>(obj, 2);
        }

        public void AddField(JContainer container, string name, object value, bool isMap)
        {
            container.Add(new JProperty(name, value));
        }

        public void AddSimpleField(JContainer container, object value)
        {
            container.Add(value);
        }

        JContainer INode<JContainer>.GetRoot(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return new JArray();
            }
            return new JObject();
        }

        public Tuple<JContainer, int> AddRootNode(JContainer container, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return new Tuple<JContainer, int>(container, 1);
            }
            if (name.Contains("="))
            {
                name = name.Split('=')[0];
            }
            var node = new JObject();
            container.Add(new JProperty(name, node));
            return new Tuple<JContainer, int>(node, 2);
        }

        public JContainer Merge(IEnumerable<JContainer> elements)
        {
            JObject obj = new JObject();
            int i = 0;
            foreach (var e in elements)
            {
                obj.Add(new JProperty($"para{i}", e));
            }
            return obj;
        }
    }
}
