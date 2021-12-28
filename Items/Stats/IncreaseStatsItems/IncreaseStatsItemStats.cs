using Godot;

public class IncreaseStatsItemStats : ItemStats
{
	[Export] public string Stat = "";
	[Export] public bool Temp = false;
	[Export] public int IncreaseAmount = 1;
	
	public override void Use()
	{
		playerStats.IncreaseStatBoost(Stat, IncreaseAmount, Temp);
	}
}
