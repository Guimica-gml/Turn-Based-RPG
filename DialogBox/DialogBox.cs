using Godot;
using Godot.Collections;

public class DialogBox : Control
{
	[Signal] private delegate void Ended();
	
	[Export(PropertyHint.File, "*.json")] public string DialogPath = "";
	[Export] private float _textSpeed = 0.025f;
	[Export] public Array<string> Replaces = new Array<string>();
	
	private Array<char> _slowerCharacters = new Array<char>() { ',', '.', '!', '?' };
	
	private bool _selectingResponseState = false;
	private string _part = "default";
	private int _page = 0;
	private Dictionary<string, Array<Dictionary>> _dialog = new Dictionary<string, Array<Dictionary>>();
	private int _selectedResponseIndex = 0;
	
	private Label _nameLabel;
	private RichTextLabel _textLabel;
	private VBoxContainer _resposnsesContainer;
	private TextureRect _nextArrowRect;
	private Timer _timer;
	
	private PackedScene _dialogResponsePreload = GD.Load<PackedScene>("res://DialogBox/DialogResponse/DialogResponse.tscn");
	
	public override void _Ready()
	{
		_nameLabel = GetNode<Label>("TextureRect/NameRect/NameLabel");
		_textLabel = GetNode<RichTextLabel>("TextureRect/MarginContainer/TextLabel");
		_resposnsesContainer = GetNode<VBoxContainer>("TextureRect/ResponsesContainer");
		_nextArrowRect = GetNode<TextureRect>("TextureRect/NextArrowRect");
		_timer = GetNode<Timer>("Timer");
		
		_dialog = LoadDialog();
		UpdateDialog();
	}
	
	public override void _Input(InputEvent @event)
	{
		if (_selectingResponseState)
		{
			MultipleResponsesInteraction(@event);
		}
		else
		{
			NormalInteraction(@event);
		}
		
		@event.Dispose();
	}
	
	private void MultipleResponsesInteraction(InputEvent @event)
	{
		// Changes the selected response depending on player input
		_selectedResponseIndex += System.Convert.ToInt16(@event.IsActionPressed("ui_down")) - System.Convert.ToInt16(@event.IsActionPressed("ui_up"));
		_selectedResponseIndex = Mathf.Clamp(_selectedResponseIndex, 0, (_dialog[_part][_page]["responses"] as Array).Count - 1);
		
		UpdateReponseButtons();
		
		// Select the response
		if (@event.IsActionPressed("ui_interact"))
			OnResponseButtonPressed(_selectedResponseIndex);
	}
	
	private void NormalInteraction(InputEvent @event)
	{
		if (!@event.IsActionPressed("ui_interact")) return;
		
		// Checking if all character are being displayed
		var charactersAmount = _textLabel.GetTotalCharacterCount();
		if (_textLabel.VisibleCharacters < charactersAmount)
		{
			_nextArrowRect.Visible = true;
			_textLabel.VisibleCharacters = charactersAmount;
			return;
		}
		
		// Checking if the player can select multiple responses
		if (_dialog[_part][_page].Contains("responses"))
		{
			CreateResponses(new Array<Dictionary>(_dialog[_part][_page]["responses"] as Array));
			_selectingResponseState = true;
			return;
		}
		
		_page += 1;
		UpdateDialog();
	}
	
	private void CreateResponses(Array<Dictionary> responses)
	{
		foreach (Dictionary response in responses)
		{
			var dialogResponse = _dialogResponsePreload.Instance<Button>();
			dialogResponse.Text = response["text"] as string;
			_resposnsesContainer.AddChild(dialogResponse);
		}
		
		UpdateReponseButtons();
	}
	
	private void DeleteResponses()
	{
		foreach (Button response in _resposnsesContainer.GetChildren())
		{
			response.QueueFree();
		}
	}
	
	private void UpdateDialog()
	{
		if (_page >= _dialog[_part].Count)
		{
			QueueFree();
			return;
		}
		
		var info = _dialog[_part][_page] as Dictionary;
		
		// Checking if it's a condition
		if (info.Contains("conditions"))
		{
			var conditions = new Array<Dictionary>(info["conditions"] as Array);
			var conditionsAreTrue = CheckConditions(conditions);
			
			ChangePart((conditionsAreTrue) ? info["partIfTrue"] as string : info["partIfFalse"] as string);
			return;
		}
		
		// Checking if we need to execute a script
		if (info.Contains("executeScripts"))
		{
			var scriptsInfo = new Array<Dictionary>(info["executeScripts"] as Array);
			
			foreach (var scriptInfo in scriptsInfo)
				ExecuteDialogFunction(scriptInfo);
		}
		
		var talker = LoadTalker(info["talker"] as string);
		
		// Setting dialog information
		_nameLabel.Text = talker.Name;
		_nameLabel.Modulate = talker.NameColor;
		_textLabel.BbcodeText = info["text"] as string;
		
		// Starting the dialog's new page
		_textLabel.VisibleCharacters = 0;
		_nextArrowRect.Visible = false;
		_timer.Start(_textSpeed);
	}
	
	private bool CheckConditions(Array<Dictionary> conditions)
	{
		var conditionsAreTrue = true;
		
		foreach (var condition in conditions)
		{
			if (ExecuteDialogFunction(condition) is bool value)
			{
				if (value) continue;
				conditionsAreTrue = false;
				break;
			}
		}
		
		return conditionsAreTrue;
	}
	
	private object ExecuteDialogFunction(Dictionary funcInfo)
	{
		var scriptName = funcInfo["script"] as string;
		var scriptArgs = new Array<string>(funcInfo["args"] as Array);
		return Global.DialogManager.CallDialogFunction(scriptName, scriptArgs);
	}
	
	private Talker LoadTalker(string talkerFilename)
	{
		var talkerPath = $"res://DialogBox/Talker/Resources/{talkerFilename}.tres";
		
		var dir = new Directory();
		if (!dir.FileExists(talkerPath))
		{
			GD.PrintErr($"There's no such file as {talkerPath}");
		}
		
		return GD.Load<Talker>(talkerPath);
	}
	
	private void UpdateReponseButtons()
	{
		foreach (Button response in _resposnsesContainer.GetChildren())
		{
			response.Pressed = (_selectedResponseIndex == response.GetIndex());
		}
	}
	
	private Dictionary<string, Array<Dictionary>> LoadDialog()
	{
		var dict = new Dictionary<string, Array<Dictionary>>();
		File file = new File();
		
		if (!file.FileExists(DialogPath))
		{
			GD.PrintErr($"Trying to load a dialog from a path that does not exists: {DialogPath}");
			QueueFree();
			return dict;
		}
		
		file.Open(DialogPath, File.ModeFlags.Read);
		var text = ApplyReplaces(file.GetAsText(), Replaces);
		dict = new Dictionary<string, Array<Dictionary>>(JSON.Parse(text).Result as Dictionary);
		file.Close();
		return dict;
	}
	
	private string ApplyReplaces(string text, Array<string> replaces)
	{
		var index = 0;
		while (text.FindN("%s") != -1)
		{
			if (index >= replaces.Count)
			{
				GD.PrintErr("More `%s` than replaces in DialogBox.cs");
				return text;
			}
			
			var replaceIndex = text.FindN("%s");
			var replaceLength = replaces[index].Length;
			var restOfText = text.Substr(replaceIndex + 2, text.Length);
			
			text = text.Substr(0, replaceIndex) + replaces[index];
			text += restOfText;
			index++;
		}
		return text;
	}
	
	private void OnResponseButtonPressed(int index)
	{
		var dict = new Array<Dictionary>(_dialog[_part][_page]["responses"] as Array);
		var partName = dict[index]["part"] as string;
		
		DeleteResponses();
		_selectingResponseState = false;
		ChangePart(partName);
	}
	
	private void ChangePart(string partName)
	{
		// Checking if it's the end of the dialog
		if (partName == "")
		{
			QueueFree();
			return;
		}
		
		_selectedResponseIndex = 0;
		_part = partName;
		_page = 0;
		UpdateDialog();
	}
	
	private void OnDialogBoxTreeExiting()
	{
		EmitSignal(nameof(Ended));
	}
	
	private void OnTimerTimeout()
	{
		if (_textLabel.VisibleCharacters < _textLabel.GetTotalCharacterCount())
		{
			_textLabel.VisibleCharacters += 1;
			var lastChar = _textLabel.Text[_textLabel.VisibleCharacters - 1];
			_timer.Start((_slowerCharacters.Contains(lastChar)) ? _textSpeed * 10f : _textSpeed);
			return;
		}
		
		_nextArrowRect.Visible = true;
		_timer.Stop();
	}
}
