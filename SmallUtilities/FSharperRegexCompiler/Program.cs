using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSharperRegexCompiler
{
    class Program
    {
        private static void Main(string[] args)
        {
            //      \(\*((([^\)][^\)]([^\*]|(\*+[^\*\)]))*)\*+\))|(\*+\)))
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(@"ASTERISKS='*'+");
            stringBuilder.AppendLine(@"DELIMITED_COMMENT_SECTION=([^\*]|({ASTERISKS}[^\*\)]))");
            stringBuilder.AppendLine(@"UNFINISHED_DELIMITED_COMMENT='(*'[^\)]{DELIMITED_COMMENT_SECTION}*");
            stringBuilder.AppendLine(@"DELIMITED_COMMENT={UNFINISHED_DELIMITED_COMMENT}{ASTERISKS}')'");

            var result = Replace(stringBuilder.ToString());

            var dictionary =
                result
                    .Split('\n')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x =>
                    {
                        var parts = x.Split('=');
                        return new
                        {
                            Name = parts[0],
                            Value = parts[1]
                        };
                    }).ToDictionary(x => x.Name, x => x.Value);

            var resultDictionary = dictionary.ToDictionary(x => x.Key, x => x.Value);

            foreach (var key in dictionary.Keys)
            {
                var wrappedKey = "{" + key + "}";
                resultDictionary = resultDictionary.ToDictionary(x => x.Key,
                    x => x.Value.Replace(wrappedKey, resultDictionary[key]));
            }

            //var result = resultDictionary.Values.Select(Replace);

            Console.WriteLine(string.Join("\r\n", resultDictionary.Values));
        }

        public static string Replace(string value)
        {
            bool inside = false;
            string word = "";
            var list = new List<string>();
            foreach (var x in value)
            {
                if (x == '\'')
                {
                    if (inside)
                    {
                        list.Add(word);
                        word = "";
                        inside = false;
                    }
                    else
                    {
                        inside = true;
                    }
                }
                else
                {
                    if (inside)
                    {
                        word += x;
                    }
                }
            }
            return
                list
                    .Select(x => new {Old = "'" + x + "'", New = x.Select(y => "\\" + y).Aggregate((current, y) => current + y)})
                    .Aggregate(value, (current, x) => current.Replace(x.Old, x.New));
        }
    }
}
