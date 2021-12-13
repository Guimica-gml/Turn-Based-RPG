using Godot;

public abstract class ItemStats : Resource
{
	[Export] public bool KeyItem;
	[Export] public string Name;
	[Export] public Texture Texture;
	[Export(PropertyHint.MultilineText)] public string Description;
	[Export] public bool Stackable = true;
	[Export] public bool UseJustInBattle = false;
	[Export] public int Price = 1;
	public int Amount = 1;
	
	protected Stats playerStats = null;
	
	public abstract void Use();
	
	public ItemStats()
	{
		playerStats = GD.Load("res://Stats/PlayerStats.tres") as Stats;
	}
}
