namespace CARL.AST.Expressions.Term;

public class Num: Term
{
    public Num(string value)
    {
        Value = value;
    }

    public string Value { get; protected set; }
}
