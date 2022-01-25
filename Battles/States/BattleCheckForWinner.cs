using Godot;

public class BattleCheckForWinner : BattleState
{
	public string SchemeWon = "";
	public string SchemeLost = "";
	
	public BattleCheckForWinner(BattleDisplayer battleDisplayer, string schemeWon, string schemeLost, string nextScheme = "") : base(battleDisplayer, nextScheme)
	{
		SchemeWon = schemeWon;
		SchemeLost = schemeLost;
	}
	
	public override void OnReady()
	{
		if (BattleDisplayer.PlayerStats.Hp <= 0)
		{
			NextScheme = SchemeLost;
		}
		else if (BattleDisplayer.EnemyStats.Hp <= 0)
		{
			NextScheme = SchemeLost;
		}
		
		EmitSignal(nameof(Finished), NextScheme);
	}
	
	public override void OnProcess(float delta)
	{
		return;
	}
	
	public override void OnFinish()
	{
		return;
	}
	
	public override bool CanShowNextStateArrow()
	{
		return true;
	}
}
