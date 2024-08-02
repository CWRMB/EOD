using Godot;
using System;

public partial class WizardPlayer: PlayerManager {
    public override void _Ready()
    {
        base._Ready();
        switch_state = new WizardSwitchState(this, MAXSPEED, ACCELERATION, ROLL_SPEED, FRICTION, ROLL_COOLDOWN, ATTACK_COOLDOWN);
    }

    public override void _PhysicsProcess(double delta){
        base._PhysicsProcess(delta);
    }

    private partial class WizardSwitchState: ActionSwitchState {
        public WizardSwitchState(WizardPlayer player, float MAXSPEED, float ACCELERATION, float ROLL_SPEED, 
        float FRICTION, float ROLL_COOLDOWN, float ATTACK_COOLDOWN) : base(player, MAXSPEED, ACCELERATION, ROLL_SPEED, FRICTION, ROLL_COOLDOWN, ATTACK_COOLDOWN){
        }
    }
}