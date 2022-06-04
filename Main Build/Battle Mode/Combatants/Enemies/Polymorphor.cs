using Godot;
using System;

public class Polymorphor : EnemyCombatant
{
    public override void _Ready()
    {
        sprite = (AnimatedSprite) GetNode("AnimatedSprite");
        //TODO Add this functionality to Combatant...Somehow...
        sprite.Connect("animation_finished", this, nameof(HandleAnimationTransition));
    }
}
