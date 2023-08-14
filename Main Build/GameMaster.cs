using Godot;
using System;
using System.Linq;
using static GameplayUtilities;
public partial class GameMaster : Node
{
    
    [Export]
    StoryState state = new StoryState();

    PlayerData[] partyData = new PlayerData[3];

    PlayerData[] bookmark = new PlayerData[3];

    GameplayMode currentMode;

    public override void _Ready()
    {
        state = GD.Load<StoryState>("res://ExampleStoryState.tres");
    }


    public override async void _Process(double delta)
    {
        base._Process(delta);
		GameplayMode returnedMode = await currentMode.RemoteProcess(delta);
        currentMode.HandleInput(ReadInput());
        if(returnedMode != null){
            await currentMode.TransitionOut();
            currentMode = returnedMode; 
            await returnedMode.StartUp();
        }
    }

    private PlayerInput ReadInput(){
        if(Input.IsActionJustPressed("ui_up")){return PlayerInput.Up;}
        if(Input.IsActionJustPressed("ui_down")){return PlayerInput.Down;}
        if(Input.IsActionJustPressed("ui_left")){return PlayerInput.Left;}
        if(Input.IsActionJustPressed("ui_down")){return PlayerInput.Right;}
        if(Input.IsActionJustPressed("ui_select")){return PlayerInput.Select;}
        if(Input.IsActionJustPressed("ui_back")){return PlayerInput.Back;}
        return PlayerInput.None;
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

    public void SetMode(GameplayMode mode){
        currentMode = mode;
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

public static class GameplayUtilities{
    public enum PlayerInput
    {
        Up,
        Right,
        Down,
        Left,
        Select,
        Back,
        None,
    }
}
