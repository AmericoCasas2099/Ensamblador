;Autores: Luis Américo Casas Vázquez,
;Habid Hazel Avitud Cruz
;Analizador léxico
;Analizador Sintactico
;Analizador Semántico

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
; Asignacion a y
	mov eax, 10
	push eax
	pop eax
	mov dword [y], eax
; Asignacion a z
	mov eax, 2
	push eax
	pop eax
	mov dword [z], eax
; Asignacion a c
	mov eax, 100
	push eax
	pop eax
	mov dword [c], eax
	PRINT_STRING msg1
; Asignacion a altura
	push input
	push salida
	call scanf
	add esp, 8
	mov eax, [input]
	mov dword[altura], eax
	PRINT_STRING msg2
	NEWLINE
	mov eax, 3
	push eax
	mov eax, [altura]
	push eax
	pop ebx
	pop eax
	add eax, ebx
	push eax
	mov eax, 8
	push eax
	pop ebx
	pop eax
	mul ebx
	push eax
	mov eax, 10
	push eax
	mov eax, 4
	push eax
	pop ebx
	pop eax
	sub eax, ebx
	push eax
	mov eax, 2
	push eax
	pop ebx
	pop eax
	cdq
	idiv ebx
	push eax
	pop ebx
	pop eax
	sub eax, ebx
	push eax
; Asignacion a x
	dec dword [x]
; Asignacion a x
	mov eax, [altura]
	push eax
	mov eax, 8
	push eax
	pop ebx
	pop eax
	mul ebx
	push eax
	pop eax
	add [x], eax
; Asignacion a x
	mov eax, 2
	push eax
	pop eax
	mov ebx, dword [x]
	imul eax, ebx
	mov dword [x], eax
	mov eax, 1
	push eax
; for1
;_AsignacionFor1:
; Asignacion a i
	mov eax, 1
	push eax
	pop eax
	mov dword [i], eax
_ForIni1:
;_CondicionFor1:
	mov eax, [k]
	push eax
	mov eax, [altura]
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jl _ForFin1
; for2
;_AsignacionFor2:
; Asignacion a j
	mov eax, 1
	push eax
	pop eax
	mov dword [j], eax
_ForIni2:
;_CondicionFor2:
	mov eax, [j]
	push eax
	mov eax, [k]
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jl _ForFin2
; if 1
	mov eax, [j]
	push eax
	mov eax, 2
	push eax
	pop ebx
	pop eax
	cdq
	idiv ebx
	push edx
	mov eax, 0
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jne _else1
	PRINT_STRING msg3
	jmp _endIf1
_else1:
	PRINT_STRING msg4
_endIf1:
;_OperacionFor2:
	inc dword [j]
jmp _ForIni2
_ForFin2:
	PRINT_STRING msg5
	NEWLINE
;_OperacionFor1:
	inc dword [k]
jmp _ForIni1
_ForFin1:
; Asignacion a i
	mov eax, 0
	push eax
	pop eax
	mov dword [i], eax
; do 1
_do1:
	PRINT_STRING msg6
; Asignacion a i
	inc dword [i]
	mov eax, [i]
	push eax
	mov eax, [altura]
	push eax
	mov eax, 2
	push eax
	pop ebx
	pop eax
	mul ebx
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jge _do1
	PRINT_STRING msg7
	NEWLINE
; for3
;_AsignacionFor3:
; Asignacion a i
	mov eax, 1
	push eax
	pop eax
	mov dword [i], eax
_ForIni3:
;_CondicionFor3:
	mov eax, [i]
	push eax
	mov eax, [altura]
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jl _ForFin3
; Asignacion a j
	mov eax, 1
	push eax
	pop eax
	mov dword [j], eax
; while 1
_whileIni1:
	mov eax, [j]
	push eax
	mov eax, [i]
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jl _whileFin1
	PRINT_STRING msg8
	mov eax, [j]
	push eax
	push salida
	call printf
; Asignacion a j
	inc dword [j]
jmp _whileIni1
_whileFin1:
	PRINT_STRING msg9
	NEWLINE
;_OperacionFor3:
	inc dword [i]
jmp _ForIni3
_ForFin3:
; Asignacion a i
	mov eax, 0
	push eax
	pop eax
	mov dword [i], eax
; do 2
_do2:
	PRINT_STRING msg10
; Asignacion a i
	inc dword [i]
	mov eax, [i]
	push eax
	mov eax, [altura]
	push eax
	mov eax, 2
	push eax
	pop ebx
	pop eax
	mul ebx
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jge _do2
	PRINT_STRING msg11
	NEWLINE
	add esp, 4

	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data

salida db "%d", 0
	altura db 0
	i db 0
	j db 0
	y db 0
	z db 0
	c db 0
	x db 0
	k db 0
	msg1 db "Valor de altura = " ,0
	msg2 db "" ,0
	msg3 db "*" ,0
	msg4 db "-" ,0
	msg5 db "" ,0
	msg6 db "-" ,0
	msg7 db "" ,0
	msg8 db "" ,0
	msg9 db "" ,0
	msg10 db "-" ,0
	msg11 db "" ,0
