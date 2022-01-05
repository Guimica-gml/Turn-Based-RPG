using Godot;

public class BattleCheckForWinner : BattleState
{
	public string SchemeWon = "";
	public string SchemeLost = "";
	
	public BattleCheckForWinner(BattlePauseDisplayer battlePauseDisplayer, string schemeWon, string schemeLost, string nextScheme = "") : base(battlePauseDisplayer, nextScheme)
	{
		SchemeWon = schemeWon;
		SchemeLost = schemeLost;
	}
	
	public override void OnReady()
	{
		if (BattlePauseDisplayer.PlayerStats.Hp <= 0)
		{
			NextScheme = SchemeLost;
		}
		else if (BattlePauseDisplayer.EnemyStats.Hp <= 0)
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
