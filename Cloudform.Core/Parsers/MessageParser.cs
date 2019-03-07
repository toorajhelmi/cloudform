using System;
using System.Linq;
using System.Collections.Generic;
using Cloudform.Core.Interpreters.Messages;

namespace Cloudform.Core.Parsers
{
    public class MessageParser
    {
        private Message message;
        private bool openBraceFound;

        public Message Parse(List<Line> lines, out int index)
        {
            message = new Message();
            for (index = 0; index < lines.Count(); index++)
            {
                var line = lines[index];
                if (index == 0)
                {
                    AddMessageName(line);
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
                        return message;
                    }
                }

                else 
                {
                    AddMessageField(line);
                }
            }

            throw new ParsingException(new Error(Error.MissingCloseBrace));
        }

        private void AddMessageName(Line line)
        {
            if (!line.PartsEqualTo(2))
            {
                throw new ParsingException(new Error(Error.UnknownSyntax));
            }
            else
            {
                message.Name = line.Parts[1];
            }
        }

        private void AddMessageField(Line line)
        {
            if (!line.PartsEqualTo(2))
            {
                throw new ParsingException(new Error(Error.UnknownSyntax));
            }
            else
            {
                message.AddField(line.Parts[1], line.Parts[0]);
            }
        }
    }
}
