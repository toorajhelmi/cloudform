using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudform.Core.Parsers
{
    public class Line
    {
        public Line(string content)
        {
            Content = content;
            Parts = content.Split(new[] { ' ', ',', '(', ')' }).ToList();
            Parts.RemoveAll(p => p == "");
        }

        public List<string> Parts { get; set; }
        public string Content { get; private set; }

        public bool PartsEqualTo(int count)
        {
            return Parts.Count == count;
        }

        public bool PartsMoreThan(int count)
        {
            return Parts.Count > count;
        }

        public bool EndsWidth(char c)
        {
            return Content.EndsWith(c);
        }

        public bool EnsureBeginScope()
        {
            if (Parts[0].IndexOf('{') == -1)
            {
                throw new ParsingException(new Error(Error.MissingOpenBrace));
            }
            else
            {
                return true;
            }
        }

        public bool EnsureEndScope()
        {
            if (Parts[0].IndexOf('}') != -1)
            {
                throw new ParsingException(new Error(Error.MissingCloseBrace));
            }
            else
            {
                return true;
            }
        }

        public string At(int index)
        {
            if (Parts.Count > index)
            {
                return Parts[index];
            }
            else 
            {
                return null;
            }
        }

        public string[] AtSplit(int index, char seprator = ':')
        {
            var part = At(index);
            if (part == null)
            {
                return null;
            }
            else
            {
                return part.Split(new[] { seprator });
            }
        }

        public string AtVal(int index, string key, char separator = ':')
        {
            var parts = AtSplit(index, separator);

            if (parts == null)
            {
                return null;
            }
            else
            {
                if (parts[0] == key)
                {
                    return parts[1];
                }
                else
                {
                    return null;
                }
            }
        }

        public bool EnsureKeyAtOrNull(int index, string[] keys, char separator = ':')
        {
            var parts = AtSplit(index, separator);
            if (parts == null)
            {
                return true;
            }
            else
            {
                return keys.Contains(parts[0]);
            }
        }

        public bool EnsureKeyAt(int index, string[] keys, char separator = ':')
        {
            var parts = AtSplit(index, separator);
            if (parts == null)
            {
                return false;
            }
            else
            {
                return keys.Contains(parts[0]);
            }
        }

        public bool KeyExists(string key)
        {
            return Parts.Any(p => p.IndexOf(key) != -1);
        }
    }
}
