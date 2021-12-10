using Godot;

public class TransitionManager : Node
{
	public enum Types { Default, Courtain, FromCenter }
	
	[Signal] private delegate void TransitionTriggered();
	[Signal] private delegate void SceneChanged();
	[Signal] private delegate void TransitionEnded();
	
	public bool InTransition = false;
	private SceneTransition _sceneTransition;
	
	private PackedScene DialogBoxPreload = GD.Load<PackedScene>("res://DialogBox/DialogBox.tscn");
	
	public override void _Ready()
	{
		PackedScene sceneTransitionPacked = GD.Load<PackedScene>("res://SceneTransition/SceneTransition.tscn");
		
		_sceneTransition = sceneTransitionPacked.Instance<SceneTransition>();
		AddChild(_sceneTransition);
		
		_sceneTransition.Connect("SceneChanged", this, "OnSceneChanged");
		_sceneTransition.Connect("TransitionTriggered", this, "OnTransitionTriggered");
		_sceneTransition.Connect("TransitionEnded", this, "OnTransitionEnded");
	}
	
	public void ChangeSceneTo(string scenePath, string sceneEntryIndetifier = "none", Types transitionType = Types.Default)
	{
		if (InTransition)
		{
			GD.PrintErr("Trying to start a transition while another transition is already happening");
			return;
		}
		
		_sceneTransition.ChangeSceneTo(scenePath, sceneEntryIndetifier, transitionType);
	}
	
	private void OnTransitionTriggered()
	{
		InTransition = true;
		EmitSignal(nameof(TransitionTriggered));
	}
	
	private void OnSceneChanged()
	{
		EmitSignal(nameof(SceneChanged));
	}
	
	private void OnTransitionEnded()
	{
		InTransition = false;
		EmitSignal(nameof(TransitionEnded));
	}
}
