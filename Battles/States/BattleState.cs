using Godot;

public abstract class BattleState : Resource
{
	[Signal] protected delegate void Finished(string newScheme);
	
	public BattlePauseDisplayer BattlePauseDisplayer;
	public string NextScheme;
	
	public BattleState(BattlePauseDisplayer battlePauseDisplayer, string nextScheme = "")
	{
		BattlePauseDisplayer = battlePauseDisplayer;
		NextScheme = nextScheme;
	}
	
	public abstract void OnReady();
	public abstract void OnProcess(float delta);
	public abstract void OnFinish();
	public abstract bool CanShowNextStateArrow();
}
