using System.Collections.Generic;
using System.Linq;
using Resources.Scripts.Interpreter.TokenInfo;

namespace Resources.Scripts.Interpreter.Types
{
    public static class TypeList
    {
        public static List<TokenId> Types { get; }
        static TypeList()
        {
            Types = new List<TokenId>()
                {
                    new("EmptyOperator", @"[\ \n\r]"),
                    new("Variable", @"[a-z]{1,}[_]{0,}[a-z]{0,}"),
                    new("Integer", @"[0-9]{1,7}"),
                    new("EndLine", @";"),
                    new("AssignmentOperator", @"\:{2}\="),// ::=
                    new("Terminal", "Print"),
                    new("Addition", @"\+"),
                    new("Subtraction", @"\-"),
                    new("Multiplication", "\\*"),
                    new("Division", @"\/"),
                    new("LeftBracket", @"\["),
                    new("RightBracket", @"\]"),
                    new("Comment", "//[а-я_]{0,}"),
                };
        }
        public static TokenId GetTokenBy(string identifier)
        {
            return Types.FirstOrDefault(id => id.Identifier == identifier);
        }
    }

}
