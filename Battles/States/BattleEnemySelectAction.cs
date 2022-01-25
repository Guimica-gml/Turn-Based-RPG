using Godot;
using Godot.Collections;

public class BattleEnemySelectAction : BattleState
{
	public Array<Action> EnemyActions;
	
	public BattleEnemySelectAction(BattleDisplayer battleDisplayer, Array<Action> enemyActions, string nextScheme = "") :
	base(battleDisplayer, nextScheme:nextScheme)
	{
		EnemyActions = enemyActions;
	}
	
	public override void OnReady()
	{
		GD.Randomize();
		
		BattleDisplayer.CurrentAction = GetAction();
		EmitSignal(nameof(Finished), NextScheme);
	}
	
	public override void OnProcess(float delta)
	{
		return;
	}
	
	public override void OnFinish()
	{
		return;
	}
	
	public override bool CanShowNextStateArrow()
	{
		return true;
	} 
	
	private Action GetAction()
	{
		var enemyStats = BattleDisplayer.EnemyDisplayer.Stats;
		var healActions = new Array<Action>();
		var attackActions = new Array<Action>();
		
		foreach (var action in EnemyActions)
		{
			if (action == null) continue;
			
			if (action.Heal)
			{
				healActions.Add(action);
			}
			else
			{
				attackActions.Add(action);
			}
		}
		
		if (enemyStats.Hp < enemyStats.MaxHp && GD.RandRange(0f, 100f) > 70f)
		{
			return healActions[(int) GD.RandRange(0, healActions.Count)];
		}
		
		return attackActions[(int) GD.RandRange(0, attackActions.Count)];
	}
}
