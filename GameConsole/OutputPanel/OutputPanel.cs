using Godot;

public class OutputPanel : PanelContainer
{
	private VBoxContainer _outputsContainer;
	private ScrollContainer _scrollContainer;
	private VScrollBar _vScrollBar;

	private PackedScene _outputLinePreload = GD.Load<PackedScene>("res://GameConsole/OutputPanel/OutputLine.tscn");

	public override void _Ready()
	{
		_outputsContainer = GetNode<VBoxContainer>("ScrollContainer/OutputsContainer");
		_scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
		_vScrollBar = _scrollContainer.GetVScrollbar();
	}

	public async void AddOutputLine(string text)
	{
		var _outputLine = _outputLinePreload.Instance<RichTextLabel>();
		_outputLine.BbcodeText = text;
		_outputsContainer.AddChild(_outputLine);

		if (_outputsContainer.GetChildCount() > 20)
		{
			var child =_outputsContainer.GetChild(0);
			_outputsContainer.RemoveChild(child);
		}

		await ToSignal(GetTree(), "idle_frame");
		_scrollContainer.ScrollVertical = (int) _vScrollBar.MaxValue;
	}
}
