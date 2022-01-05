using Godot;

public class BattlePlayerLevelUp : BattleUpdateText
{
	public BattlePlayerLevelUp(BattlePauseDisplayer battlePauseDisplayer, string nextScheme = "") : base(battlePauseDisplayer, nextScheme:nextScheme)
	{
		// Don't need anything here
	}
	
	public override void OnReady()
	{
		if (BattlePauseDisplayer.LevelUpMessage == "")
		{
			EmitSignal(nameof(Finished), NextScheme);
			return;
		}
		
		Text = $"You Leveled Up! \n{BattlePauseDisplayer.LevelUpMessage}";
		base.OnReady();
	}
	
	public override void OnFinish()
	{
		BattlePauseDisplayer.LevelUpMessage = "";
	}
}
