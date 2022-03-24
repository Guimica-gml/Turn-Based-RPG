using Godot;

public abstract class Interaction : Resource
{
	[Signal] protected delegate void Ended();

	public abstract bool CanTrigger();
	public abstract void OnTrigger();
	public abstract void OnEnd();
	public abstract Texture GetIcon();
}
