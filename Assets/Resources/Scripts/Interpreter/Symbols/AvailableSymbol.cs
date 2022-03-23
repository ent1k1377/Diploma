using System.Collections.Generic;

namespace Resources.Scripts.Interpreter.Symbols
{
    public class AvailableSymbol : Symbol
    {
        public List<Symbol> CodeLine { get; }
        public AvailableSymbol() => CodeLine = new List<Symbol>();
        public void Add(Symbol node) => CodeLine.Add(node);
    }
}
