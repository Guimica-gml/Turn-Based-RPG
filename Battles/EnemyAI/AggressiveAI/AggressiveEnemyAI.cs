using Godot;

/// <summary>
/// This AI tends to use more offensive actions than heal actions.<br/>
/// It always chooses the strongest action if it's going to attack.
/// </summary>
public class AggressiveEnemyAI : EnemyAI
{
	public override Action GetAction(Stats context)
	{
		var attackAction = GetStrongestAction(context.Actions);
		var healActions = GetHealActions(context.Actions);

		if (context.Hp < context.MaxHp && GD.RandRange(0f, 100f) < 30f)
		{
			return GetRandomAction(healActions);
		}

		return attackAction;
	}
}
