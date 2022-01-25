using Godot;

public class BattlePerformAction : BattleUpdateText
{
	public BattleCharacterDisplayer UserCharacterDisplayer;
	public BattleCharacterDisplayer TargetCharacterDisplayer;
	
	private bool _animEnded = false;
	
	public BattlePerformAction(BattleDisplayer battleDisplayer, BattleCharacterDisplayer userCharacterDisplayer, BattleCharacterDisplayer targetCharacterDisplayer, string nextScheme = "") :
	base(battleDisplayer, nextScheme:nextScheme)
	{
		UserCharacterDisplayer = userCharacterDisplayer;
		TargetCharacterDisplayer = targetCharacterDisplayer;
	}
	
	public override void OnReady()
	{
		if (BattleDisplayer.CurrentAction == null)
		{
			EmitSignal(nameof(Finished), NextScheme);
			return;
		}
		
		Text = $"{UserCharacterDisplayer.Stats.Name} used action {BattleDisplayer.CurrentAction.Name}.";
		base.OnReady();
		
		if (!BattleDisplayer.CurrentAction.Heal)
		{
			TargetCharacterDisplayer.ApplyAction(BattleDisplayer.CurrentAction, UserCharacterDisplayer.Stats);
			TargetCharacterDisplayer.Connect("AnimEnded", this, nameof(OnAnimEnded));
		}
		else
		{
			UserCharacterDisplayer.ApplyAction(BattleDisplayer.CurrentAction);
			UserCharacterDisplayer.Connect("AnimEnded", this, nameof(OnAnimEnded));
		}
	}
	
	public override void OnProcess(float delta)
	{
		if (_animEnded) base.OnProcess(delta);
	}
	
	public override void OnFinish()
	{
		if (TargetCharacterDisplayer.IsConnected("AnimEnded", this, nameof(OnAnimEnded)))
			TargetCharacterDisplayer.Disconnect("AnimEnded", this, nameof(OnAnimEnded));
		
		if (UserCharacterDisplayer.IsConnected("AnimEnded", this, nameof(OnAnimEnded)))
			UserCharacterDisplayer.Disconnect("AnimEnded", this, nameof(OnAnimEnded));
		
		_animEnded = false;
		BattleDisplayer.CurrentAction = null;
	}
	
	public override bool CanShowNextStateArrow()
	{
		return BattleDisplayer.CanChangeText() && _animEnded;
	}
	
	private void OnAnimEnded()
	{
		_animEnded = true;
	}
}
