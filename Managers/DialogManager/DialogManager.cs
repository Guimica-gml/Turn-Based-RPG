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
	
	private Dictionary<string, string> _variableDefinitions = new Dictionary<string, string>();
	
	public object CallDialogFunction(string funcName, Array<string> args)
	{
		if (!HasMethod(funcName))
		{
			GD.PrintErr($"The Dialog function `{funcName}` doesn't exists");
			return null;
		}
		
		return Call(funcName, args);
	}
	
	private object AddItem(Array<string> args)
	{
		var inventory = GD.Load<Inventory>(args[0]);
		var item = GD.Load<ItemStats>(args[1]);
		inventory.AddItem(item);
		
		return null;
	}
	
	private object HasItem(Array<string> args)
	{
		var inventory = GD.Load<Inventory>(args[0]);
		var item = GD.Load<ItemStats>(args[1]);
		
		return (inventory.HasKeyItem(item.Name) || inventory.HasItem(item.Name));
	}
	
	private object SetVariableDefinition(Array<string> args)
	{
		var variableKey = args[0];
		var variableValue = args[1];
		_variableDefinitions[variableKey] = variableValue;
		
		return null;
	}
	
	private object CheckVariableDefinition(Array<string> args)
	{
		return (_variableDefinitions.ContainsKey(args[0]) &&  _variableDefinitions[args[0]] == args[1]);
	}
}
