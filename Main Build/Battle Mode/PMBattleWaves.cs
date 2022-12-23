using Godot;
using System;

public partial class PMBattleWaves : PMBattle
{
    [Export(PropertyHint.File)]
    public string nextBattle;
    private PackedScene nextWave;
    public override void _Ready()
    {
        base._Ready();
        //nextWave = ResourceLoader.Load<PackedScene>(nextBattle).Instance<PackedScene>();
    }
    public override void EndBattle(bool gameOver){
        if(gameOver){
            GD.Print("Game Over");
            _ExitTree();
        }
        else{
            //var old = GetTree().Root.GetChild(0);
            //master.SavePlayerParty(roster);
            GD.Print("Send in the Next Wave");
            master.NextWave(GD.Load<PackedScene>(nextBattle));
            //var root = GetTree().Root;
            //nextWave.LoadPlayerCharactersFromGM();
            //old.Free();
            //Figure out how to change scenes well!
        }
    }
}
