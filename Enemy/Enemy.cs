using Godot;

public class Enemy : Npc
{
	[Export] public Stats Stats;
	[Export(PropertyHint.Range, "1, 100")] private int _customLevel = 1;

	public override void _Ready()
	{
		base._Ready();

		Stats = Stats.Duplicate() as Stats;
		Stats.Level = _customLevel;
	}

	private void OnPlayerDetectionAreaPlayerEnteredArea(Player player)
	{
		Global.BattleManager.StartBattle(this);
	}
}
