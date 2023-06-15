using Godot;
using System;
using System.Collections.Generic;
public partial class EnemyNameplate : Sprite3D
{
	//Used to specify which sprites have been set up for their textures to be set to current status effects the character has
	[Export]
	Godot.Collections.Array<NodePath> statusSpritePath;
	Godot.Collections.Array<Sprite3D> statusSprites = new Godot.Collections.Array<Sprite3D>();
	[Export]
	EnemyCombatant parent;
	[Export]
	Label3D nameLabel;
	[Export]
	Label3D maxHP;

	[Export]
	Label3D currentHP; 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach(NodePath path in statusSpritePath){
			statusSprites.Add(GetNode<Sprite3D>(path));
		}
		nameLabel.Text = parent.GetName();
	}
	public void UpdateHP(int cHP, int mHP){
		maxHP.Text = ""+mHP;
		currentHP.Text = ""+cHP;
	}

	public void SetComName(string name){
		nameLabel.Text = name;
	}

	public void SetNamePlateVisible(bool toggle){
		this.Visible = toggle;
	}

/*
	public void UpdateStatus(List<PMStatus> statuses){
		if(statuses.Count > statusSprites.Count){
			throw new NotImplementedException(); //TODO Custom Exception
			//If we run into a situation where we can't display all statuses, that's an issue
			//TODO: Maybe add a UI Thing?
		}
		//Hide all status sprites, we'll make the ones we need visible
		foreach(Sprite3D sprite in statusSprites){
			sprite.Visible = false;
		}
		//Get the texture from each status, and assign it to one of our pre-positioned sprites
		for(int i = 0; i < statuses.Count; i++){
			statusSprites[i].Visible = true;
			statusSprites[i].Texture = statuses[i].GetEnemyTexture();
		}
	}
*/
}
