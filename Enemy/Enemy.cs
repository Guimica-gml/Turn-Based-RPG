using Godot;

public class Enemy : Npc
{
	[Export] public Stats Stats;
	
	private void OnPlayerDetectionAreaPlayerEnteredArea(Player player)
	{
		Global.BattleManager.StartBattle(this);
	}
}
