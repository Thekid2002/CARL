using System.Globalization;
using CARL.AST.Expressions;
using CARL.AST.Expressions.Term;
using Expressions_Expression = CARL.AST.Expressions.Expression;
using Term_Num = CARL.AST.Expressions.Term.Num;

namespace CARLLanguageProcessor;

public class Interpreter
{
    public object EvaluateProgram(CARL.AST.Program program)
    {
        return EvaluateExpression(program.Expression);
    }

    public object EvaluateExpression(Expressions_Expression expression)
    {
        switch (expression)
        {
            case Num num:
                return EvaluateLiterals(num);
            case Bool boolean:
                return EvaluateLiterals(boolean);
            case UnaryOp unaryOp:
                var val = EvaluateExpression(unaryOp.Expression);
                return unaryOp.Op switch
                {
                    "-" => -(double)val,
                    "!" => !(bool)val,
                    _ => throw new NotImplementedException()
                };

            case BinaryOp binaryOp:
                var left = EvaluateExpression(binaryOp.Left);
                var right = EvaluateExpression(binaryOp.Right);
                if ((binaryOp.Op == "/" || binaryOp.Op == "%") && (double)right == 0)
                    throw new Exception("Division by zero is not allowed.");
                
                if(binaryOp.Op == "==")
                    return left == right;
                
                if(binaryOp.Op == "!=")
                    return left != right;
                
                if(left is bool && right is bool)
                    return binaryOp.Op switch
                    {
                        "&&" => (bool)left && (bool)right,
                        "||" => (bool)left || (bool)right,
                    };
                
                if(left is double && right is double)
                    return binaryOp.Op switch
                    {
                        "+" => (double)left + (double)right,
                        "-" => (double)left - (double)right,
                        "*" => (double)left * (double)right,
                        "/" => (double)left / (double)right,
                        "%" => (double)left % (double)right,
                        "<" => (double)left < (double)right,
                        ">" => (double)left > (double)right,
                        "<=" => (double)left <= (double)right,
                        ">=" => (double)left >= (double)right,
                    };

                throw new NotImplementedException(binaryOp.Op);
        }

        return null;
    }

    public object? EvaluateLiterals(Expression expression)
    {
        switch (expression)
        {
            case Term_Num num:
                return double.Parse(num.Value, CultureInfo.InvariantCulture);
            case Bool boolean:
                return bool.Parse(boolean.Value);
            default:
                throw new NotImplementedException();
        }
    }
}
