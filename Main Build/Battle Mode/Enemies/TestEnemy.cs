using System;
using Godot;

public partial class TestEnemy : EnemyCombatant
{
    [Export]
    public Ability testAttack;

    public override void _Ready(){
        base._Ready();
		testAttack.Setup(this);
    }

    public override CombatEventData DecideAction(Battle parentBattle){
        return testAttack.GetEventData();
    }   
}