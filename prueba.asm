Autores: Luis Américo Casas Vázquez,
Habid Hazel Avitud Cruz
Analizador léxico
Analizador Sintactico
Analizador Semántico

%include 'io.inc'
extern fflush
extern printf
extern scanf
extern stdout

section .bss
	input resd 1

segment .text
	global main

main:
; Asignacion a x
	push input
	push tipo
	call scanf
	add esp, 8
	mov eax, [input]
	mov dword[x], eax
	PRINT_STRING msg1
	mov eax, [x]
	push eax
	push tipo
	call printf
	PRINT_STRING msg2
	add esp, 4

	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data

tipo db "%d", 0
	x db 0
	msg1 db "x es igual a: " ,0
	msg2 db "" ,13, 0
