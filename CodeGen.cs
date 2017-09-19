using Collections = System.Collections.Generic;
using Reflect = System.Reflection;
using IO = System.IO;

public sealed class CodeGen
{
    System.Text.StringBuilder accum = new System.Text.StringBuilder();
    Collections.Dictionary<string, System.Type> varTable;

    public System.Text.StringBuilder Accum
    {
        get { return accum; }
    }

    public CodeGen(Stmt stmt)
    //public CodeGen(Stmt stmt, string moduleName)
    {
        /*if (IO.Path.GetFileName(moduleName) != moduleName)
        {
            throw new System.Exception("can only output into current directory!");
        }*/

        accum.Append("using System;\nusing System.Text;\nusing System.IO;\n\nnamespace b2cCompiler\n{\nclass Program\n{\nstatic void Main(string[] args)\n{");
        this.varTable = new Collections.Dictionary<string, System.Type>();
        this.GenStmt(stmt);
        this.varTable = null;
        accum.Append("\n}\n}\n}");
    }


	private void GenStmt(Stmt stmt)
	{
        //Обработка элемента последовательности дерева
		if (stmt is Sequence)
		{
			Sequence seq = (Sequence)stmt;
			//Выполним обработку левой части дерева
            this.GenStmt(seq.First);
            //Выполним обработку правой части дерева
			this.GenStmt(seq.Second);
		}		
        
        //Обработка элемента дерева - "Объявление"
        else if (stmt is DeclareVar)
		{
			// Добавим переменную в список переменных
			DeclareVar declare = (DeclareVar)stmt;
		    this.varTable[declare.Ident] = this.TypeOfExpr(declare.Expression);

			//Приведем элемент типа "Объевление" к типу "Присвоение"
			Assign assign = new Assign();
			assign.Ident = declare.Ident;
			assign.Expression = declare.Expression;
           
            //Запишем тип переменной
            accum.Append(string.Format("\n{0} ", this.TypeOfExpr(declare.Expression).Name));
            //Запустим на обработку
            this.GenStmt(assign);
		}

        //Обработка элемента дерева - "Присвоение"
        else if (stmt is Assign)
        {
	        Assign assign = (Assign)stmt;
            accum.Append(string.Format("{0}=", assign.Ident));

            //Обработка правой части элемента
            this.GenAssign(assign.Expression);
            accum.Append(";");
        }
        
        //Обработка элемента дерева - "Вывод данных"
        else if (stmt is Print)
        {
            Print print = (Print) stmt;            
            accum.Append(print.VarExpression != null
                             ? string.Format("\nConsole.WriteLine(\"{0}\", {1});", this.GetExprValue(print.Expression),
                                             this.GetExprValue(print.VarExpression))
                             : string.Format("\nConsole.WriteLine(\"{0}\");", this.GetExprValue(print.Expression)));            
        }

        //Обработка элемента дерева - "Ввод данных"
        else if (stmt is ReadValue)
        {
            ReadValue readValue = (ReadValue)stmt;
            accum.Append(readValue.Exp != null
                             ? string.Format("\n{0} = Console.ReadLine(\"{1}\");", readValue.Ident, this.GetExprValue(readValue.Exp))
                             : string.Format("\n{0} = Console.ReadLine();", readValue.Ident));
            
            //Проверка, что переменная объявлена ранее
            //CheckVariable            
        }
        else if (stmt is IfElse)
        {
            IfElse ifElse = (IfElse) stmt;
            string operation = string.Empty;

            switch (ifElse.Condition.Operation)
            {
                case ConOperation.Equal: operation = "="; break;
                case ConOperation.Less: operation = "<"; break;
                case ConOperation.LessEqual: operation = "<="; break;
                case ConOperation.More: operation = ">"; break;
                case ConOperation.MoreEqual: operation = ">="; break;
            }                        
            accum.Append(string.Format("\nif ({0}{1}{2})", this.GetExprValue(ifElse.Condition.Left), operation, this.GetExprValue(ifElse.Condition.Right)));
            

            if (ifElse.BodyThen != null)
            {
                accum.Append("\n{\n");
                this.GenStmt(ifElse.BodyThen);
                accum.Append("\n}");
            }

            if (ifElse.BodyElse != null)
            {
                if(ifElse.BodyThen == null)
                    throw new System.Exception("error if - else");
                accum.Append("\nelse\n{\n");
                this.GenStmt(ifElse.BodyElse);
                accum.Append("\n}");
            }
        }
        else if (stmt is ForNext)
        {
            ForNext forNext = (ForNext) stmt;

            accum.Append(string.Format("\nfor("));
            Assign assign = new Assign();
            assign.Ident = forNext.Ident;
            assign.Expression = forNext.From;
            this.GenStmt(assign);
            accum.Append(string.Format("{0}<{1};{2}++)", forNext.Ident, this.GetExprValue(forNext.To), forNext.Ident));
            
            this.varTable[forNext.Ident] = typeof(int);
            
            if (forNext.Body != null)
            {
                accum.Append("\n{");
                this.GenStmt(forNext.Body);
                accum.Append("\n}");
            }
        }
        else
        {
            throw new System.Exception("Отсутствует инструкция для генерирования операции: " + stmt.GetType().Name);
        }
	}

    private void CheckVariable(string name, System.Type type)
    {/*
        if (this.symbolTable.ContainsKey(name))
        {
            Emit.LocalBuilder locb = this.symbolTable[name];

            if (locb.LocalType != type)
            {
                throw new System.Exception("'" + name + "' is of type " + locb.LocalType.Name + " but attempted to store value of type " + type.Name);
            }
        }
        else
        {
            throw new System.Exception("Использование необъявленной переменной '" + name + "'");
        }
      * */
    }    

    private void GenAssign(Expression expression)
    {
        if (expression is BinExpression)
        {
            BinExpression binExpression = (BinExpression) expression;
            //Запишем левую часть выражения (число/переменная)
            accum.Append(string.Format("{0}", GetExprValue(binExpression.Left)));
            //Запишем знак операции
            switch (binExpression.Operation)
            {
                case BinOperation.Add: accum.Append("+"); break;
                case BinOperation.Sub: accum.Append("-"); break;
                case BinOperation.Mul: accum.Append("*"); break;
                case BinOperation.Div: accum.Append("/"); break;
            }
            //Обработаем правую часть выражения
            this.GenAssign(binExpression.Right);
        }
        else if (expression is Expression)
        {
            Expression expr = (Expression)expression;
            accum.Append(string.Format("{0}", GetExprValue(expr)));
        }
        else
        {
            throw new System.Exception("Error 5");
        }
    }   

    /// <summary></summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    private string GetExprValue(Expression expression)
    {
        System.Type deliveredType;

        if (expression is StringLiteral)
        {
            deliveredType = typeof(string);
            return string.IsNullOrEmpty(((StringLiteral)expression).Value) ? "\"\"" : ((StringLiteral)expression).Value;
        }
        else if (expression is IntLiteral)
        {
            deliveredType = typeof(int);
            return ((IntLiteral)expression).Value.ToString();
        }
        else if (expression is DoubleLiteral)
        {
            deliveredType = typeof(double);
            return ((DoubleLiteral)expression).Value.ToString();
        }

        else if (expression is Variable)
        {
            string ident = ((Variable)expression).Ident;
            deliveredType = this.TypeOfExpr(expression);           

            if (!this.varTable.ContainsKey(ident))
            {
                throw new System.Exception("Использование необъявленной переменной '" + ident + "'");
            }

            return ident;
        }
        else
        {
            throw new System.Exception("Отсутствуют инструкции для обработки " + expression.GetType().Name);
        }       
    }


/*
    private void GenExpr(Expression expression, System.Type expectedType)
	{
		System.Type deliveredType;
		
        if (expression is StringLiteral)
		{
			deliveredType = typeof(string);			
		}
		else if (expression is IntLiteral)
		{
			deliveredType = typeof(int);			
		}
        else if (expression is DoubleLiteral)
        {
            deliveredType = typeof(double);            
        }
        else if (expression is Variable)
		{
			string ident = ((Variable)expression).Ident;
			deliveredType = this.TypeOfExpr(expression);

			if (!this.symbolTable.ContainsKey(ident))
			{
                throw new System.Exception("Использование необъявленной переменной '" + ident + "'");
			}
		}
		else
		{
			throw new System.Exception("don't know how to generate " + expression.GetType().Name);
		}

        if (deliveredType != expectedType)
        {
            if (deliveredType == typeof(int) &&
                expectedType == typeof(string))
            {
                this.il.Emit(Emit.OpCodes.Box, typeof(int));
                this.il.Emit(Emit.OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
            }
            else
            {
                throw new System.Exception("can't coerce a " + deliveredType.Name + " to a " + expectedType.Name);
            }
        }

	}
*/
    /// <summary>Возвращает тип объекта</summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    private System.Type TypeOfExpr(Expression expression)
	{
		if (expression is StringLiteral)
		{
			return typeof(string);
		}
		else if (expression is IntLiteral)
		{
			return typeof(int);
		}
        else if (expression is DoubleLiteral)
        {
            return typeof(double);
        }
		else if (expression is Variable)
		{
            Variable var = (Variable)expression;

            if (this.varTable.ContainsKey(var.Ident))
            {
                return this.varTable[var.Ident];
            }
            else
            {
                throw new System.Exception("Использование необъявленной переменной '" + var.Ident + "'");                
            }            
		}
		else
		{
			throw new System.Exception("Ошибка определения типа выражения " + expression.GetType().Name);
		}
	}	
}
