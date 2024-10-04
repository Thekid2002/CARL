grammar CARL;

//Program
program : expression EOF;

// Expressions
expression : equalityExpression (('||' | '&&') equalityExpression)* ;
equalityExpression : relationExpression (('==' | '!=') relationExpression)* ;
relationExpression : binaryExpression (('<' | '>' | '<=' | '>=') binaryExpression)* ;
binaryExpression : multExpression (('+' | '-') (multExpression))* ;
multExpression : unaryExpression (('*' | '/' | '%' ) unaryExpression)* ;
unaryExpression : ('!' | '-')* term;

//Terms
term : NUM | 'true' | 'false' | '(' expression ')';


NUM : '0' | [0-9]* '.' [0-9]+ | [0-9]+ ;
WS : [ \t\r\n]+ -> skip ; // Ignore/skip whitespace

