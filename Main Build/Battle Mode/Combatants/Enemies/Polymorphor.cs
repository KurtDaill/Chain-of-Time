using Godot;
using System;

public class Polymorphor : EnemyCombatant
{
    public override void _Ready()
    {
        sprite = (AnimatedSprite) GetNode("AnimatedSprite");
        sprite.Connect("animation_finished", this, nameof(HandleAnimationTransition));
    }
}
