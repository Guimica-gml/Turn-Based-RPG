using Godot;

public class IncreaseStatsItemStats : ItemStats
{
	[Export] public string Stat = "";
	[Export] public int IncreaseAmount = 1;
	
	public override void Use()
	{
		playerStats.Set(Stat, ((int) playerStats.Get(Stat)) + IncreaseAmount);
	}
}
