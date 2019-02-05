using System.Linq;
using System.Collections.Generic;
using Cadl.Core.Components;

namespace Cadl.Core.Parsers
{
    public class SqlParser : ComponentParser
    {
        private Sql sql;

        public override Component Parse(List<Line> lines, out int index)
        {
            sql = new Sql();
            bool openBraceFound = false;

            for (index = 0; index < lines.Count(); index++)
            {
                var line = lines[index];
                if (index == 0)
                {
                    SetSqlName(line);
                }

                else if (index == 1)
                {
                    if (line.Parts[0].IndexOf('{') == -1)
                    {
                        throw new ParsingException(new Error(Error.MissingOpenBrace));
                    }
                    else
                    {
                        openBraceFound = true;
                    }
                }

                else if (line.Parts[0].IndexOf('}') != -1)
                {
                    if (!openBraceFound)
                    {
                        throw new ParsingException(new Error(Error.MissingOpenBrace));
                    }
                    else
                    {
                        return sql;
                    }
                }

                else if (line.Parts[0] == "Table")
                {
                    index += AddTable(lines.Skip(index).ToList());
                }

                else 
                {
                    throw new ParsingException(new Error(Error.UnknownSyntax));
                }
            }

            throw new ParsingException(new Error(Error.MissingCloseBrace));
        }

        private void SetSqlName(Line line)
        {
            if (!line.PartsEqualTo(4))
            {
                throw new ParsingException(new Error(Error.UnknownSyntax));
            }
            else
            {
                sql.DbName = line.Parts[2];
                sql.ComponentName = line.Parts[3];
            }
        }

        private int AddTable(List<Line> lines)
        {
            bool openBraceFound = false;
            bool openBracketFound = false;

            var table = new Table();
            int index = 0;
            for (; index < lines.Count(); index++)
            {
                var line = lines[index];

                if (index == 0)
                {
                    if (!line.PartsEqualTo(2))
                    {
                        throw new ParsingException(new Error(Error.UnknownSyntax));
                    }
                    else
                    {
                        table.Name = line.Parts[1];
                    }
                }

                else if (index == 1)
                {
                    if (line.Parts[0].IndexOf('[') == -1)
                    {
                        throw new ParsingException(new Error(Error.MissingOpenBrace));
                    }
                    else
                    {
                        openBracketFound = true;
                    }
                }

                else if (line.Parts[0].IndexOf(']') != -1)
                {
                    if (!openBracketFound)
                    {
                        throw new ParsingException(new Error(Error.MissingOpenBrace));
                    }
                    else
                    {
                        openBracketFound = false;
                        if (lines[index + 1].Parts[0].IndexOf('{') == -1)
                        {
                            sql.Tables.Add(table);
                            return index;
                        }
                    }
                }

                else if (line.Parts[0].IndexOf('{') != -1)
                {
                    openBraceFound = true;
                }

                else if (line.Parts[0].IndexOf('}') != -1)
                {
                    if (!openBraceFound)
                    {
                        throw new ParsingException(new Error(Error.MissingOpenBrace));
                    }
                    else
                    {
                        openBraceFound = false;
                        sql.Tables.Add(table);
                        return index;
                    }
                }

                else if (openBracketFound)
                {
                    //if (!line.PartsEqualTo(2))
                    //{
                    //    throw new ParsingException(new Error(Error.UnknownSyntax));
                    //}
                    //else
                    //{
                    //    table.AddColumn(line.Parts[1], line.Parts[0]);
                    //}
                    table.TSql += line.Content + "\n";
                }

                else if (openBraceFound)
                {
                    //if (!line.PartsEqualTo(2))
                    //{
                    //    throw new ParsingException(new Error(Error.UnknownSyntax));
                    //}
                    //else
                    //{
                    //    table.InsertValues.Add(line.Parts.ToArray());
                    //}
                    table.Insests += line.Content + "\n";
                }
            }

            return index;
        }
    }
}
