using Godot;
using System;

public partial class GameMaster : Node
{
    PlayerCharacterData[] playerData = Array.Empty<PlayerCharacterData>();
    public struct PlayerCharacterData{
        public PlayerCharacterData(string filePath, int hp, int maxHP, int sp, int maxSP, uint position, int[] abilitiesKnown, int[] abilitiesPrepared){
            this.filePath = filePath;
            this.hp = hp;
            this.maxHP = maxHP;
            this.sp = sp;
            this.maxSP = maxSP;
            this.position = position;
            this.abilitiesKnown = abilitiesKnown;
            this.abilitiesPrepared = abilitiesPrepared;
        }

        public string filePath {get; set;}
        public int hp { get; set; }
        public int sp { get; set; }
        public int maxHP { get; set; }
        public int maxSP { get; set; }
        public uint position {get; set;}
        public int[] abilitiesKnown {get; set;}
        public int [] abilitiesPrepared {get; set;}
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void SavePlayerParty(PMBattleRoster roster){
        var players = roster.GetPlayerCharacters();
        playerData = new PlayerCharacterData[players.Length];
        for(int i = 0; i < players.Length; i++){
            playerData[i] = players[i].ExportData();
        }
    }

    public bool IsPartyDataOnFile(){
        return(playerData != Array.Empty<PlayerCharacterData>());
    }

    public PlayerCharacterData[] GetPlayerCharacterData(){
        return playerData;
    }

    public void NextWave(PackedScene newBattle){
        PMBattle old = GetNode<PMBattle>("/root/Battle");
        SavePlayerParty(old.roster);
        PMBattle battle = newBattle.Instantiate<PMBattle>();
        battle.GetChild<PMBattleRoster>(0).LoadPlayerCharacters(playerData);
        playerData = Array.Empty<PlayerCharacterData>();
        GetTree().Root.RemoveChild(old);
        GetTree().Root.AddChild(battle);
        old.Free();
        //GetTree().ChangeSceneToFile(nextBattle);
    }
}
