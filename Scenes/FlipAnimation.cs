using Godot;
using System;

/*
	This class handles all the animations for the player using a switch state method
*/
public partial class FlipAnimation : AnimatedSprite2D
{
    private enum PlayerState
    {
        Idle,
        Walking,
        Rolling,
        Attack
    }

    private PlayerState currentState = PlayerState.Idle;
    public Sprite2D playerSword;
    private AnimationTree animSwordTree;
    public AnimatedSprite2D swordSwipe;
    public Player player;

    //public Vector2 mousePos;

    public override void _Ready()
    {
        base._Ready();
        
        player = GetNode<Player>("/root/Main/Player");
        playerSword = GetNode<Sprite2D>("/root/Main/Player/Sword2D");
        animSwordTree = GetNode<AnimationTree>("/root/Main/Player/Sword2D/AnimationTree");
        swordSwipe = GetNode<AnimatedSprite2D>("/root/Main/Player/SwordSwipe");
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        HandleInput();
        UpdateAnimation();
    }

    private void HandleInput()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                if(Input.IsActionJustPressed("attack")){
                    currentState = PlayerState.Attack;
                }

				if (Input.IsActionPressed("walk_right"))
                {
                    // Transform the player sword to align with the proper sprite
                    playerSword.Transform = new Transform2D(0, new Vector2(-7, -11));
                    playerSword.FlipH = false;
                    FlipH = false;
                    currentState = PlayerState.Walking;
                }
                else if (Input.IsActionPressed("walk_left"))
                {
                    playerSword.Transform = new Transform2D(0, new Vector2(7, -11));
                    playerSword.FlipH = true;
                    FlipH = true;
                    currentState = PlayerState.Walking;
                }

                if (Input.IsActionJustPressed("roll"))
                {
                    Play("player_death");
                    currentState = PlayerState.Rolling;
                }

                break;

            case PlayerState.Walking:
                if(Input.IsActionJustPressed("attack")){
                    currentState = PlayerState.Attack;
                }

                if (!Input.IsActionPressed("walk_right") && !Input.IsActionPressed("walk_left"))
                {
                    currentState = PlayerState.Idle;
                }

				else if (Input.IsActionPressed("walk_right"))
                {
                    // Transform the player sword to align with the proper sprite
                    playerSword.Transform = new Transform2D(0, new Vector2(-7, -11));
                    playerSword.FlipH = false;
                    FlipH = false;
                } 
                else if (Input.IsActionPressed("walk_left"))
                {
                    playerSword.Transform = new Transform2D(0, new Vector2(7, -11));
                    playerSword.FlipH = true;
                    FlipH = true;
                }

                if (Input.IsActionJustPressed("roll"))
                {
                    Play("player_death");
                    currentState = PlayerState.Rolling;
                }
                break;

            case PlayerState.Rolling:
                // Do nothing here, let the animation play out
                break;
            case PlayerState.Attack:
                if (!Input.IsActionPressed("walk_right") && !Input.IsActionPressed("walk_left")){
                    Vector2 mousePosition = GetGlobalMousePosition();
                    Vector2 directionToMouse = (mousePosition - GlobalPosition).Normalized();
                    if(directionToMouse.X > 0){
                        playerSword.Transform = new Transform2D(0, new Vector2(-7, -11));
                        playerSword.FlipH = false;
                        FlipH = false;
                    }
                    else{
                        playerSword.Transform = new Transform2D(0, new Vector2(7, -11));
                        playerSword.FlipH = true;
                        FlipH = true;
                    }
                    currentState = PlayerState.Idle;
                }
                else{
                    currentState = PlayerState.Walking;
                }
                break;
        }
    }

    private void UpdateAnimation()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                animSwordTree.Active = false;
                Play("player_idle");
                break;

            case PlayerState.Walking:
                animSwordTree.Active = true;
                Play("player_walk_right");
                break;

            case PlayerState.Rolling:
                playerSword.Visible = false;
                if (!IsPlaying())
                {
                    playerSword.Visible = true;
                    // Animation is complete, transition back to idle
                    currentState = PlayerState.Idle;
                }
                break;
            case PlayerState.Attack:
                swordSwipe.Visible = true;
                swordSwipe.Play("Sword Swipe");
                break;
        }
    }

}