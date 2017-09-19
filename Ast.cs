
/* <stmt> := var <ident> = <expr>
	| <ident> = <expr>
	| for <ident> = <expr> to <expr> do <stmt> end
	| read_int <ident>
	| print <expr>
	| <stmt> ; <stmt>
  */
public abstract class Stmt
{
}

// var <ident> = <expr>
public class DeclareVar : Stmt
{
    public string Ident;
    public Expression Expression;
}

// print [expr] <expr>
public class Print : Stmt
{
    public Expression Expression;
    public Expression VarExpression;
}

// <ident> = <expr> | <bin_expr>
public class Assign : Stmt
{
    public string Ident;
    public Expression Expression;
}

// for <ident> = <expr> to <expr> do <stmt> end
public class ForNext : Stmt
{
    public string Ident;
    public Expression From;
    public Expression To;
    public Stmt Body;
}

// if <expr> <con_op> <expr> then <stmt> [else <stmt>] endif
public class IfElse : Stmt
{    
    public ConExpression Condition;    
    public Stmt BodyThen;
    public Stmt BodyElse;
}

// read_value <expr> <ident>
public class ReadValue : Stmt
{
    public string Ident;
    public Expression Exp;
}

// <stmt> ; <stmt>
public class Sequence : Stmt
{
    public Stmt First;
    public Stmt Second;
}

/* <expr> := <string>
 *  | <int>
 *  | <arith_expr>
 *  | <ident>
 */
public abstract class Expression
{
}

// <string> := " <string_elem>* "
public class StringLiteral : Expression
{
	public string Value;
}

// <int> := <digit>+
public class IntLiteral : Expression
{
	public int Value;
}

// <double> := <digit>+
public class DoubleLiteral : Expression
{
    public double Value;
}

// <ident> := <char> <ident_rest>*
// <ident_rest> := <char> | <digit>
public class Variable : Expression
{
	public string Ident;
}

// <bin_expr> := <expr> <bin_op> <expr>
public class BinExpression : Expression
{
	public Expression Left;
	public Expression Right;
	public BinOperation Operation;
}

/// <summary>Перечисление выполняемых операций</summary>
public enum BinOperation
{
    /// <summary>Сложение</summary>
	Add,
    /// <summary>Вычитание</summary>
	Sub,
    /// <summary>Умножение</summary>
    Mul,
    /// <summary>Деление</summary>
	Div
}

// <con_expr> := <expr> <con_op> <expr>
public class ConExpression : Expression
{
    public Expression Left;
    public Expression Right;
    public ConOperation Operation;
}

/// <summary>Перечисление условных операций</summary>
public enum ConOperation
{
    /// <summary>Больше</summary>
    More,
    /// <summary>Меньше</summary>
    Less,
    /// <summary>Больше - равно</summary>
    MoreEqual,
    /// <summary>Меньше - равно</summary>
    LessEqual,
    /// <summary>Равно</summary>
    Equal
}
