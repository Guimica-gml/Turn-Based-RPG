using Godot;

public abstract class State : Node
{
	[Signal] protected delegate void RequestState(string newState);
	
	public abstract void StateReady();
	public abstract void StateProcess(float delta);
	public abstract void StateFree();
}
