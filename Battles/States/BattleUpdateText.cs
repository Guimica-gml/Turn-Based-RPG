using Godot;

public class BattleUpdateText : BattleState
{
	public string Text = "";
	public bool StartInvisible = false;
	
	public BattleUpdateText(BattlePauseDisplayer battlePauseDisplayer, string text = "", bool startInvisible = true, string nextScheme = "") :
	base(battlePauseDisplayer, nextScheme)
	{
		Text = text;
		StartInvisible = startInvisible;
	}
	
	public override void OnReady()
	{
		BattlePauseDisplayer.SetBattleText(Text, StartInvisible);
	}
	
	public override void OnProcess(float delta)
	{
		if (BattlePauseDisplayer.CanChangeText() && BattlePauseDisplayer.MouseHoverTextBox && Input.IsActionJustPressed("left_click"))
		{
			EmitSignal(nameof(Finished), NextScheme);
		}
	}
	
	public override void OnFinish()
	{
		return;
	}
	
	public override bool CanShowNextStateArrow()
	{
		return BattlePauseDisplayer.CanChangeText();
	}
}
