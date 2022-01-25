using Godot;

public class BattleUseItem : BattleUpdateText
{
	public BattleUseItem(BattleDisplayer battleDisplayer, string nextScheme = "") : base(battleDisplayer, nextScheme:nextScheme)
	{
		// Don't need anything here
	}
	
	public override void OnReady()
	{
		if (BattleDisplayer.UsedItemName == "")
		{
			EmitSignal(nameof(Finished), NextScheme);
			return;
		}
		
		Text = $"You used item {BattleDisplayer.UsedItemName}";
		base.OnReady();
	}
	
	public override void OnFinish()
	{
		BattleDisplayer.UsedItemName = "";
	}
}
