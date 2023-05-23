using Godot;
using System;
using static BattleUtilities;

public partial class Combatant : Node
{
	protected int HP;
	protected AnimationPlayer animPlay;

	protected BattlePosition currentPosition;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public virtual void TakeDamage(int damage)
	{
		animPlay.Play("HitReact");
		this.HP -= damage;
		//Figure how how we're displaying damage numbers
		//Figure out how we're dealing with death logic
		//TODO Figure out how we want to pause the progression of the battle state while we wait for animations to complete, then put it here

	}

	public bool HasAnimation(string name){
		return animPlay.HasAnimation(name);
	}

	public BattlePosition GetPosition(){
		return currentPosition;
	}
}
