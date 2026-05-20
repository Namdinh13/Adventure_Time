public class FuncPredicate : IPredicate
{
    private readonly System.Func<bool> func;
    public FuncPredicate(System.Func<bool> func)
    {
        this.func = func;
    }
    public bool Evaluate()
    {
        return func.Invoke();
    }
}