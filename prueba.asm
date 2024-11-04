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
	mov eax, [x]
	push eax
	push format
	call printf
	PRINT_STRING msg1
; if 1
	mov eax, [x]
	push eax
	mov eax, 3
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jge _else1
; Asignacion a x
	mov eax, 0
	push eax
	pop eax
	mov dword [x], eax
	jmp _endIf1
_else1:
_endIf1:
	add esp, 4

	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data

format db "%d", 0
	x db 0
	msg1 db "" ,13, 0
