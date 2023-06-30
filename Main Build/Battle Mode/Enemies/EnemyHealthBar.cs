using Godot;
using System;

public partial class EnemyHealthBar : Sprite3D
{
	private Sprite3D juice;
	
	[Export]
	//How many frames of animation does the juice bar have?
	private int juiceFrames;

	public override void _Ready(){
		juice = this.GetNode<Sprite3D>("Juice");
		//We control the ammount of health displayed by the "juice" (red part) of the health part being rendered as an animation
		//The first frame is full, the last frame is empty.
		juice.Frame = 0;
	}

	public void SetDisplay(int hp, int maxHP){
		if(hp <= 0){
			juice.Frame = juiceFrames;
			return;
		}
		float factor = ((float)hp/(float)maxHP);
		float adjustedFactor = 1 - factor;
		float test = juiceFrames * adjustedFactor;
		juice.Frame = Mathf.RoundToInt(test);
	}
}
