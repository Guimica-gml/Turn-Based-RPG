using Godot;
using Godot.Collections;

public class Talker : Resource
{
	[Export] public string Name = "";
	[Export] public Color NameColor = Colors.Black;
	[Export] public Dictionary<string, Texture> Portraits = new Dictionary<string, Texture>();
}
