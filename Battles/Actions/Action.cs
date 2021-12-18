using Godot;

public class Action : Resource
{
	[Export] public string Name = "";
	[Export(PropertyHint.File, "*.tscn")] public string AnimScenePath = "";
	[Export] public bool Heal = false;
	[Export] public int Value = 1;
	
	~Action()
	{
		Dispose();
	}
}
