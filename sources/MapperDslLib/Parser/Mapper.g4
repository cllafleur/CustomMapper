grammar Mapper;

file
	: ( WS | WS? NEWLINE | statement)+
	;

statement
	: extractExpr WS? ASSIGNMENT WS? insertExpr WS? NEWLINE
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
	: ( tupleOfExpr | returnFunctionDereferencement | expr )
	;

tupleOfExpr
	: '(' namedExpr ( ',' namedExpr )* WS? ')'
	;

namedExpr
	: WS? ( IDENTIFIER WS? ':' )? WS? (returnFunctionDereferencement | expr) WS?
	;

function
	: IDENTIFIER WS? '(' WS? complexExpr ( WS? ',' WS? complexExpr )* WS? ')'
	;

returnFunctionDereferencement
	: function ( '.' instanceRef )
	;

instanceRef
	: fieldInstanceRef (DOT fieldInstanceRef)*
	;

fieldInstanceRef
	: IDENTIFIER
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
	: [ \t]+
	;
