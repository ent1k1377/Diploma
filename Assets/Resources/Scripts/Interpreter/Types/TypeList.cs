using System.Collections.Generic;
using System.Linq;
using Resources.Scripts.Interpreter.TokenInfo;
using static Resources.Scripts.Interpreter.Types.TokenType;

namespace Resources.Scripts.Interpreter.Types
{
    public static class TypeList
    {
        public static List<TokenId> Types { get; }
        static TypeList()
        {
            Types = new List<TokenId>()
                {
                    new(EmptyOperator, @"[\ \n\r]"),
                    new(Comment,"//[а-я_]{0,}"),
                    new(Step, "Step "),
                    new(TakeFrom, "TakeFrom "),
                    new(GiveTo, "GiveTo "),
                    new(Direction, "[N,E,S,W,C]{1,2}"),
                    new(MyItem, "MyItem"),
                    new(If, "if"),
                    new(Else, "else"),
                    new(EndIf, "endif"),
                    new(ComparisonOperator, "([=,!,>,<][=])|[>,<]"),
                    new(And, "and"),
                    new(Or, "or"),
                    new(BooleanArgument, "something|nothing|datacube|worker|hole|wall|[0-9]{1,2}"),
                    new(EndBody, ":"),
                };
        }
        public static TokenId GetTokenBy(TokenType tokenType)
        {
            return Types.FirstOrDefault(id => id.Type == tokenType);
        }

        public static List<TokenId> GetMethodsTokens()
        {
            return new List<TokenId>
            {
                GetTokenBy(Step),
                GetTokenBy(TakeFrom),
                GetTokenBy(GiveTo),
            };
        }

        public static List<TokenId> GetLeftBooleanArgumentTokens()
        {
            return new List<TokenId>
            {
                GetTokenBy(Direction),
                GetTokenBy(MyItem),
            };
        }
    }
    
    public enum TokenType
    {
        Null,
        EmptyOperator,
        Comment,
        Direction,
        MyItem,
        Step,
        TakeFrom,
        GiveTo,
        If,
        Else,
        EndIf,
        ComparisonOperator,
        And,
        Or,
        BooleanArgument,
        EndBody
    }

}
