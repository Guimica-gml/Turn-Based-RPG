using Godot;

public class BattleUpdateText : BattleState
{
	public string Text = "";
	public bool StartInvisible = false;
	
	public BattleUpdateText(BattleDisplayer battleDisplayer, string text = "", bool startInvisible = true, string nextScheme = "") :
	base(battleDisplayer, nextScheme)
	{
		Text = text;
		StartInvisible = startInvisible;
	}
	
	public override void OnReady()
	{
		BattleDisplayer.SetBattleText(Text, StartInvisible);
	}
	
	public override void OnProcess(float delta)
	{
		if (BattleDisplayer.CanChangeText() && BattleDisplayer.MouseHoverTextBox && Input.IsActionJustPressed("left_click"))
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
		return BattleDisplayer.CanChangeText();
	}
}
