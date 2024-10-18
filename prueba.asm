Autores: Don guamas y El greñas
Analizador léxico
Analizador Sintactico
Analizador Semántico
; Asignacion a x
	mov ax, 3
	push
	mov ax, 5
	push
	pop bx
	pop ax
	add ax, bx
	push ax
	mov ax, 8
	push
	pop bx
	pop ax
	mul bx
	push ax
	mov ax, 10
	push
	mov ax, 4
	push
	pop bx
	pop ax
	sub ax, bx
	push ax
	mov ax, 2
	push
	pop bx
	pop ax
	div bx
	push ax
	pop bx
	pop ax
	sub ax, bx
	push ax
	pop ax
	mov x, ax
; if1
	mov ax, x
	push
	mov ax, 62
	push
	pop ax
	pop bx
	cmp ax, bx
	jne 
; Asignacion a x
	mov ax, 0
	push
	pop ax
	mov x, ax
; if2
	mov ax, x
	push
	mov ax, 0
	push
	pop ax
	pop bx
	cmp ax, bx
	je
; Asignacion a x
	mov ax, 1
	push
	pop ax
	mov x, ax
; _if2:
; _if1:

segment .data
	x db 0
	y db 0
