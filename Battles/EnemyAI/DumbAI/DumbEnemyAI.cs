
/// <summary>
/// This AI just gets a random action.
/// </summary>
public class DumbEnemyAI : EnemyAI
{
	public override Action GetAction(Stats context)
	{
		return GetRandomAction(GetUsableActions(context.Actions));
	}
}
