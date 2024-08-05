using Godot;
using System;

public partial class WizardPlayer: PlayerManager {
    public override void _Ready()
    {
        base._Ready();
        switch_state = new WizardSwitchState(this, MAXSPEED, ACCELERATION, ROLL_SPEED, FRICTION, ROLL_COOLDOWN, ATTACK_COOLDOWN);

        // TODO During loading from save state / scene change this should be instantiated from a json saved state file or something else, etc.
        current_weapon = new WeaponManager(this, 2000);
    }

    public override void _PhysicsProcess(double delta){
        base._PhysicsProcess(delta);
    }

    private partial class WizardSwitchState: ActionSwitchState {
        public WizardSwitchState(WizardPlayer player, float MAXSPEED, float ACCELERATION, float ROLL_SPEED, 
        float FRICTION, float ROLL_COOLDOWN, float ATTACK_COOLDOWN) : base(player, MAXSPEED, ACCELERATION, ROLL_SPEED, FRICTION, ROLL_COOLDOWN, ATTACK_COOLDOWN){
        }

        protected override void AttackState()
        {
            base.AttackState();
            player.current_weapon?.Attack();
        }

    }
}