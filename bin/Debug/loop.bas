var n = ""
var n = 0.00650
var m = 1
var rez = 0

input "Ввод вещественного числа:", f

print "Вывод строки текста"
input "Ввод вещественного числа:", n

print "Введено число", n
input "Ввод целого числа:", m

for i = 0 to m
    for j = 0 to m
	print "i = ", i
	print "j = ", j

	if i > 0 then
		if j <= 3 then
		   rez = rez + i*n - j/m
		endif
	else
	   rez = rez - i*n + j/m
	endif

	print "Результат вычислений", rez
    next
next
