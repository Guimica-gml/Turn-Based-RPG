using Godot;

public class BattlePerformAction : BattleUpdateText
{
	public BattleCharacterDisplayer UserCharacterDisplayer;
	public BattleCharacterDisplayer TargetCharacterDisplayer;
	
	private bool _animEnded = false;
	
	public BattlePerformAction(BattlePauseDisplayer battlePauseDisplayer, BattleCharacterDisplayer userCharacterDisplayer, BattleCharacterDisplayer targetCharacterDisplayer, string nextScheme = "") :
	base(battlePauseDisplayer, nextScheme:nextScheme)
	{
		UserCharacterDisplayer = userCharacterDisplayer;
		TargetCharacterDisplayer = targetCharacterDisplayer;
	}
	
	public override void OnReady()
	{
		if (BattlePauseDisplayer.CurrentAction == null)
		{
			EmitSignal(nameof(Finished), NextScheme);
			return;
		}
		
		Text = $"{UserCharacterDisplayer.Stats.Name} used action {BattlePauseDisplayer.CurrentAction.Name}.";
		base.OnReady();
		
		if (!BattlePauseDisplayer.CurrentAction.Heal)
		{
			TargetCharacterDisplayer.ApplyAction(BattlePauseDisplayer.CurrentAction, UserCharacterDisplayer.Stats);
			TargetCharacterDisplayer.Connect("AnimEnded", this, nameof(OnAnimEnded));
		}
		else
		{
			UserCharacterDisplayer.ApplyAction(BattlePauseDisplayer.CurrentAction);
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
		BattlePauseDisplayer.CurrentAction = null;
	}
	
	public override bool CanShowNextStateArrow()
	{
		return BattlePauseDisplayer.CanChangeText() && _animEnded;
	}
	
	private void OnAnimEnded()
	{
		_animEnded = true;
	}
}
