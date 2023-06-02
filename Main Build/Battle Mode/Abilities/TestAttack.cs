using System;
using Godot;

public partial class TestAttack : EnemyAbility
{
    public override void _Ready(){
        name = "TestAttack";
        animation = "Wiggle";
    }
    public override void Activate(int phase){
        GD.Print("EnemyAbilityHasActivated");
    }
}