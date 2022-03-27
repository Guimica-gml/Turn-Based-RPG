using Godot;
using Godot.Collections;

public class DialogManager : CanvasLayer
{
	[Signal] private delegate void DialogTriggered(string dialogPath);
	[Signal] private delegate void DialogEnded(string dialogPath);

	public DialogBox DialogBox { get; private set; } = null;
	public string DialogPath { get; private set; } = "";

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

	// --- DIALOG DEFINITIONS
	// These methods can be used from the DialogBox

	private Dictionary<string, string> _globalDialogDefinitions = new Dictionary<string, string>();
	private Dictionary<string, Dictionary<string, string>> _localDialogDefinitions = new Dictionary<string, Dictionary<string, string>>();

	public object CallDialogFunction(string funcName, Array<string> args)
	{
		if (!HasMethod(funcName))
		{
			GD.PrintErr($"The Dialog function `{funcName}` doesn't exists");
			return null;
		}

		return Call(funcName, args);
	}

	public void LoadLocalDefinitions(string filePath, Dictionary<string, string> definitions)
	{
		if (_localDialogDefinitions.ContainsKey(filePath)) return;
		_localDialogDefinitions[filePath] = definitions;
	}

	// The following functions are used inside the dialgos, all should accept and Array of string as parameter
	// and return an object, return null if returning something is not necessary

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

	private object SetGlobalDefinition(Array<string> args)
	{
		var variableKey = args[0];
		var variableValue = args[1];
		_globalDialogDefinitions[variableKey] = variableValue;

		return null;
	}

	private object CheckGlobalDefinition(Array<string> args)
	{
		return (_globalDialogDefinitions.ContainsKey(args[0]) && _globalDialogDefinitions[args[0]] == args[1]);
	}

	private object SetLocalDefinition(Array<string> args)
	{
		var variableFile = args[0];
		var variableKey = args[1];
		var variableValue = args[2];

		_localDialogDefinitions[variableFile][variableKey] = variableValue;
		return null;
	}

	private object CheckLocalDefinition(Array<string> args)
	{
		var variableFile = args[0];
		var variableKey = args[1];
		var variableValue = args[2];

		if (!_localDialogDefinitions.ContainsKey(variableFile) || !_localDialogDefinitions[variableFile].ContainsKey(variableKey))
		{
			GD.PrintErr($"No local definition on file {variableFile} or key {variableKey}");
			return false;
		}

		return (_localDialogDefinitions[variableFile][variableKey] == variableValue);
	}
}
