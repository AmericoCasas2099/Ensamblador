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
	pop eax
	mov dword [x], eax
; Asignacion a y
	mov eax, 0
	push eax
	pop eax
	mov dword [y], eax
; for2
_ForIni2:
; Asignacion a y
	mov eax, 0
	push eax
	pop eax
	mov dword [y], eax
	mov eax, y
	push eax
	mov eax, 3
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jge _ForFin2
; Asignacion a y
	inc dword [y]
; Asignacion a x
	mov eax, 1
	push eax
	pop eax
	add [x], eax
jmp _ForIni2
_ForFin2:
	add esp, 4

	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data
	x db 0
	y db 0
