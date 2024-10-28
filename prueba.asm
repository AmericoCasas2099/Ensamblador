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
	global _main

_main:
; Asignacion a x
	mov eax, 3
	push eax
	mov eax, 5
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
	div ebx
	push eax
	pop ebx
	pop eax
	sub eax, ebx
	push eax
	pop eax
	mov dword [x], eax
; if1
	mov eax, x
	push eax
	mov eax, 62
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jne ; _if1
jz ; _else 1
; Asignacion a x
	mov eax, 0
	push eax
	pop eax
	mov dword [x], eax
jmp ; _finIf2
; _else 1:
; _finIf2:
; Asignacion a x
	mov eax, 2
	push eax
	pop eax
	add [x], eax
	add esp, 4

	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data
	x db 0
	y db 0
