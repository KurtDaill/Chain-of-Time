using System;
using Godot;

public partial class TestAttack : EnemyAbility
{
    public override void _Ready(){
        name = "TestAttack";
        animation = "Wiggle";
    }
    public override void AnimationTrigger(int phase){
        GD.Print("EnemyAbilityHasActivated");
        target[0].TakeDamage(1);
    }

    public override void Begin(){
		base.Begin();
		PlayCoreAnimation();
	}
}