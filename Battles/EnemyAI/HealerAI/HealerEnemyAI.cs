using Godot;

/// <summary>
/// This AI tends to use more heal actions than offensive actions.<br/>
/// It chooses a random action if it's going to attack.
/// </summary>
public class HealerEnemyAI : EnemyAI
{
	public override Action GetAction(Stats context)
	{
		var attackActions = GetAttackActions(context.Actions);
		var healActions = GetHealActions(context.Actions);

		if (context.Hp < context.MaxHp && GD.RandRange(0f, 100f) < 60f)
		{
			return GetRandomAction(healActions);
		}

		return GetRandomAction(attackActions);
	}
}
