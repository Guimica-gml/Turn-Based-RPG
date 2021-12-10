using Godot;

public class HealItemStats : ItemStats
{
	[Export] public int HealAmount = 1;
	
	public override void Use()
	{
		playerStats.Heal(HealAmount);
	}
}
