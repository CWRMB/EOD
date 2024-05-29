using Godot;
using System;

public partial class health_potion : Area2D
{
    public override void _Ready()
    {
        Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
    }

	private void OnBodyEntered(Node body) {
		if (body is Player player) {
			if (player.HEALTH < 100.0) {
			player.heal();
			QueueFree();
			}
		}
	}
}
