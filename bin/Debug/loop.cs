using System;
using System.Text;
using System.IO;

namespace b2cCompiler
{
class Program
{
static void Main(string[] args)
{
String n="";
Double n=0,0065;
Int32 m=1;
Int32 rez=0;
f = Console.ReadLine("Ввод вещественного числа:");
Console.WriteLine("Вывод строки текста");
n = Console.ReadLine("Ввод вещественного числа:");
Console.WriteLine("Введено число", n);
m = Console.ReadLine("Ввод целого числа:");
for(i=0;i<m;i++)
{
for(j=0;j<m;j++)
{
Console.WriteLine("i = ", i);
Console.WriteLine("j = ", j);
if (i>0)
{

if (j<=3)
{
rez=rez+i*n-j/m;
}
}
else
{
rez=rez-i*n+j/m;
}
Console.WriteLine("Результат вычислений", rez);
}
}
}
}
}