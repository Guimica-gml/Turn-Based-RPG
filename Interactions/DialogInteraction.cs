using Godot;
using Godot.Collections;

public class DialogInteraction : Interaction
{
	[Export(PropertyHint.File, "*.json")] public string DialogPath;
	[Export] public Array<string> Replaces = new Array<string>();

	public override void OnTrigger()
	{
		Global.DialogManager.Connect("DialogEnded", this, nameof(OnDialogEnded));
		Global.DialogManager.StartDialog(DialogPath, Replaces);
	}

	public override bool CanTrigger()
	{
		return true;
	}

	public override void OnEnd()
	{
		Global.DialogManager.Disconnect("DialogEnded", this, nameof(OnDialogEnded));
	}

	public override Texture GetIcon()
	{
		return GD.Load<Texture>("res://Interactions/Icons/SpeechBubbleIcon.png");
	}

	private void OnDialogEnded(string dialogPath)
	{
		if (dialogPath != DialogPath) return;
		EmitSignal(nameof(Ended));
	}
}
