using System;
using System.Collections.Generic;
using System.Linq;

public class StateMashine {
    private State currentState;
    private Dictionary<Type, List<Transition>> transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> currentTransitions = new List<Transition>();
    private List<Transition> anyTransitions = new List<Transition>();
    private static List<Transition> EmptyTransitions = new List<Transition>(0);

    public void Tick() {
        Transition transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        currentState?.Tick();
    }

    public void SetState(State state) {
        if (state == currentState)
            return;
        currentState?.OnExit();
        currentState = state;

        transitions.TryGetValue(currentState.GetType(), out currentTransitions);
        if (currentTransitions == null)
            currentTransitions = EmptyTransitions;

        currentState.OnEnter();
    }

    public void AddTransition(State from, State to, Func<bool> predicate) {
        if (!this.transitions.TryGetValue(from.GetType(), out List<Transition> transitions)) {
            transitions = new List<Transition>();
            this.transitions[from.GetType()] = transitions;
        }
        transitions.Add(new Transition(to, predicate));
    }

    public void AddAnyTransition(State state, Func<bool> predicate) {
        anyTransitions.Add(new Transition(state, predicate));
    }

    private Transition GetTransition() {
        foreach(Transition transition in anyTransitions) {
            if (transition.Condition())
                return transition;
        }

        foreach (Transition transition in currentTransitions) {
            if (transition.Condition())
                return transition;
        }

        return null;
    }
}
