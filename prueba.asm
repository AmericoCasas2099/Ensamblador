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
extern fgets

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
	push x
	push tipo
	call scanf
	mov eax, [x]
	push eax
	push tipo
	call printf
	NEWLINE
; for1
; Asignacion a x
	mov eax, 0
	push eax
	pop eax
	mov dword [x], eax
_ForIni1:
	mov eax, [x]
	push eax
	mov eax, 4
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jle _ForFin1
     jmp _forInstruction1
_forIncremento1:
; Asignacion a x
	inc dword [x]
     jmp _ForIni1
_forInstruction1:
	PRINT_STRING msg2
	mov eax, [x]
	push eax
	push tipo
	call printf
	PRINT_STRING msg3
	NEWLINE
     jmp _forIncremento1
_ForFin1:
	add esp, 4

	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data

tipo db "%d", 0
	x db 0
	msg1 db "Caa" ,0
	msg2 db "x es igual a: " ,0
	msg3 db "" ,0
