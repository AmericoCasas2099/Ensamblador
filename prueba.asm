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

segment .text
	global main

main:
; Asignacion a x
	mov eax, 2
	push eax
	pop eax
	mov dword [x], eax
	PRINT_STRING msg1
	PRINT_STRING msg2
	mov eax, [x]
	push eax
	push format
	call printf
	PRINT_STRING msg3
	add esp, 4

	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data

format db "%d", 0
	x db 0
	msg1 db "Holaa " ,0
	msg2 db "amigo " ,0
	msg3 db "xd" ,13, 0
