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
	[Export]
	PackedScene mainScene;
    PlayerData[] partyData = new PlayerData[3];
    List<Item> inventory = new List<Item>();

    PlayerData[] bookmark = new PlayerData[3];

    private string spawnPoint = "";

    [Export(PropertyHint.Range, "0,3")]
    private int DayTimeUnitsRemaining = 3;

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
        partyData = new PlayerData[3]{
            new PlayerData("Cato", "res://Battle Mode/Player Characters/Cato Combatant.tscn", 2, 6, 2, 2, new BattlePosition(BattleUtilities.BattleLane.Center, BattleUtilities.BattleRank.HeroFront)),
            new PlayerData("Luciene", "res://Battle Mode/Player Characters/Lucienne Combatant.tscn", 5,5, 3,3, new BattlePosition(BattleUtilities.BattleLane.Center, BattleUtilities.BattleRank.HeroMid)),
            new PlayerData("Silver", "res://Battle Mode/Player Characters/Silver Combatant.tscn", 4,4, 4,4, new BattlePosition(BattleUtilities.BattleLane.Center, BattleUtilities.BattleRank.HeroBack))
            };
        inventory = new List<Item>();
        currentTime = TimeOfDay.Morning;
        ProcessMode = ProcessModeEnum.Always;
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
        if(Input.IsActionJustPressed("ui_start")){return PlayerInput.Start;}
        return PlayerInput.None;
    }

    public void SavePlayerParty(Roster roster){
        PlayerCombatant[] currentParty = roster.GetAllPlayerCombatants();
        SavePlayerParty(currentParty);
    }

    public void SavePlayerParty(PlayerCombatant[] currentParty){
        for(int i = 0; i < currentParty.Length; i++){partyData[i] = currentParty[i].GetPlayerData();} 
    }

    public void LoadPartyData(Roster roster){
        foreach(PlayerData playerCombatantData in partyData){
            var playerCharacter = GD.Load<PackedScene>(playerCombatantData.GetPath()).Instantiate<PlayerCombatant>();
            playerCharacter.LoadPlayerData(playerCombatantData);
            roster.SetPositionNewCharacter(playerCharacter, playerCombatantData.GetStartingPosition());
        }
        foreach(Item item in inventory){
            roster.GetNode("Items").AddChild(item);
        }
    }

    public PlayerCombatant[] LoadPartyData(){
        List<PlayerCombatant> currentParty = new();
        foreach(PlayerData playerCombatantData in partyData){
            var playerCharacter = GD.Load<PackedScene>(playerCombatantData.GetPath()).Instantiate<PlayerCombatant>();
            playerCharacter.LoadPlayerData(playerCombatantData);
            currentParty.Add(playerCharacter);
        }
        return currentParty.ToArray();
    }

    public void GainItem(Item item){
        inventory.Add(item);
    }

    public void RestorePartyHPAndSP(){
        foreach(PlayerData data in partyData.Where(x => x != null)){
            data.RestoreHP();
            data.RestoreSP();
        }
    }

    public void RestorePartyHP(){
        foreach(PlayerData data in partyData.Where(x => x != null)){
            data.RestoreHP();
        }
    }

    public void RestoreCharacterHP(string characterName){
        foreach(PlayerData data in partyData.Where(x => x != null)){
            if(data.GetName() == characterName){
                data.RestoreHP();
                return;
            }
        }
        GetTree().Quit();
        throw new ArgumentException("Character Name: " + characterName + " not found in current party...");
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

    public void NewDay(){
        DayTimeUnitsRemaining = 3;
        EmitSignal(GameMaster.SignalName.TimeOfDayChanged, DayTimeUnitsRemaining);
    }

    public int GetCurrentTU(){
        return DayTimeUnitsRemaining;
    }

    public void SpendTU(int spend){
        if(DayTimeUnitsRemaining < spend){
            GetTree().Quit();
            throw new ArgumentException("Not enough Time Units to Spend!");
        }
        DayTimeUnitsRemaining -= spend;
        EmitSignal(GameMaster.SignalName.TimeOfDayChanged, DayTimeUnitsRemaining);
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

    public List<Item> GetInventory(){
        return inventory;
    }
    public PackedScene GetMainScenePacked(){
        return mainScene;
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
        Start,
        None,
    }
}
