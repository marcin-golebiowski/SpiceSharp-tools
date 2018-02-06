﻿using NLex;

namespace SpiceParser
{
    public class ParseTreeTerminalNode : ParseTreeNode
    {
        public Token Token { get; set; }

        public override string ToString()
        {
            return "Terminal: [" + (Token.Value == "\r\n" ? "newline" : Token.Value) + "]";
        }

        public override void Accept(ParseTreeVisitor visitor)
        {
            visitor.VisitParseTreeTerminal(this);
        }
    }
}