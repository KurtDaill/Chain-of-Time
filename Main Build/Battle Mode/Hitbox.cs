using Godot;
using System;

public class Hitbox : Area2D
{
    int damage;
    public override void _Ready()
    {

    }

    public void SetDamage(int damage){
        this.damage = damage;
    }

    public int GetDamage(){
        return damage;
    }
}
