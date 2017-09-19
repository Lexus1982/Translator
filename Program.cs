using System;
using System.Text;
using System.IO;

namespace GfnCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            /*if (args.Length != 1)
            {
                Console.WriteLine("Usage: gfn.exe program.gfn");
                return;
            }*/

            try
            {
                Scanner scanner = null;

                //������ ������� �������� ���������
                using (TextReader input = File.OpenText("loop.bas"))
                //using (TextReader input = File.OpenText(args[0]))
                {
                    scanner = new Scanner(input);
                }
                
                //������ ������� ������ (����������� ������)
                Parser parser = new Parser(scanner.Tokens);
                //������ ���������� ������������� ���
                CodeGen codeGen = new CodeGen(parser.Result);
                //CodeGen codeGen = new CodeGen(parser.Result, Path.GetFileNameWithoutExtension("loop.bas") + ".cs");
                
                //�������� ��������������� ����� � �����
                //File.WriteAllText("loop.cs", codeGen.Accum.ToString(), Encoding.Unicode);
                File.WriteAllText(Path.GetFileNameWithoutExtension(args[0]) + ".cs", codeGen.Accum.ToString(), Encoding.Unicode);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);                
            }
            Console.Write("���������� ���������. �������� ����: " + Path.GetFileNameWithoutExtension(args[0]) + ".cs");
            Console.Read();           
        }
    }
}
