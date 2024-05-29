using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Reflection.Metadata.Ecma335;

public partial class Player : CharacterBody2D
{
	[Export]
	public float HEALTH = 100.0f;
	[Export]
	public float MAXSPEED = 200.0f;
	[Export]
	public float ACCELERATION = 500.0f;
	[Export]
	public float ROLL_SPEED = 250.0f;
	[Export]
	public float FRICTION = 600.0f;

	public enum STATE{
		MOVE,
		ROLL,
		ATTACK,
		DEATH
	}

	public STATE state = STATE.MOVE;
	public Vector2 velocity = Vector2.Zero;
	public Vector2 roll_vector = Vector2.Down;
	public RayCast2D rayCast2D;
	public AnimatedSprite2D aniSprite;
	public int rayCastLen = 35;
	private float attackCooldown = 0.5f;
	private float timeSinceLastAttack = 0.0f;
	public Sprite2D playerSword;
	private AnimationTree animSwordTree;
	public AnimatedSprite2D swordSwipe;
	public Vector2 direction;
	public bool invincible;
	private UI health_bar;

	public override void _Ready()
	{
		base._Ready();
		rayCast2D = GetNode<RayCast2D>("RayCast2D");
		aniSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		playerSword = GetNode<Sprite2D>("Sword2D");
		animSwordTree = GetNode<AnimationTree>("Sword2D/AnimationTree");
		swordSwipe = GetNode<AnimatedSprite2D>("SwordSwipe");
		health_bar = GetNode<UI>("HealthBar");

		// Set player health value to the UI
		health_bar.Set_health_value(HEALTH);
		// TODO (set to false when player is ready to die) enables I frames when rolling to allow for player to be invincible
		invincible = false;
	}


	public override void _Process(double delta)
	{
		base._Process(delta);
		// EVERY WHOLE INT IS UPDATED PER SECOND I.E 1, 2, 3 IN DELTA IS SECONDS
		timeSinceLastAttack += (float)delta;
	}


	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		switch(state){
			case STATE.MOVE:
				move_state(delta);
				break;
			case STATE.ROLL:
				roll_state();	
				break;
			case STATE.ATTACK:
				attack_state();
				break;
			case STATE.DEATH:
				break;
		}
	}


	public void move_state(double delta){
		direction = Input.GetVector("walk_left","walk_right", "walk_up", "walk_down");
		direction = direction.Normalized();

		if(direction != Vector2.Zero){
			roll_vector = direction;
			velocity = velocity.MoveToward(direction * MAXSPEED, ACCELERATION * (float)delta);

			// running animations
			if(direction.X > 0){
				playerSword.Transform = new Transform2D(0, new Vector2(-7, -11));
				playerSword.FlipH = false;
				aniSprite.FlipH = false;
			}
			else if(direction.X < 0){
				playerSword.Transform = new Transform2D(0, new Vector2(7, -11));
				playerSword.FlipH = true;
				aniSprite.FlipH = true;
			}
			aniSprite.Play("player_walk_right");

		}
		else{
			velocity = velocity.MoveToward(Vector2.Zero, FRICTION * (float) delta);
			aniSprite.Play("player_idle");
		}

		// Change player raycast based on mouse direction
		rayCast2D.TargetPosition = (GetGlobalMousePosition() - GlobalPosition).Normalized() * rayCastLen;

		move();

		if(Input.IsActionJustPressed("roll")){
			// Call roll animation here so it does not recursively call itself
			aniSprite.Play("player_death");
			invincible = true;
			state = STATE.ROLL;
		}

		if(Input.IsActionJustPressed("attack") && timeSinceLastAttack >= attackCooldown){
			state = STATE.ATTACK;
			timeSinceLastAttack = 0.0f;
		}

	}


	public void roll_state(){
		velocity = roll_vector * ROLL_SPEED;
		move();

		// Rolling animation
		playerSword.Visible = false;
		if(!aniSprite.IsPlaying()){
			invincible = false;
			playerSword.Visible = true;
			state = STATE.MOVE;
		}
	}


	public void move(){
		Velocity = velocity;
		MoveAndSlide();
	}


	public void attack_state(){
		// Adjust attacking animations
		if(direction == Vector2.Zero){
			Vector2 mousePosition = GetGlobalMousePosition();
			Vector2 directionToMouse = (mousePosition - GlobalPosition).Normalized();

			if(directionToMouse.X > 0){
				playerSword.Transform = new Transform2D(0, new Vector2(-7, -11));
				playerSword.FlipH = false;
				aniSprite.FlipH = false;
			}
			else if(directionToMouse.X < 0){
				playerSword.Transform = new Transform2D(0, new Vector2(7, -11));
				playerSword.FlipH = true;
				aniSprite.FlipH = true;
			}
		}
		
		swordSwipe.Visible = true;
		swordSwipe.Play("Sword Swipe");

		var collider = rayCast2D.GetCollider();
		if(rayCast2D.IsColliding() && collider is CharacterBody2D characterBody2D){
			if(characterBody2D.HasMethod("kill")){
				characterBody2D.Call("kill");
			}
		}

		state= STATE.MOVE;
	}

	public void kill(){
		// Only subtract health if player is not invincible
		if(!invincible){
			HEALTH -= 1.0f;
			health_bar.Set_health_value(HEALTH);
			health_bar.Visible = true;
		}
		if(HEALTH <= 0){
			GetTree().ReloadCurrentScene();
		}
	}

	public void heal(){
		if(HEALTH < 100){
			HEALTH += 25;
			if(HEALTH > 100){
				HEALTH = 100;
			}
		}
		health_bar.Set_health_value(HEALTH);
	}
}
