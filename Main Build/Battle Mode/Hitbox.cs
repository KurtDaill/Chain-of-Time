using Godot;
using System;

public class Hitbox : Area2D
{
    int damage;

    Vector2 knockback = Vector2.Zero;
    public override void _Ready()
    {

    }

    public void SetDamage(int damage){
        this.damage = damage;
    }

    public int GetDamage(){
        return damage;
    }

    public void SetKnockback(Vector2 knockback){
        this.knockback = knockback;
    }

    public Vector2 GetKnockback(){
        return knockback;
    }
}
