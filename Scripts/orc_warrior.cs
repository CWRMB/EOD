using Godot;
using System;
using System.Reflection.Metadata;

public partial class orc_warrior : CharacterBody2D
{
	[Export]
	public float HEALTH = 100.0f;
	[Export]
	public float SPEED = 100.0f;
	[Export]
	public float ACCELERATION = 500.0f;
	[Export]
	public float FRICTION = 600.0f;
	private float attackCooldown = 1.0f;
	private float timeSinceLastAttack = 0.0f;
	AnimatedSprite2D aniSprite;

	public CharacterBody2D player;
	private RayCast2D rayCast2D;
	private int rayCastLen = 35;
	public Vector2 velocity = Vector2.Zero;
	public enum STATE{
		MOVE,
		ATTACK,
		DEATH
	}
	public STATE currState = STATE.MOVE;


    public override void _Ready()
    {
        base._Ready();
		rayCast2D = GetNode<RayCast2D>("RayCast2D");
		player = GetNode<CharacterBody2D>("/root/Main/TileMap/Player");
		aniSprite  = GetNode<AnimatedSprite2D>("Orc Warrior Sprites");
		AddToGroup("orc_warrior");
    }


    public override void _Process(double delta)
    {
        base._Process(delta);
		timeSinceLastAttack += (float)delta;
    }


	public override void _PhysicsProcess(double delta)
	{
		switch(currState){
			case STATE.MOVE:
				move_state(delta);
				break;
			case STATE.ATTACK:
				attack_state();
				break;
			case STATE.DEATH:

				break;
		}

	}


	public void move_state(double delta){
		if(player == null){
			GD.Print("PLAYER IS NULL (orc_warrior)");
			return;
		}

		Vector2 vecToPlayer = player.GlobalPosition - GlobalPosition;
		vecToPlayer = vecToPlayer.Normalized();
		
		// animation movements
		if(vecToPlayer != Vector2.Zero){
			
			if(vecToPlayer.X > 0){
				aniSprite.FlipH = false;
			}
			else{
				aniSprite.FlipH = true;
			}
			aniSprite.Play("orc_run");
		}
		else{
			aniSprite.Play("orc_idle");
		}

		velocity = velocity.MoveToward(vecToPlayer * SPEED, ACCELERATION * (float)delta);
		rayCast2D.TargetPosition = vecToPlayer * rayCastLen;

		move();

		if(timeSinceLastAttack >= attackCooldown){
			var collider = rayCast2D.GetCollider();
			if(rayCast2D.IsColliding() && collider.Equals(player)){
				currState = STATE.ATTACK;
			}
		}
	}

	public void attack_state(){
		if(player.HasMethod("Kill")){
			player.Call("Kill");
		}
		currState = STATE.MOVE;
	}


	public void move(){
		Velocity = velocity;
		MoveAndSlide();
	}


	// This is called from the player when the raycast hits this coll
	public void kill(){
		HEALTH -= 25.0f;

		// if(HEALTH <= 0.0f){
		currState = STATE.DEATH;
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
		aniSprite.Play("orc_death");
		aniSprite.Connect("animation_finished", Callable.From(on_death_animation_finished));
		// }
	}


	private void on_death_animation_finished(){
		// Ensure that the signal is disconnected to avoid multiple calls
		aniSprite.Disconnect("animation_finished", Callable.From(on_death_animation_finished));
		QueueFree();
	}

}
