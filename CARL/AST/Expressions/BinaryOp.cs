namespace CARL.AST.Expressions;

public class BinaryOp : Expression
{
    public BinaryOp(Expression left, string op, Expression right)
    {
        Left = left;
        Op = op;
        Right = right;
    }

    public Expression Left { get; protected set; }
    public Expression Right { get; protected set; }
    public string Op { get; protected set; }
}
