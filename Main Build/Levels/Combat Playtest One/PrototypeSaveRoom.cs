using Godot;
using System;

public partial class PrototypeSaveRoom : Node3D
{
	bool savedYet = false;
	GameMaster GM;
	[Export(PropertyHint.File)]
	private string nextBattlePath;
	PackedScene nextBattle;

	public override void _Ready(){
		base._Ready();
		GM = this.GetNode<GameMaster>("/root/GameMaster");
		if(!savedYet){
			GM.RestorePartyHPAndSP();
			GM.BookmarkCurrentParty();
			savedYet = true;
		}else{
			GM.LoadBookmarkSave();
		}
		nextBattle = GD.Load<PackedScene>(nextBattlePath);
	}

	public override void _Process(double delta){
		if(Input.IsActionJustPressed("ui_start")){
			GetTree().ChangeSceneToPacked(nextBattle);
		}
	}
}
