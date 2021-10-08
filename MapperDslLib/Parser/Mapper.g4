grammar Mapper;

file
	: ( (WS)* NEWLINE | statement)+
	;

statement
	: extractExpr ASSIGNMENT insertExpr NEWLINE
	;

extractExpr
	: complexExpr
	;

insertExpr
	: instanceRef | function
	;

expr
	: ( instanceRef | function | LITTERAL )
	;

complexExpr
	: ( tupleOfExpr | expr )
	;

tupleOfExpr
	: '(' namedExpr ( ',' namedExpr )* ')'
	;

namedExpr
	: ( IDENTIFIER ':' )? expr
	;

function
	: IDENTIFIER '(' complexExpr ( ',' complexExpr )* ')'
	;

instanceRef
	: IDENTIFIER (DOT IDENTIFIER)*
	;

ASSIGNMENT
	: '->' | '=>'
	;

DOT
	: '.'
	;

LITTERAL
	: '"' ~ ["]* '"'
	;

NEWLINE
	: '\r'? '\n'
	;

IDENTIFIER
	: [_a-zA-Z] [_0-9a-zA-Z]*
	;

LINE_COMMENT
	: '#' ~[\r\n]* -> skip
	;

WS
	: [ \t]+ -> skip
	;
