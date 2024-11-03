Autores: Luis Américo Casas Vázquez,
Habid Hazel Avitud Cruz
Analizador léxico
Analizador Sintactico
Analizador Semántico

extern fflush
extern printf
extern scanf
extern stdout

segment .text
	global main

main:
	push ebp
	mov ebp, esp
	push msg1
	call printf
	mov esp, ebp
	pop ebp
	add esp, 4

	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data
	msg1 db ")", 0
