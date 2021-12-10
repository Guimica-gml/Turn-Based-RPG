using Godot;
using Godot.Collections;

public class DialogManager : CanvasLayer
{
	[Signal] private delegate void DialogTriggered(string dialogPath);
	[Signal] private delegate void DialogEnded(string dialogPath);
	
	public DialogBox DialogBox = null;
	public string DialogPath = "";
	
	private PackedScene DialogBoxPreload = GD.Load<PackedScene>("res://DialogBox/DialogBox.tscn");
	
	public void StartDialog(string dialogPath, Array<string> replaces = null)
	{
		if (DialogBox != null)
		{
			GD.PrintErr("Trying to start a dialog while another dialog is already happening");
			return;
		}
		
		DialogPath = dialogPath;
		DialogBox = DialogBoxPreload.Instance<DialogBox>();
		DialogBox.DialogPath = DialogPath;
		if (replaces != null) DialogBox.Replaces = replaces;
		DialogBox.Connect("Ended", this, nameof(EndDialog));
		
		AddChild(DialogBox);
		EmitSignal(nameof(DialogTriggered), DialogPath);
	}
	
	private void EndDialog()
	{
		EmitSignal(nameof(DialogEnded), DialogPath);
		DialogBox.QueueFree();
		DialogBox = null;
		DialogPath = "";
	}
	
	/// --- DIALOG DEFINITIONS
	
	public void CallDialogFunction(string funcName, Array<string> args)
	{
		if (!HasMethod(funcName))
		{
			GD.PrintErr($"The Dialog function `{funcName}` doesn't exists");
			return;
		}
		
		Call(funcName, args);
	}
	
	private void AddItem(Array<string> args)
	{
		var inventory = GD.Load<Inventory>(args[0]);
		var item = GD.Load<ItemStats>(args[1]);
		inventory.AddItem(item);
	}
}
