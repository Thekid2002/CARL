using CARL.AST.Expressions;

namespace CARL.AST;

public class Program : AstNode
{
    public Expression Expression { get; set; }

    public Program(Expression expression)
    {
        Expression = expression;
    }

}
