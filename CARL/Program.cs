// See https://aka.ms/new-console-template for more information


using Antlr4.Runtime;
using CARL;
using CARLLanguageProcessor;
using CARLLanguageProcessor.Frontend;

Main(new[] { "Frontend/test.carl" });


static void Main(string[] args)
{
    var fileContents = File.ReadAllText(args[0]);
    var inputStream = CharStreams.fromString(fileContents);
    var lexer = new CARLLexer(inputStream);

    var errorListener = new ParserErrorListener();
    var tokenStream = new CommonTokenStream(lexer);
    var parser = new CARLParser(tokenStream);
    parser.RemoveErrorListeners();
    parser.AddErrorListener(errorListener);
    var parseTree = parser.program();
    errorListener.StopIfErrors();

    var ast = parseTree.Accept(new ToAstVisitor());
    Console.WriteLine(ast);
    var interpreter = new Interpreter();
    var finalValue = interpreter.EvaluateProgram(ast as CARL.AST.Program);
    Console.WriteLine(finalValue);
}
