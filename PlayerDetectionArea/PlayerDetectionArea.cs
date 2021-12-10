using Godot;

public class PlayerDetectionArea : Area2D
{
	[Signal] private delegate void PlayerEnteredArea(Player player);
	[Signal] private delegate void PlayerExitedArea(Player player);
	
	private void OnPlayerDetectionAreaBodyEntered(Entity entity)
	{
		if (!(entity is Player player)) return;
		EmitSignal(nameof(PlayerEnteredArea), player);
	}
	
	private void OnPlayerDetectionAreaBodyExited(Entity entity)
	{
		if (!(entity is Player player)) return;
		EmitSignal(nameof(PlayerExitedArea), player);
	}
}
