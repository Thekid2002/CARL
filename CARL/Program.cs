// See https://aka.ms/new-console-template for more information


using Antlr4.Runtime;
using CARL;
using CARLLanguageProcessor;
using CARLLanguageProcessor.Frontend;

Main(new[] { "Frontend/test.carl" });


static void Main(string[] args)
{
    var fileContents = File.ReadAllText(args[0]);
    Console.WriteLine("Input: " + fileContents); // Debug: Print the input

    var inputStream = CharStreams.fromString(fileContents);
    var lexer = new CARLLexer(inputStream);
    var errorListener = new ParserErrorListener();
    var tokenStream = new CommonTokenStream(lexer);
    var parser = new CARLParser(tokenStream);
    parser.RemoveErrorListeners();
    parser.AddErrorListener(errorListener);

    var parseTree = parser.program();
    Console.WriteLine("Parse Tree: " + parseTree.ToStringTree(parser));
    if(parseTree == null)
    {
        Console.WriteLine("Parse Tree is null. Check the parser.");
        return;
    }

    Console.WriteLine("Parse Tree: " + parseTree.ToString()); 
    
    errorListener.StopIfErrors();

    var ast = parseTree.Accept(new ToAstVisitor());
    if (ast == null)
    {
        Console.WriteLine("AST is null. Check ToAstVisitor.");
        return;
    }

    Console.WriteLine("AST: " + ast); // Debug: Print the AST

    var interpreter = new Interpreter();
    var finalValue = interpreter.EvaluateProgram(ast as CARL.AST.Program);
    Console.WriteLine("Final Value: " + finalValue); 
}
