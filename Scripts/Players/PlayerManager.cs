using Godot;
using System;

public partial class PlayerManager : CharacterBody2D
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
    [Export]
    public int rayCastLen = 50;
    [Export]
    public float ROLL_COOLDOWN = 0.5f;
    [Export]
    public float ATTACK_COOLDOWN = 0.3f;

    public bool invincible = false;
    public RayCast2D rayCast2D;
    public AnimatedSprite2D aniSprite;
    public CollisionShape2D hit_box;
    protected ActionSwitchState switch_state;
    protected bool death = false;

    public override void _Ready(){
        base._Ready();
        rayCast2D = GetNode<RayCast2D>("RayCast2D");

		aniSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        aniSprite.Connect("animation_finished", Callable.From(OnAnimationFinished));

        hit_box = GetNode<CollisionShape2D>("HitBox");
        hit_box.Disabled = false;

        switch_state = new(this, MAXSPEED, ACCELERATION, ROLL_SPEED, FRICTION, ROLL_COOLDOWN, ATTACK_COOLDOWN);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._Process(delta);
        if(!death){
            switch_state.UpdatePlayerDirection();
            switch_state.CheckState(delta);
        }
    }

    public void Move(Vector2 velocity){
		Velocity = velocity;
		MoveAndSlide();
	}

    public void Kill(){
        /*
        * Called from another entity that searches for the kill method on attack. This then
        * subtracts health from the player until the health is 0 or less then take away control,
        * play the death animation and reload the scene.
        */
        if(!invincible){HEALTH -= 15.0f;}

        if(HEALTH <= 0.0f){
            hit_box.Disabled = true;
            death = true;
            switch_state.DeathState();
        }
	}

    public void OnAnimationFinished(){
        GetTree().ReloadCurrentScene();
    }

}
