using Godot;

/// <summary>
/// It's just like Resource, but it has an _Init() method that you can override
/// </summary>
public class MyResource : Resource
{
	[Signal] private delegate void Constructed();

	public MyResource()
	{
		Global.InitResource(this);
	}

	public virtual void _Init()
	{
	}
}
