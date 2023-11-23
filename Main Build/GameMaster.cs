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

    [Export]
    int nightNumber = 1;

    private string spawnPoint = "";

    [Export(PropertyHint.Range, "0,3")]
    private int DayTimeUnitsRemaining = 3;
    private List<StringName> tutorialPopupsThatHaveActivated;
    [Export]
    private bool tutorialEnalbed = true;

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
        inventory = new List<Item>(){
            GD.Load<PackedScene>("res://Battle Mode/Items/IronSoulTablet.tscn").Instantiate() as Item
            };
        currentTime = TimeOfDay.Morning;
        ProcessMode = ProcessModeEnum.Always;
        //TODO Save this along with everything else in the files!
        tutorialPopupsThatHaveActivated = new();
    }


    public override async void _Process(double delta)
    {
        base._Process(delta);
        GameplayMode returnedMode = await currentMode.RemoteProcess(delta); //There was some kind of failure to exit a battle that had an exception around here...TODO figure that out?
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

    //Wrapping this function incase I want to change how other objects get to Read Input
    public PlayerInput ReadInputRemotely(){
        return ReadInput();
    }

    public void SavePartyData(Roster roster){
        PlayerCombatant[] currentParty = roster.GetAllPlayerCombatants();
        SavePlayerParty(currentParty);
        var tempInv = roster.GetNode("Items").GetChildren().Where(x => x is Item).Cast<Item>().ToList();
        inventory = new List<Item>();
        //We have to dupe the items to get around an issue with Battle deleting the items attached to it
        foreach(Item item in tempInv){
            inventory.Add(item.Duplicate() as Item);
        }
    }

    private void SavePlayerParty(PlayerCombatant[] currentParty){
        for(int i = 0; i < currentParty.Length; i++){partyData[i] = currentParty[i].GetPlayerData();} 
    }

    public void LoadPartyData(Roster roster){
        foreach(PlayerData playerCombatantData in partyData){
            var playerCharacter = GD.Load<PackedScene>(playerCombatantData.GetPath()).Instantiate<PlayerCombatant>();
            playerCharacter.LoadPlayerData(playerCombatantData);
            roster.SetPositionNewCharacter(playerCharacter, playerCombatantData.GetStartingPosition());
        }
        foreach(Item item in inventory){
            roster.GetNode("Items").AddChild(item.Duplicate());
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

    public void RestorePartySP(){
        foreach(PlayerData data in partyData.Where(x => x != null)){
            data.RestoreSP();
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

    public void SetPartyHPToMinimumOfOne(){
        foreach(PlayerData data in partyData.Where(x => x != null)){
            if(data.GetHP() <= 0){
                data.SetHPToMinimumOfOne();
            }
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

    public int GetNightNumber(){
        return nightNumber;
    }

    private void IncrementNight(){
        nightNumber++;
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
        IncrementNight();
        RestorePartySP();
        SetPartyHPToMinimumOfOne(); //TODO - Make characters who were downed have only half max hp?
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

    //
    public bool TutorialTriggerCheckIn(StringName triggerName){
        if(!tutorialEnalbed){
            return false;
        }else{
            if(tutorialPopupsThatHaveActivated.Contains(triggerName)){
                return false;
            }else{
                tutorialPopupsThatHaveActivated.Add(triggerName);
                return true;
            }
        }
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
    public void SetHPToMinimumOfOne(){hp = Math.Max(1, hp);}
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
