using System;
using System.Collections.Generic;
using System.Text;

namespace ConvertLib
{
    public interface IJLog
    {
        void Analysis(string log);

        Object Root { get; }
    }

    public class JLog<T1, T> : IJLog where T1 : INode<T>, new()
                               where T : class
    {

        public T Root { get; private set; }

        object IJLog.Root => Root;

        private T1 factory;

        private List<char> content;

        private Stack<int> flags;

        private List<T> roots;

        private bool isMap;

        public JLog()
        {
            factory = new T1();
        }

        private T lastFieldNode;

        public void Analysis(string log)
        {
            flags = new Stack<int>();
            roots = new List<T>();
            content = new List<char>();
            var lefts = new List<char>() { '[', '{' };
            var rights = new List<char>() { ']', '}' };

            foreach (var c in log)
            {
                if (c == '\n' || c == '\t' || c == ' ' || c == '\r')
                {
                    continue;
                }
                if (c == ',')
                {
                    addField();
                }
                else if (lefts.Contains(c))
                {
                    openFlag(c);
                }
                else if (rights.Contains(c))
                {
                    closeFlag(c);
                }
                else
                {
                    content.Add(c);
                }
            }

            if (roots.Count == 1)
            {
                Root = roots[0];
            }
            else if (roots.Count > 1)
            {
                Root = factory.Merge(roots);
            }
        }

        private void openFlag(char c)
        {
            lastFieldNode = null;
            isSimple = true;
            var dataContent = new string(content.ToArray());
            Tuple<T, int> result = null;

            if (Root == null)
            {
                Root = factory.GetRoot(dataContent);
                result = factory.AddRootNode(Root, dataContent);
            }
            else
            {
                if (c == '{')
                {
                    if (dataContent.Contains("="))
                    {
                        dataContent = dataContent.Split('=')[0];
                    }
                    result = factory.AddMapNode(Root, dataContent);
                    isMap = true;
                }
                else
                {
                    if (dataContent.Contains("="))
                    {
                        var pair = dataContent.Split('=');
                        if (pair[1] == "")
                        {
                            result = factory.AddArray(Root, pair[0]);
                        }
                        else
                        {
                            result = factory.AddNode(Root, pair[0], pair[1]);
                        }
                    }
                    else
                    {
                        result = factory.AddArrayNode(Root, dataContent);
                    }
                }
            }

            Root = result.Item1;
            flags.Push(result.Item2);
            content.Clear();
        }

        private void closeFlag(char c)
        {
            addField();
            content.Clear();
            var count = flags.Pop();
            for (int i = 0; i < count; i++)
            {
                var root = factory.GetParent(Root);
                if (root == null)
                {
                    roots.Add(Root);
                }
                Root = factory.GetParent(Root);
            }
            if (isMap)
            {
                isMap = false;
            }
        }


        private void addField()
        {
            if (content.Count == 0)
            {
                return;
            }
            var dataContent = new string(content.ToArray());
            if (content.Contains('='))
            {
                isSimple = false;
                var pair = dataContent.Split('=', 2, StringSplitOptions.None);
                if (pair[1] == "<null>")
                {
                    factory.AddField(Root, pair[0], null, isMap);
                }
                else
                {
                    lastFieldNode = factory.AddField(Root, pair[0], pair[1], isMap);
                }
            }
            else
            {
                if (isSimple)
                {
                    factory.AddSimpleField(Root, dataContent);
                }
                else
                {
                    dataContent = "," + dataContent;
                    factory.ModifyField(lastFieldNode, dataContent);
                }
            }
            content.Clear();
        }

        private bool isSimple = false;
    }


}
