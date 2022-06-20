using Godot;
using System;

public class EnemyCombatant : Combatant
{
    [Export]
    public NodePath AnimationTree;
    protected EnemyAttack[] attacksKnown;

    public override void _Ready()
    {
        //hitbox = (Area2D) GetNode("Hitbox");
        //if(hitbox == null){
            //throw new NotImplementedException();
        //}
        animTree = (AnimationTree) GetNode(AnimationTree);
        animSM = (AnimationNodeStateMachinePlayback) animTree.Get("parameters/playback");
        animPlayer = (AnimationPlayer) GetNode(animationPlayer);
    }
    public override void _Process(float delta)
    {
        updateAnimationTree();
    }

    //Ran every frame while a player is attacking!
    //Returns true if this enemy is in a pain state
    public virtual bool DodgeBehaviour(){
        var newState = state.Process(this);
        if(newState != null){
            SetState(newState);
        }
        return state is CombatantStatePain;
    }

    public override int TakeDamage(int incomingDamage, Vector3 knockback){
        int dmg = base.TakeDamage(incomingDamage, knockback);
        GD.Print(Name + " hit! : " + dmg + " Damage Dealt!");
        inPainState = true;
        return dmg;
    }
}
