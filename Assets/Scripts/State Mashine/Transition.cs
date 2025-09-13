using System;

public class Transition {
    public Func<bool> Condition { get; private set; }
    public State To { get; private set; }

    public Transition(State to, Func<bool> condition) {
        To = to;
        Condition = condition;
    }
}
