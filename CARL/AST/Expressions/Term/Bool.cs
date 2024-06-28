namespace CARL.AST.Expressions.Term;

public class Bool: Term
{
    public Bool(string value)
    {
        Value = value;
    }

    public string Value { get; protected set; }
}
