using Godot;
using System;
using System.Linq;
public partial class GameMaster : Node
{
    
    [Export]
    StoryState state = new StoryState();

    PlayerData[] partyData = new PlayerData[3];

    PlayerData[] bookmark = new PlayerData[3];

    public override void _Ready()
    {
        state = GD.Load<StoryState>("res://ExampleStoryState.tres");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }


    public void SavePlayerParty(Roster roster){
        PlayerCombatant[] currentParty = roster.GetAllPlayerCombatants();
        for(int i = 0; i < currentParty.Length; i++){partyData[i] = currentParty[i].GetPlayerData();} 
    }

    public void LoadPartyData(Roster roster){
        PlayerCombatant[] currentParty = roster.GetAllPlayerCombatants();
        foreach(PlayerCombatant player in currentParty){
            foreach(PlayerData data in partyData.Where(x => x != null)){
                if(data.GetName() == player.GetName()){
                    player.LoadPlayerData(data);
                }
            }
        }
    }

    public void RestorePartyHPAndSP(){
        foreach(PlayerData data in partyData.Where(x => x != null)){
            data.RestoreHP();
            data.RestoreSP();
        }
    }

    public void BookmarkCurrentParty(){
        bookmark = partyData;
;
    }

    public void LoadBookmarkSave(){
        partyData = bookmark;
    }


    public StoryState GetStoryState(){
        return state;
    }

    public bool GetFlagValue(string flag){
        state.TryGetValue(flag, out int result);
        return result == 1;
        /*bool result = false;
        state.TryGetFlag(flag, out result);
        return result;
        */
    }

    public void SetFlagValue(string flag, bool value){
        state.TrySetFlag(flag, value);
    }
}

public class PlayerData{
    int hp, maxHP, sp, maxSP;
    string name;
    //Abilities Known

    public PlayerData(string name, int hp, int maxHP, int sp, int maxSP){
        this.name = name;
        this.hp = hp;
        this.maxHP = maxHP;
        this.sp = sp;
        this.maxSP = maxSP; 
    }

    public void RestoreHP(){hp = maxHP;}
    public void RestoreSP(){sp = maxSP;}

    public int GetHP(){return hp;}
    public int GetMaxHP(){return maxHP;}
    public int GetSP(){return sp;}
    public int GetMaxSP(){return maxSP;}
    public string GetName(){return name;}
}
