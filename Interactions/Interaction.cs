using Godot;

public abstract class Interaction : Resource
{
	[Signal] protected delegate void Ended();
	
	public abstract bool CanTrigger();
	public abstract void OnInteractionTrigger();
	public abstract void OnInteractionEnd();
	public abstract Texture GetIcon();
}
