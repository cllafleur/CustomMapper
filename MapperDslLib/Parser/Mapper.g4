grammar Mapper;

file
	: ( (WS)* NEWLINE | statement)+
	;

statement
	: extractExpr ASSIGNMENT insertExpr NEWLINE
	;

extractExpr
	: ( expr | tupleOfExpr )
	;

insertExpr
	: expr
	;

expr
	: ( instanceRef | function | LITTERAL )
	;

tupleOfExpr
	: TUPLE_START expr ( TUPLE_SEPARATOR expr )* TUPLE_END
	;

function
	: IDENTIFIER '(' expr ')'
	;

instanceRef
	: IDENTIFIER (DOT IDENTIFIER)*
	;

TUPLE_START
	: '('
	;

TUPLE_END
	: ')'
	;

TUPLE_SEPARATOR
	: ','
	;

ASSIGNMENT
	: ('-' | '=') '>'
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
	: ' '+ -> skip
	;