using Godot;
using System;

public class Hitbox : Area
{
    int damage;

    Vector3 knockback = Vector3.Zero;
    public override void _Ready()
    {

    }

    public void SetDamage(int damage){
        this.damage = damage;
    }

    public int GetDamage(){
        return damage;
    }

    public void SetKnockback(Vector3 knockback){
        this.knockback = knockback;
    }

    public Vector3 GetKnockback(){
        return knockback;
    }
}
