using Godot;
using System;
using System.Linq;
using static GameplayUtilities;
using System.Collections.Generic;
public partial class GameMaster : Node
{
    [Signal]
    public delegate void GameModeBeginEventHandler(GameplayMode beginningMode);
    [Export]
    StoryState state = new StoryState();
    [Export]
    int numberOfBuildingsDestroyedForGameOver = 5;
    PlayerData[] partyData = new PlayerData[3];
    List<Item> inventory = new List<Item>();

    PlayerData[] bookmark = new PlayerData[3];

    private string spawnPoint = "";

    GameplayMode currentMode;
    TimeOfDay currentTime;
    public enum TimeOfDay{
        Morning = 0,
        Noon = 1,
        Evening = 2,
        Night = 3
    }
    [Signal]
    public delegate void TimeOfDayChangedEventHandler(int time);
    [Signal]
    public delegate void NightBeginsEventHandler();
    public override void _Ready()
    {
        state = GD.Load<StoryState>("res://ExampleStoryState.tres");
        //Default Party Data
        partyData = new PlayerData[1]{new PlayerData("Cato", "res://Battle Mode/Player Characters/Cato Combatant.tscn", 6, 6, 2, 2, new BattlePosition(BattleUtilities.BattleLane.Center, BattleUtilities.BattleRank.HeroFront))};
        inventory = new List<Item>{GD.Load<PackedScene>("res://Battle Mode/Items/OrcishFireBrew.tscn").Instantiate<OrcishFireBrew>(), GD.Load<PackedScene>("res://Battle Mode/Items/OrcishFireBrew.tscn").Instantiate<OrcishFireBrew>()};
        currentTime = TimeOfDay.Morning;
    }


    public override async void _Process(double delta)
    {
        base._Process(delta);
        GameplayMode returnedMode = await currentMode.RemoteProcess(delta);
        currentMode.HandleInput(ReadInput());
        if(returnedMode != null){
            SetMode(returnedMode);
        }
    }

    private PlayerInput ReadInput(){
        if(Input.IsActionJustPressed("ui_up")){return PlayerInput.Up;}
        if(Input.IsActionJustPressed("ui_down")){return PlayerInput.Down;}
        if(Input.IsActionJustPressed("ui_left")){return PlayerInput.Left;}
        if(Input.IsActionJustPressed("ui_right")){return PlayerInput.Right;}
        if(Input.IsActionJustPressed("ui_accept")){return PlayerInput.Select;}
        if(Input.IsActionJustPressed("ui_back")){return PlayerInput.Back;}
        return PlayerInput.None;
    }

    public void SavePlayerParty(Roster roster){
        PlayerCombatant[] currentParty = roster.GetAllPlayerCombatants();
        for(int i = 0; i < currentParty.Length; i++){partyData[i] = currentParty[i].GetPlayerData();} 
    }

    public void LoadPartyData(Roster roster){
        foreach(PlayerData playerCombatantData in partyData){
            var playerCharacter = GD.Load<PackedScene>(playerCombatantData.GetPath()).Instantiate<PlayerCombatant>();
            playerCharacter.LoadPlayerData(playerCombatantData);
            roster.SetPositionNewCharacter(playerCharacter, playerCombatantData.GetStartingPosition());
            foreach(Item item in inventory){
                roster.GetNode("Items").AddChild(item);
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
    public async void SetMode(GameplayMode newMode){
            if(currentMode != null && IsInstanceValid(currentMode)) await currentMode.TransitionOut();
            GameplayMode oldMode = currentMode;
            currentMode = newMode; 
            await currentMode.StartUp(oldMode);
            EmitSignal(GameMaster.SignalName.GameModeBegin, new Variant[]{newMode});
    }
    public GameplayMode GetMode(){
        return currentMode;
    }

    public void AdvanceClock(){
        if(currentTime == TimeOfDay.Night) throw new IndexOutOfRangeException("Can't advance time when already in night!");
        switch(currentTime){
            case TimeOfDay.Morning : currentTime = TimeOfDay.Noon; break;
            case TimeOfDay.Noon : currentTime = TimeOfDay.Evening ; break;
        }
        if(currentTime == TimeOfDay.Night) EmitSignal(GameMaster.SignalName.NightBegins);
        else EmitSignal(GameMaster.SignalName.TimeOfDayChanged, (int)currentTime);
    }

    public int GetNumberOfBuildingsDestroyedForGameOver(){
        return numberOfBuildingsDestroyedForGameOver;
    }

    public void SetSpawnPoint(string spawn){
        spawnPoint = spawn;
    }

    public string GetSpawnPoint(){
        return spawnPoint;
    }

    public void ClearSpawnPoint(){
        spawnPoint = "";
    }
}

public class PlayerData{
    int hp, maxHP, sp, maxSP;
    string name;
    string pathToPrefabScene;
    BattlePosition position;
    //Abilities Known

    public PlayerData(string name, string pathToPrefabScene, int hp, int maxHP, int sp, int maxSP, BattlePosition position){
        this.name = name;
        this.hp = hp;
        this.maxHP = maxHP;
        this.sp = sp;
        this.maxSP = maxSP; 
        this.pathToPrefabScene = pathToPrefabScene;
        this.position = position;
    }

    public void RestoreHP(){hp = maxHP;}
    public void RestoreSP(){sp = maxSP;}

    public int GetHP(){return hp;}
    public int GetMaxHP(){return maxHP;}
    public int GetSP(){return sp;}
    public int GetMaxSP(){return maxSP;}
    public string GetName(){return name;}
    public string GetPath(){return pathToPrefabScene;}
    public BattlePosition GetStartingPosition(){
        return position;
    }
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
