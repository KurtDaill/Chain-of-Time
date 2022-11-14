using Godot;
using System;

public class GameMaster : Node
{
    PlayerCharacterData[] playerData;
    public struct PlayerCharacterData{
        public PlayerCharacterData(string coreNode, int hp, int maxHP, int sp, int maxSP, uint position, int[] abilitiesKnown, int[] abilitiesPrepared){
            this.coreNode = coreNode;
            this.hp = hp;
            this.maxHP = maxHP;
            this.sp = sp;
            this.maxSP = sp;
            this.position = position;
            this.abilitiesKnown = abilitiesKnown;
            this.abilitiesPrepared = abilitiesPrepared;
        }

        public string coreNode {get; set;}
        public int hp { get; set; }
        public int sp { get; set; }
        public int maxHP { get; set; }
        public int maxSP { get; set; }
        public uint position {get; set;}
        public int[] abilitiesKnown {get; set;}
        public int [] abilitiesPrepared {get; set;}
    }

    public void SavePlayerParty(PMBattleRoster roster){
        var players = roster.GetPlayerCharacters();
        playerData = new PlayerCharacterData[players.Length];
        for(int i = 0; i < players.Length; i++){
            playerData[i] = players[i].ExportData();
        }
    }
}
