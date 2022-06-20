using Godot;
using System;

public class Polymorphor : EnemyCombatant
{

    public override void updateAnimationTree(){
        animTree.Set("parameters/conditions/inPainState", inPainState);
    }
}
