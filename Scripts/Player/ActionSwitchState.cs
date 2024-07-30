using Godot;
using System;
using System.ComponentModel;

public partial class ActionSwitchState : Node2D
{
	/**
		This classes purpose is for controling switch state actions of the player. Such as,
		 moving, attacking, rolling, etc. Then, playing the corresponding animation from the animation player.
	*/

	public enum STATE{
		MOVE,
		IDLE,
		ROLL,
		ATTACK,
		DEATH
	}
	public STATE current_state = STATE.IDLE;
	public Vector2 roll_vector = Vector2.Down;
	private float roll_duration = 0.5f;
	private float roll_timer = 0f;
	private bool is_rolling = false;
	private float ROLL_COOLDOWN;
	private float roll_cooldown_timer = 0f;
	private float ATTACK_COOLDOWN;
	private float attack_cooldown_timer = 0f;
	private Vector2 velocity = Vector2.Zero;
	private Vector2 direction = Vector2.Zero;
	public float MAXSPEED;
	public float ACCELERATION;
	public float ROLL_SPEED;
	public float FRICTION;
	private PlayerManager player;

	public ActionSwitchState(PlayerManager player, float MAXSPEED, float ACCELERATION, 
		float ROLL_SPEED, float FRICTION, float ROLL_COOLDOWN = 0.5f, float ATTACK_COOLDOWN = 0.3f){
		this.player = player;
		this.MAXSPEED = MAXSPEED;
		this.ACCELERATION = ACCELERATION;
		this.ROLL_SPEED = ROLL_SPEED;
		this.FRICTION = FRICTION;
		this.ROLL_COOLDOWN = ROLL_COOLDOWN;
		this.ATTACK_COOLDOWN = ATTACK_COOLDOWN;
	}

	public void UpdatePlayerDirection(){
		// Change player raycast based on mouse direction
		player.rayCast2D.TargetPosition = (player.GetGlobalMousePosition() - player.GlobalPosition).Normalized() * player.rayCastLen;
	}

	private STATE CheckInputs(){
		if(is_rolling){
			return STATE.ROLL;
		}
		if(Input.IsActionJustPressed("roll") && roll_cooldown_timer <= 0){
			return STATE.ROLL;
		}
		if(Input.IsActionJustPressed("attack") && attack_cooldown_timer <= 0){
			return STATE.ATTACK;
		}
		if(Input.IsActionPressed("walk_up") || Input.IsActionPressed("walk_down") || 
			Input.IsActionPressed("walk_left") || Input.IsActionPressed("walk_right")){
			return STATE.MOVE;
		}
		else{return STATE.IDLE;}
	}

	public void CheckState(double delta){
		roll_timer -= (float)delta;
		roll_cooldown_timer -= (float)delta;

		attack_cooldown_timer -= (float)delta;

		if(roll_timer <= 0 && is_rolling){
			is_rolling = false;
			player.invincible = false;
			roll_cooldown_timer = ROLL_COOLDOWN;
		}

		current_state = CheckInputs();

		switch(current_state){
			case STATE.MOVE:
				MoveState(delta);
				break;
			case STATE.ROLL:
				RollState();	
				break;
			case STATE.ATTACK:
				AttackState();
				break;
			case STATE.IDLE:
				IdleState(delta);
				break;
			case STATE.DEATH:
				DeathState();
				break;
		}
	}

	private void IdleState(double delta){
		player.aniSprite.Play("Idle");
		velocity = velocity.MoveToward(Vector2.Zero, FRICTION * (float) delta);
		player.Move(velocity);
	}

    private void MoveState(double delta)
    {
        direction = Input.GetVector("walk_left", "walk_right", "walk_up", "walk_down");

		direction = direction.Normalized();

		if(direction != Vector2.Zero){
			roll_vector = direction;
			velocity = velocity.MoveToward(direction * MAXSPEED, ACCELERATION * (float)delta);
			player.aniSprite.Play("Run");
		}

		player.Move(velocity);
	}

	private void RollState(){
		if(!is_rolling) {
			player.aniSprite.Play("Death");
			velocity = roll_vector * ROLL_SPEED;
			roll_timer = roll_duration;
			is_rolling = true;
			player.invincible = true;
		}
		player.Move(velocity);
	}

	private void AttackState(){
		// Set the cooldown which prevents spamming attacks
		attack_cooldown_timer = ATTACK_COOLDOWN;

		var collider = player.rayCast2D.GetCollider();
		if(player.rayCast2D.IsColliding() && collider is CharacterBody2D characterBody2D){
			if(characterBody2D.HasMethod("kill")){
				characterBody2D.Call("kill");
			}
		}
	}

	public void DeathState(){
		player.aniSprite.Play("Death");
	}

}
