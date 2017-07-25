using System;
using System.Collections.Generic;
using System.Text;

namespace ConvertLib
{
    public class JLogFormat
    {
        public List<char> content;

        public string Data {
            get {
                return new string(content.ToArray());
            }
        }

        private List<char> dataContent;

        private Stack<int> flags;



        public void Format(string log)
        {
            flags = new Stack<int>();
            dataContent = new List<char>();
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
                    openFormat(c);
                }
                else if (lefts.Contains(c))
                {
                    if (flags.Count == 0)
                    {
                        flags.Push(1);
                    }
                    else
                    {
                        flags.Push(flags.Peek() + 1);
                    }
                    openFormat(c);
                }
                else if (rights.Contains(c))
                {
                    closeFormat(c);
                }
                else
                {
                    dataContent.Add(c);
                }
            }

        }

        private void closeFormat(char c)
        {
            if (dataContent.Count > 0)
            {
                content.AddRange(dataContent);
                dataContent.Clear();

            }
            content.Add('\n');
            var empty = flags.Pop();
            for (int i = 0; i < empty - 1; i++)
            {
                content.Add('\t');
            }
            content.Add(c);

        }

        private void openFormat(char c)
        {

            if (dataContent.Count > 0)
            {
                content.AddRange(dataContent);
                dataContent.Clear();
            }
            content.Add(c);
            content.Add('\n');
            if (flags.Count > 0)
            {
                var empty = flags.Peek();
                for (int i = 0; i < empty; i++)
                {
                    content.Add('\t');
                }
            }
        }
    }
}
