using CARL.AST;
using CARL.AST.Expressions;
using CARL.AST.Expressions.Term;
using Xunit;
using Expression = CARL.AST.Expressions.Expression;

namespace CARL;

public class ToAstVisitor : CARLBaseVisitor<AstNode>
{
    public override AstNode VisitProgram(CARLParser.ProgramContext context)
    {
        var expression = (Expression) context.expression().Accept(this);
        return new AST.Program(expression);
    }

    public override AstNode VisitExpression(CARLParser.ExpressionContext context)
    {
        var leftOrPrimary = context.equalityExpression()[0].Accept(this) as Expression;
        Assert.NotNull(leftOrPrimary);

        var operatorIndex = 1;
        for (int i = 1; i < context.equalityExpression().Length; i++)
        {
            Expression? right = context.equalityExpression()[i].Accept(this) as Expression;
            leftOrPrimary = new BinaryOp(leftOrPrimary, context.GetChild(operatorIndex).GetText(), right)
            {
                LineNum = context.Start.Line
            };
            operatorIndex += 2;
        }

        return leftOrPrimary;
    }

    public override AstNode VisitEqualityExpression(CARLParser.EqualityExpressionContext context)
    {
        var leftOrPrimary = context.relationExpression()[0].Accept(this) as Expression;
        Assert.NotNull(leftOrPrimary);

        var operatorIndex = 1;

        for (int i = 1; i < context.relationExpression().Length; i++)
        {
            Expression? right = context.relationExpression()[i].Accept(this) as Expression;
            leftOrPrimary = new BinaryOp(leftOrPrimary, context.GetChild(operatorIndex).GetText(), right)
            {
                LineNum = context.Start.Line
            };
            operatorIndex += 2;
        }

        return leftOrPrimary;
    }

    public override AstNode VisitRelationExpression(CARLParser.RelationExpressionContext context)
    {
        var leftOrPrimary = context.binaryExpression()[0].Accept(this) as Expression;
        Assert.NotNull(leftOrPrimary);

        var operatorIndex = 1;

        for (int i = 1; i < context.binaryExpression().Length; i++)
        {
            Expression? right = context.binaryExpression()[i].Accept(this) as Expression;
            leftOrPrimary = new BinaryOp(leftOrPrimary, context.GetChild(operatorIndex).GetText(), right)
            {
                LineNum = context.Start.Line
            };
            operatorIndex += 2;
        }

        return leftOrPrimary;
    }
    
       

    public override AstNode VisitBinaryExpression(CARLParser.BinaryExpressionContext context)
    {
        var leftOrPrimary = context.multExpression()[0].Accept(this) as Expression;
        Assert.NotNull(leftOrPrimary);

        var operatorIndex = 1;

        for (int i = 1; i < context.multExpression().Length; i++)
        {
            Expression? right = context.multExpression()[i].Accept(this) as Expression;
            leftOrPrimary = new BinaryOp(leftOrPrimary, context.GetChild(operatorIndex).GetText(), right)
            {
                LineNum = context.Start.Line
            };
            operatorIndex += 2;
        }

        return leftOrPrimary;
    }


    public override AstNode VisitMultExpression(CARLParser.MultExpressionContext context)
    {
        var leftOrPrimary = context.unaryExpression()[0].Accept(this) as Expression;
        Assert.NotNull(leftOrPrimary);

        var operatorIndex = 1;

        for (int i = 1; i < context.unaryExpression().Length; i++)
        {
            Expression? right = context.unaryExpression()[i].Accept(this) as Expression;
            leftOrPrimary = new BinaryOp(leftOrPrimary, context.GetChild(operatorIndex).GetText(), right)
            {
                LineNum = context.Start.Line
            };
            operatorIndex += 2;
        }

        return leftOrPrimary;
    }

    public override AstNode VisitUnaryExpression(CARLParser.UnaryExpressionContext context)
    {
        if (context.children.Count == 1) return base.VisitUnaryExpression(context);

        var rightOrPrimary = context.term().Accept(this) as Expression;

        return new UnaryOp(context.GetChild(0).GetText(), rightOrPrimary)
            { LineNum = context.Start.Line };
    }


    public override AstNode VisitTerm(CARLParser.TermContext context)
    {
        if (context.NUM() != null)
        {
            return new Num(context.NUM().GetText()) {LineNum = context.Start.Line};
        }
        else if (context.GetText() == "true" || context.GetText() == "false")
        {
            return new Bool(context.GetText()) {LineNum = context.Start.Line};
        }
        else if (context.expression() != null)
        {
            return context.expression().Accept(this);
        }
        else
        {
            throw new NotSupportedException($"Term type not supported: {context.GetText()}");
        }
    }

}
