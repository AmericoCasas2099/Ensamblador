;Autores: Luis Américo Casas Vázquez,
;Habid Hazel Avitud Cruz
;Analizador léxico
;Analizador Sintactico
;Analizador Semántico
;Analizador Sintactico
;Analizador Semántico

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
; Asignacion a y
	mov eax, 10
	push eax
	pop eax
	mov dword [y], eax
; Fin asignacion a y
; Asignacion a z
	mov eax, 2
	push eax
	pop eax
	mov dword [z], eax
; Fin asignacion a z
; Asignacion a c
	mov eax, 100
	push eax
	pop eax
	mov dword [c], eax
; Fin asignacion a c
	PRINT_STRING msg1
; Asignacion a altura
	push input
	push salida
	call scanf
	add esp, 8
	mov eax, [input]
	mov dword[altura], eax
; Fin asignacion a altura
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
	pop eax
	mov dword [x], eax
	PRINT_STRING msg3
	mov eax, [x]
	push eax
	push salida
	call printf
	NEWLINE
; Asignacion a x
	dec dword [x]
; Fin asignacion a x
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
; Fin asignacion a x
; Asignacion a x
	mov eax, 2
	push eax
	pop eax
	mov ebx, dword [x]
	imul eax, ebx
	mov dword [x], eax
; Fin asignacion a x
	PRINT_STRING msg4
	mov eax, [x]
	push eax
	push salida
	call printf
	NEWLINE
	add esp, 4

	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data

salida db "%d", 0
	altura dd 0
	i dd 0
	j dd 0
	y dw 0 
	z dw 0 
	c db 0
	x dw 0 
	msg1 db "Valor de altura = " ,0
	msg2 db "" ,0
	msg3 db "Valor de x: " ,0
	msg4 db "Valor de x: " ,0
