using Godot;
using Godot.Collections;

public abstract class EnemyAI : Resource
{
	public abstract Action GetAction(Stats context);

	protected Action GetStrongestAction(Array<Action> actions)
	{
		var attackActions = GetAttackActions(actions);
		var strongest = attackActions[0];

		foreach (Action action in attackActions)
		{
			if (action.Value > strongest.Value) strongest = action;
		}

		return strongest;
	}

	protected Action GetWeakestAction(Array<Action> actions)
	{
		var attackActions = GetAttackActions(actions);
		var weakest = attackActions[0];

		foreach (Action action in attackActions)
		{
			if (action.Value < weakest.Value) weakest = action;
		}

		return weakest;
	}

	protected Array<Action> GetUsableActions(Array<Action> actions)
	{
		var healActions = new Array<Action>();

		foreach (var action in actions)
		{
			if (action == null) continue;
			healActions.Add(action);
		}

		return healActions;
	}

	protected Array<Action> GetHealActions(Array<Action> actions)
	{
		var healActions = new Array<Action>();

		foreach (var action in actions)
		{
			if (action == null || !action.Heal) continue;
			healActions.Add(action);
		}

		return healActions;
	}

	protected Array<Action> GetAttackActions(Array<Action> actions)
	{
		var attackActions = new Array<Action>();

		foreach (var action in actions)
		{
			if (action == null || action.Heal) continue;
			attackActions.Add(action);
		}

		return attackActions;
	}

	protected Action GetRandomAction(Array<Action> actions)
	{
		return actions[(int) GD.RandRange(0, actions.Count)];
	}
}
