grammar Mapper;

file
	: ( (WS)* NEWLINE | statement)+
	;

statement
	: expr ASSIGNMENT expr NEWLINE
	;

expr
	: ( instanceRef | function | LITTERAL )
	;

function
	: IDENTIFIER '(' expr ')'
	;

instanceRef
	: IDENTIFIER (DOT IDENTIFIER)*
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