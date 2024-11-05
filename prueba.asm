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
	mov eax, 4
	push eax
	pop eax
	mov dword [x], eax
	mov eax, [x]
	push eax
	push tipo
	call printf
	NEWLINE
	PRINT_STRING msg1
	NEWLINE
	add esp, 4

	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data

tipo db "%d", 0
	x db 0
	msg1 db "Caa" ,0
