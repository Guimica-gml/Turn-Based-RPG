using Godot;

public class BattlePlayerLevelUp : BattleUpdateText
{
	public BattlePlayerLevelUp(BattleDisplayer battleDisplayer, string nextScheme = "") : base(battleDisplayer, nextScheme:nextScheme)
	{
		// Don't need anything here
	}
	
	public override void OnReady()
	{
		if (BattleDisplayer.LevelUpMessage == "")
		{
			EmitSignal(nameof(Finished), NextScheme);
			return;
		}
		
		Text = $"You Leveled Up! \n{BattleDisplayer.LevelUpMessage}";
		base.OnReady();
	}
	
	public override void OnFinish()
	{
		BattleDisplayer.LevelUpMessage = "";
	}
}
