Autores: Don guamas y El greñas
Analizador léxico
Analizador Sintactico
Analizador Semántico
; Asignacion a x
	mov eax, 3
	push
	mov eax, 5
	push
	pop ebx
	pop eax
	add eax, ebx
	push ax
	mov eax, 8
	push
	pop ebx
	pop eax
	mul ebx
	push eax
	mov eax, 10
	push
	mov eax, 4
	push
	pop ebx
	pop eax
	sub eax, ebx
	push eax
	mov eax, 2
	push
	pop ebx
	pop eax
	div ebx
	push eax
	pop ebx
	pop eax
	sub eax, ebx
	push eax
	pop eax
	mov x, eax
; if1
	mov eax, x
	push
	mov eax, 62
	push
	pop eax
	pop ebx
	cmp eax, ebx
	jne 
; Asignacion a x
	mov eax, 0
	push
	pop eax
	mov x, eax
; if2
	mov eax, x
	push
	mov eax, 0
	push
	pop eax
	pop ebx
	cmp eax, ebx
	je
; Asignacion a x
	mov eax, 1
	push
	pop eax
	mov x, eax
; _if2:
; _if1:

segment .data
	x db 0
	y db 0
