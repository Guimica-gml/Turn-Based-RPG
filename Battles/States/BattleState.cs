using Godot;

public abstract class BattleState : Resource
{
	[Signal] protected delegate void Finished(string newScheme);
	
	public BattleDisplayer BattleDisplayer;
	public string NextScheme;
	
	public BattleState(BattleDisplayer battleDisplayer, string nextScheme = "")
	{
		BattleDisplayer = battleDisplayer;
		NextScheme = nextScheme;
	}
	
	public abstract void OnReady();
	public abstract void OnProcess(float delta);
	public abstract void OnFinish();
	public abstract bool CanShowNextStateArrow();
}
