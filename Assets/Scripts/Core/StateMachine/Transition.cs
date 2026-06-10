public class Transition : ITransition
{
    public IPredicate Condition { get; }
    public IState To { get; }
    public Transition(IPredicate condition, IState to)
    {
        Condition = condition;
        To = to;
    }
}
