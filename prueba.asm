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
; while 1
_whileIni1:
	mov eax, x
	push eax
	mov eax, 0
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jle _whileFin1
; Asignacion a x
	dec x
jmp _whileIni1
_whileFin1:
	add esp, 4

	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data
	x db 0
	y db 0
