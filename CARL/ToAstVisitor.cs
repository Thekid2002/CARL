using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CARL.AST;
using CARL.AST.Expressions;
using CARL.AST.Expressions.Term;
using Expression = CARL.AST.Expressions.Expression;

namespace CARL;

public class ToAstVisitor : CARLBaseVisitor<AstNode>
{
    public override AST.Program VisitProgram(CARLParser.ProgramContext context)
    {
        var expression = (Expression) context.expression().Accept(this);
        return new AST.Program(expression);
    }

    #region Expressions

    public override Expression VisitExpression(CARLParser.ExpressionContext context)
    {
        return VisitBinaryExpressionContext(context.equalityExpression(), context.children, context.Start.Line);
    }

    public override Expression VisitEqualityExpression(CARLParser.EqualityExpressionContext context)
    {
        return VisitBinaryExpressionContext(context.relationExpression(), context.children, context.Start.Line);
    }

    public override Expression VisitRelationExpression(CARLParser.RelationExpressionContext context)
    {
        return VisitBinaryExpressionContext(context.binaryExpression(), context.children, context.Start.Line);
    }

    public override Expression VisitBinaryExpression(CARLParser.BinaryExpressionContext context)
    {
        return VisitBinaryExpressionContext(context.multExpression(), context.children, context.Start.Line);
    }

    public override Expression VisitMultExpression(CARLParser.MultExpressionContext context)
    {
        return VisitBinaryExpressionContext(context.unaryExpression(), context.children, context.Start.Line);
    }
    
    private Expression VisitBinaryExpressionContext<T>(IEnumerable<T> expressionsContext, IList<IParseTree> children, int line) where T : ParserRuleContext
    {
        var expressions = expressionsContext.Select(e => e.Accept(this) as Expression).ToList();
    
        var operators = children
            .Where(c => c is ITerminalNode)
            .Select(c => c.GetText())
            .ToList();
        
        var leftOrPrimary = expressions[0];

        if (expressions.Count == 1)
        {
            return leftOrPrimary;
        }

        for (int i = 1; i < expressions.Count; i++)
        {
            Expression right = expressions[i];
            leftOrPrimary = new BinaryOp(leftOrPrimary, operators[i - 1], right)
            {
                LineNum = line
            };
        }

        return leftOrPrimary;
    }
    
    public override Expression VisitUnaryExpression(CARLParser.UnaryExpressionContext context)
    {
        var rightOrPrimary = context.term().Accept(this) as Expression;

        var operators = context.children
            .TakeWhile(c => c is ITerminalNode)  // Assuming unary operators are terminal nodes
            .Select(c => c.GetText())
            .ToList();

        foreach (var op in operators.AsEnumerable().Reverse())
        {
            rightOrPrimary = new UnaryOp(op, rightOrPrimary)
            {
                LineNum = context.Start.Line
            };
        }

        return rightOrPrimary;
    }

    
    public override Expression VisitTerm(CARLParser.TermContext context)
    {
        if (context.NUM() != null)
        {
            return new Num(context.NUM().GetText()) {LineNum = context.Start.Line};
        }
        
        if (context.GetText() == "true" || context.GetText() == "false")
        {
            return new Bool(context.GetText()) {LineNum = context.Start.Line};
        }
        
        if (context.expression() != null)
        {
            return context.expression().Accept(this) as Expression;
        }
        
        throw new NotSupportedException($"Term type not supported: {context.GetText()}");
    }
    
    #endregion


}
