using Godot;
using System;

public partial class Weapon : Node2D
{
    protected PlayerManager player;
    protected int weapon_id;

    public Weapon(PlayerManager player, int weapon_id) {
        this.player = player;
        this.weapon_id = weapon_id;
    }
}
