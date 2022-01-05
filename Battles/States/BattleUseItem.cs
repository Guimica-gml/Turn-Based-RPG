using Godot;

public class BattleUseItem : BattleUpdateText
{
	public BattleUseItem(BattlePauseDisplayer battlePauseDisplayer, string nextScheme = "") : base(battlePauseDisplayer, nextScheme:nextScheme)
	{
		// Don't need anything here
	}
	
	public override void OnReady()
	{
		if (BattlePauseDisplayer.UsedItemName == "")
		{
			EmitSignal(nameof(Finished), NextScheme);
			return;
		}
		
		Text = $"You used item {BattlePauseDisplayer.UsedItemName}";
		base.OnReady();
	}
	
	public override void OnFinish()
	{
		BattlePauseDisplayer.UsedItemName = "";
	}
}
