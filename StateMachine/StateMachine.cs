using Godot;
using Godot.Collections;

public class StateMachine : Node
{
	[Export] private string _firstState = "";
	
	public Dictionary<string, State> States = new Dictionary<string, State>();
	public State CurrentState = null;
	
	public override void _Ready()
	{
		var children = new Array<State>(GetChildren());
		foreach (var state in children)
		{
			state.Connect("RequestState", this, nameof(ChangeState));
			States[state.Name] = state;
		}
		
		if (_firstState != "") ChangeState(_firstState);
	}
	
	public override void _Process(float delta)
	{
		CurrentState?.StateProcess(delta);
	}
	
	public void ChangeState(string newState)
	{
		CurrentState?.StateFree();
		CurrentState = States[newState];
		CurrentState?.CallDeferred("StateReady");
	}
}
