using Collections = System.Collections.Generic;
using Text = System.Text;

public sealed class Parser
{
	private int index;
	private Collections.IList<object> tokens;
	private readonly Stmt result;

	public Parser(Collections.IList<object> tokens)
	{
		this.tokens = tokens;
		this.index = 0;
		this.result = this.ParseStmt();
		
		if (this.index != this.tokens.Count)
			throw new System.Exception("expected EOF");
	}

	public Stmt Result
	{
		get { return result; }
	}

    private Stmt ParseStmt()
    {
        Stmt result;

		if (this.index == this.tokens.Count)
		{
			throw new System.Exception("expected statement, got EOF");
		}

        // <stmt> := print <expr> 

        // <expr> := <string>
	    // | <int>
	    // | <arith_expr>
	    // | <ident>
		if (this.tokens[this.index].Equals("print"))
		{
			this.index++;
			Print print = new Print();

            //Если есть текстовое описание
            if (this.index < this.tokens.Count &&
                this.tokens[this.index] is Text.StringBuilder)
            {
                print.Expression = this.ParseExpr();
            }

            if (this.index < this.tokens.Count &&
                this.tokens[this.index] is string)
            {
                print.VarExpression = this.ParseExpr();            
            }
			
			result = print;
		}
		else if (this.tokens[this.index].Equals("var"))
		{
			this.index++;
			DeclareVar declareVar = new DeclareVar();

			if (this.index < this.tokens.Count &&
				this.tokens[this.index] is string)
			{
				declareVar.Ident = (string)this.tokens[this.index];
			}
			else
			{
				throw new System.Exception("expected variable name after 'var'");
			}

			this.index++;

			if (this.index == this.tokens.Count ||
				this.tokens[this.index] != Scanner.Equal)
			{
				throw new System.Exception("expected = after 'var ident'");
			}

			this.index++;

			declareVar.Expression = this.ParseExpr();
			result = declareVar;
		}
        else if (this.tokens[this.index].Equals("input"))
        {
            this.index++;
            ReadValue readValue = new ReadValue();

            //Если есть текстовое описание
            if (this.index < this.tokens.Count &&
                this.tokens[this.index] is Text.StringBuilder)
            {
                readValue.Exp = this.ParseExpr();
            }
            
            if (this.index < this.tokens.Count &&
                this.tokens[this.index] is string)
            {
                readValue.Ident = (string)this.tokens[this.index++];
                result = readValue;
            }
            else
            {
                throw new System.Exception("expected variable name after 'input'");
            }
        }
        /*else if (this.tokens[this.index].Equals("read_int"))
		{
			this.index++;
			ReadValue readValue = new ReadValue();

			if (this.index < this.tokens.Count &&
				this.tokens[this.index] is string)
			{
				readValue.Ident = (string)this.tokens[this.index++];
				result = readValue;
			}
			else
			{
				throw new System.Exception("expected variable name after 'read_int'");
			}
		}*/
		else if (this.tokens[this.index].Equals("for"))
		{
			this.index++;
			ForNext forNext = new ForNext();

			if (this.index < this.tokens.Count &&
				this.tokens[this.index] is string)
			{
				forNext.Ident = (string)this.tokens[this.index];
			}
			else
			{
				throw new System.Exception("expected identifier after 'for'");
			}

			this.index++;

			if (this.index == this.tokens.Count ||
				this.tokens[this.index] != Scanner.Equal)
			{
				throw new System.Exception("for missing '='");
			}

			this.index++;

			forNext.From = this.ParseExpr();

			if (this.index == this.tokens.Count ||
				!this.tokens[this.index].Equals("to"))
			{
				throw new System.Exception("expected 'to' after for");
			}

			this.index++;

			forNext.To = this.ParseExpr();

			/*if (this.index == this.tokens.Count ||
				!this.tokens[this.index].Equals("do"))
			{
				throw new System.Exception("expected 'do' after from expression in for next");
			}*/

			this.index++;

			forNext.Body = this.ParseStmt();
			result = forNext;

			if (this.index == this.tokens.Count ||
				!this.tokens[this.index].Equals("next"))
			{
				throw new System.Exception("unterminated 'for' loop body");
			}

			this.index++;
		}
        else if (this.tokens[this.index].Equals("if"))
        {
            this.index++;
            IfElse ifthen = new IfElse();
            ifthen.Condition = new ConExpression();

            //Запишем первый операнд в условии
            if (this.index < this.tokens.Count &&
                this.tokens[this.index] is string)
            {
                ifthen.Condition.Left = this.ParseExpr();
            }
            else
            {
                throw new System.Exception("expected identifier after 'if'");
            }

            // Запишем условный оператор            
            if (this.tokens[this.index] == Scanner.More) //Больше
            {
                ifthen.Condition.Operation = ConOperation.More;
            }
            else if (this.tokens[this.index] == Scanner.MoreEqual) //Больше-равно
            {
                ifthen.Condition.Operation = ConOperation.MoreEqual;
            }
            else if (this.tokens[this.index] == Scanner.Less) //Меньше
            {
                ifthen.Condition.Operation = ConOperation.Less;
            }
            else if (this.tokens[this.index] == Scanner.LessEqual) //Меньше-равно
            {
                ifthen.Condition.Operation = ConOperation.LessEqual;
            }
            else if (this.tokens[this.index] == Scanner.Equal) //Равенство
            {
                ifthen.Condition.Operation = ConOperation.Equal;
            }
            else
            {
                throw new System.Exception("missing condition operator");
            }

            //Запишем второй операнд в условии
            this.index++;
            ifthen.Condition.Right = this.ParseExpr();

            if (this.index == this.tokens.Count ||
                !this.tokens[this.index].Equals("then"))
            {
                throw new System.Exception("expected 'then' after if");
            }

            this.index++;
            ifthen.BodyThen = this.ParseStmt();
            
            if (this.index < this.tokens.Count && this.tokens[this.index].Equals("else"))
            {
                this.index++;
                ifthen.BodyElse = this.ParseStmt();
            }

            result = ifthen;

            if (this.index == this.tokens.Count ||
                !this.tokens[this.index].Equals("endif"))
            {
                throw new System.Exception("unterminated 'if' body");
            }

            this.index++;
        }
        else if (this.tokens[this.index] is string)
        {
            //---------------------------------------------------------
            // assignment
            Assign assign = new Assign();
            assign.Ident = (string) this.tokens[this.index++];

            if (this.index == this.tokens.Count ||
                this.tokens[this.index] != Scanner.Equal)
            {
                throw new System.Exception("expected '='");
            }

            this.index++;

            assign.Expression = this.ParseExpr();

            //Проверим наличие операции

            if (this.tokens[this.index] != Scanner.Semi)
            {
                assign.Expression = this.ParseBinExpr(assign.Expression);
            }

            result = assign;
        }
        else
        {
            throw new System.Exception("parse error at token " + this.index + ": " + this.tokens[this.index]);
        }


        if (this.index < this.tokens.Count && this.tokens[this.index] == Scanner.Semi)
		{
			this.index++;

			if (this.index < this.tokens.Count &&
                !this.tokens[this.index].Equals("next") && !this.tokens[this.index].Equals("endif") && !this.tokens[this.index].Equals("else"))
			{
				Sequence sequence = new Sequence();
				sequence.First = result;
				sequence.Second = this.ParseStmt();
				result = sequence;
			}
		}

        return result;
    }

    private BinExpression ParseBinExpr(Expression expr)
    {
        if (this.index == this.tokens.Count)
        {
            throw new System.Exception("expected expression, got EOF");
        }

        BinExpression binResult = new BinExpression();

        //Запишем первый операнд
        binResult.Left = expr;

        // Запишем операцию
        if (this.tokens[this.index] == Scanner.Add) //Сложение
        {
            binResult.Operation = BinOperation.Add;
        }
        else if (this.tokens[this.index] == Scanner.Div) //Сложение
        {
            binResult.Operation = BinOperation.Div;
        }
        else if (this.tokens[this.index] == Scanner.Mul) //Сложение
        {
            binResult.Operation = BinOperation.Mul;
        }
        else if (this.tokens[this.index] == Scanner.Sub) //Сложение
        {
            binResult.Operation = BinOperation.Sub;
        }
        else
        {
            throw new System.Exception("missing bin operator");
        }

        // Запишем второй операнд
        this.index++;
        binResult.Right = this.ParseExpr();

        if (this.tokens[this.index] != Scanner.Semi && this.index < this.tokens.Count)
        {
            binResult.Right = this.ParseBinExpr(binResult.Right);
        }

        return binResult;
    }

    private Expression ParseExpr()
    {
		if (this.index == this.tokens.Count)
		{
			throw new System.Exception("expected expression, got EOF");
		}

		if (this.tokens[this.index] is Text.StringBuilder)
		{
			string value = ((Text.StringBuilder)this.tokens[this.index++]).ToString();
			StringLiteral stringLiteral = new StringLiteral();
			stringLiteral.Value = value;
			return stringLiteral;
		}
		else if (this.tokens[this.index] is int)
		{
			int intValue = (int)this.tokens[this.index++];
			IntLiteral intLiteral = new IntLiteral();
			intLiteral.Value = intValue;
			return intLiteral;
		}
        else if (this.tokens[this.index] is double)
        {
            double doubleValue = (double)this.tokens[this.index++];
            DoubleLiteral doubleLiteral = new DoubleLiteral();
            doubleLiteral.Value = doubleValue;
            return doubleLiteral;
        }        
		else if (this.tokens[this.index] is string)
		{
			string ident = (string)this.tokens[this.index++];
			Variable var = new Variable();
			var.Ident = ident;
			return var;
		}
		else
		{
            throw new System.Exception("expected string literal, int literal, double literal or variable");
		}
    }

}
