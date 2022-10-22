using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using static PMBattleUtilities;
using static BattleMenu;

public class PMBattle : Node
{
    private enum TurnPhase{
        Upkeep,
        PlayerMenu,
        PlayerAction,
        TurnOverPause,
        EnemyAction

    }

    [Export]
    private NodePath battleGUI;
    [Export]
    private NodePath debugPlayerOne;
    [Export]
    private NodePath debugEnemyOne;
    private PMBattleGUI gui;

    TurnPhase currentPhase;

    List<PMStatus> trackedStatusEffects = new List<PMStatus>();
    Stack<PMStatus> effectStack;
    
    private Dictionary<BattlePos, PMCharacter> battleRoster = new Dictionary<BattlePos, PMCharacter>(){
        {BattlePos.HeroThree, new PMCharacter()},
        {BattlePos.HeroTwo, new PMCharacter()},
        {BattlePos.HeroOne, new PMCharacter()},
        {BattlePos.EnemyOne, new PMCharacter()},
        {BattlePos.EnemyTwo, new PMCharacter()},
        {BattlePos.EnemyThree, new PMCharacter()}
    };
    Queue<PMCharacter> enemyBench = new Queue<PMCharacter>();

    //Tracks the amount of damage done this turn by each character in order HeroOne, HeroTwo, HeroThree, EnemyOne, EnemyTwo, EnemyThree
    
    Dictionary<PMCharacter, int> damageScoreboard = new Dictionary<PMCharacter, int>();
    //Tracks healing in the same way as damageScoreboard
    Dictionary<PMCharacter, int> healingScoreboard = new Dictionary<PMCharacter, int>();

    public bool heroTauntUp{
        get;
        private set;
    }
    public bool enemyTauntUp{
        get;
        private set;
    }

    Queue<PMPlayerAbility> playerAttacks;
    Queue<PMEnemyAbility> enemyAttacks;
    BattleRoster roster = new BattleRoster();
    public override void _Ready()
    {
        //Normal Goto upkeep functions
        currentPhase = TurnPhase.Upkeep;
        effectStack = new Stack<PMStatus>(trackedStatusEffects);
        gui = (PMBattleGUI) GetNode(battleGUI);
        //Add Debugs to BattleRosters
        roster.SetCharacter(GetNode<PMCharacter>(debugPlayerOne), BattlePos.HeroOne);
        roster.SetCharacter(GetNode<PMCharacter>(debugEnemyOne), BattlePos.EnemyOne);
        heroTauntUp = false;
        enemyTauntUp = false;
    }

    public override void _Process(float delta)
    {
        switch(currentPhase){
            case TurnPhase.Upkeep :
                foreach(PMCharacter ch in roster.GetCharacters()){
                    ch.NewTurnUpkeep();
                }

                if(trackedStatusEffects.Count == 0){ //If there's no more effects to resolve, continue
                    //TODO Check for Taunts
                    gui.ResetGUIState(roster.GetPlayerCharacters(), this);
                    currentPhase = TurnPhase.PlayerMenu;
                    playerAttacks = new Queue<PMPlayerAbility>();
                    break;            
                } 

                if(effectStack.Peek().Execute()){ //Execute the Effect, if it's done...
                    if(effectStack.Peek().GetDuration() == 0){
                        effectStack.Peek().Expire();
                        
                    }
                    effectStack.Pop();  //Remove it 
                }

                foreach(PMPlayerCharacter character in damageScoreboard.Keys){ //Reset the Player Half of the Scoreboards
                    damageScoreboard.Remove(character);
                }
                foreach(PMPlayerCharacter character in healingScoreboard.Keys){
                    healingScoreboard.Remove(character);
                }
                //TODO If Players should die, they do
                break;
            case TurnPhase.PlayerMenu :
                //Send Input to the Battle GUI
                var temp = gui.Execute(GetPlayerInput(), this);
                if(temp != null){
                    playerAttacks = temp;
                    gui.HideGUI();
                    playerAttacks.Peek().Begin();
                    currentPhase = TurnPhase.PlayerAction;
                }
                break;
            case TurnPhase.PlayerAction : 
                if(playerAttacks.Peek().CheckForCompletion()){//Peek Player Attack Stack, get notice whether the attack is still running or not
                    playerAttacks.Dequeue();
                    if(playerAttacks.Count == 0){//Is there any more attacks?
                        currentPhase = TurnPhase.TurnOverPause;
                    }else{
                        playerAttacks.Peek().Begin(); //Start the next attack, the previous attack should have reset itself
                    }
                }
                break;
            case TurnPhase.TurnOverPause : 
            /*TODO: "Specified Cast Not Valid" on these functions: Check out 
                foreach(PMEnemyCharacter character in damageScoreboard.Keys){
                    damageScoreboard.Remove(character);
                }
                foreach(PMEnemyCharacter character in healingScoreboard.Keys){
                    healingScoreboard.Remove(character);
                }
            */
                //If Enemies should die, they do
                enemyAttacks = new Queue<PMEnemyAbility>();
                foreach(PMEnemyCharacter en in roster.GetEnemyCharacters()){
                    var enAb = en.DecideAttack();
                    if(enAb != null){
                        enemyAttacks.Enqueue(enAb);
                    }
                }
                currentPhase = TurnPhase.EnemyAction;
                enemyAttacks.Peek().Begin();
                break;
            case TurnPhase.EnemyAction :
                //Basically the same loop as PlayerAction but with the enemy stack 
                if(enemyAttacks.Peek().CheckForCompletion()){//Peek Player Attack Stack, get notice whether the attack is still running or not
                    enemyAttacks.Dequeue();
                    if(enemyAttacks.Count == 0){//Is there any more attacks?
                        //Setup the status effect stack, then turn it over to the next turn
                        effectStack = new Stack<PMStatus>();
                        foreach(PMStatus status in trackedStatusEffects) effectStack.Push(status);
                        effectStack.Peek().StartUpkeep();
                        currentPhase = TurnPhase.Upkeep;
                    }else{
                        enemyAttacks.Peek().Begin(); //Start the next attack, the previous attack should have reset itself
                    }
                }
                break;
        }
    }

    public PMCharacter PositionLookup(BattlePos target){
        var temp = roster.GetSingle(target);
        return roster.GetSingle(target);
    }

    public MenuInput GetPlayerInput(){
        if(Input.IsActionJustPressed("ui_up")){
            return BattleMenu.MenuInput.Up;
        }else if(Input.IsActionJustPressed("ui_right")){
            return BattleMenu.MenuInput.Right;
        }else if(Input.IsActionJustPressed("ui_down")){
            return BattleMenu.MenuInput.Down;
        }else if(Input.IsActionJustPressed("ui_left")){
            return BattleMenu.MenuInput.Left;
        }else if(Input.IsActionJustPressed("ui_back")){
            return BattleMenu.MenuInput.Back;
        }else if(Input.IsActionJustPressed("ui_accept")){
            return BattleMenu.MenuInput.Select;
        }else{
            return BattleMenu.MenuInput.None;
        }
    }

    public void UpdateDamageScoreboard(int damage, PMCharacter character){
        int newTotal = 0;
        damageScoreboard.TryGetValue(character, out newTotal);
        damageScoreboard.Remove(character);
        newTotal += damage;
        damageScoreboard.Add(character, newTotal);
    }

    public void UpdateHealingScoreboard(int heal, PMCharacter character){
        int newTotal = 0;
        healingScoreboard.TryGetValue(character, out newTotal);
        healingScoreboard.Remove(character);
        newTotal += heal;
        healingScoreboard.Add(character, newTotal);
    }

    public PMPlayerCharacter GetPlayerDamageLeader(){
        int lead = 0;
        PMPlayerCharacter leader = null;
        foreach(PMCharacter testCharacter in damageScoreboard.Keys){
            if(testCharacter.GetType() == typeof(PMPlayerCharacter)){
                damageScoreboard.TryGetValue(testCharacter, out var temp);
                if(temp > lead){
                    lead = temp;
                    leader = (PMPlayerCharacter) testCharacter;
                }
            }
        }
        return leader;
    }

    public PMPlayerCharacter GetPlayerHealingLeader(){
        int lead = 0;
        PMPlayerCharacter leader = null;
        foreach(PMCharacter testCharacter in damageScoreboard.Keys){
            if(testCharacter.GetType() == typeof(PMPlayerCharacter)){
                healingScoreboard.TryGetValue(testCharacter, out var temp);
                if(temp > lead){
                    lead = temp;
                    leader = (PMPlayerCharacter) testCharacter;
                }
            }
        }
        return leader;
    }

    public PMPlayerCharacter[] GetPlayerCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true){
        return roster.GetPlayerCharacters(includeFlying, includeInvisible, includePhasedOut);
    }

    public PMEnemyCharacter[] GetEnemyCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true){
        return roster.GetEnemyCharacters(includeFlying, includeInvisible, includePhasedOut);
    }

    public PMCharacter[] GetCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true){
        return roster.GetCharacters(includeFlying, includeInvisible, includePhasedOut);
    }

    private void SetNewPosition(BattlePos newPosition, PMCharacter ch){ //TODO make align with positioning rules

    }
}

//Designed to handle where characters are standing in the battle
public class BattleRoster{
    //Note: given how BattlePos' are bitwise flagged, using "log(2)" on a battle pos gets the correct position in the characters array
    private PMCharacter[] characters = new PMCharacter[6];

    public void SetCharacter(PMCharacter ch, BattlePos pos){
        characters[(uint)Math.Log((uint)pos, 2)] = ch;
        ch.myPosition = pos;
    }
    public PMCharacter GetSingle(BattlePos pos){
        return characters[(uint)Math.Log((uint)pos, 2)];
    }

    public List<PMCharacter> GetGroup(BattlePos[] pos){
        List<PMCharacter> group = new List<PMCharacter>();
        foreach(BattlePos addPos in pos){
            group.Add(GetSingle(addPos));
        }
        return group;
    }

    public void MoveCharacter(PMCharacter ch, BattlePos pos){   

    }
    
    //Returns all player characters, allowing to filter them by invisible, flying, and phased out.
    public PMPlayerCharacter[] GetPlayerCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true){
        List<PMPlayerCharacter> result = new List<PMPlayerCharacter>();
        var temp = characters.ToList<PMCharacter>();
        temp.RemoveAll(x => x == null);
        foreach(PMCharacter ch in temp){
            if(ch.GetType() == typeof(PMPlayerCharacter)){
                List<StatusEffect> chStatus = ch.GetMyStatuses();
                if(!includeInvisible){
                    if(chStatus.Contains(StatusEffect.Invisible)){
                        continue;
                    }
                }
                if(!includeFlying){
                    if(chStatus.Contains(StatusEffect.Flying)){
                        continue;
                    }
                }
                if(!includePhasedOut){
                    if(chStatus.Contains(StatusEffect.PhasedOut)){
                        continue;
                    }
                }
                result.Add((PMPlayerCharacter)ch);
            }
        }
        return result.ToArray();
    }

    public PMEnemyCharacter[] GetEnemyCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true){
        List<PMEnemyCharacter> result = new List<PMEnemyCharacter>();
        var temp = characters.ToList<PMCharacter>();
        temp.RemoveAll(x => x == null);
        foreach(PMCharacter ch in temp){
            if(ch.GetType() == typeof(PMEnemyCharacter)){
                List<StatusEffect> chStatus = ch.GetMyStatuses();
                if(!includeInvisible){
                    if(chStatus.Contains(StatusEffect.Invisible)){
                        continue;
                    }
                }
                if(!includeFlying){
                    if(chStatus.Contains(StatusEffect.Flying)){
                        continue;
                    }
                }
                if(!includePhasedOut){
                    if(chStatus.Contains(StatusEffect.PhasedOut)){
                        continue;
                    }
                }
                result.Add((PMEnemyCharacter)ch);
            }
        }
        return result.ToArray();
    }

    public PMCharacter[] GetCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true){
        List<PMCharacter> temp = characters.ToList<PMCharacter>();
        temp.RemoveAll(x => x == null);
        foreach(PMCharacter ch in temp){
            List<StatusEffect> statuses = ch.GetMyStatuses();
            if(!includeFlying && statuses.Contains(StatusEffect.Flying)) temp.Remove(ch);
            if(!includeInvisible && statuses.Contains(StatusEffect.Flying)) temp.Remove(ch);
            if(!includePhasedOut && statuses.Contains(StatusEffect.PhasedOut)) temp.Remove(ch);
        } 
        return temp.ToArray<PMCharacter>();
    }
}

public static class PMBattleUtilities{

    public static NodePath pathToBattle = "/root";

    public enum AbilityAlignment{
        Normal = 0,
        Magic = 1,
        Tech = 2
    }

    public enum EventType{
        None = 0,
        Damage = 1,
        Status = 2,
        Healing = 3
    }

    public enum EnemyRole{
        Minion = 0,
        Tank = 1,
        Bruiser = 2,
        Artillery = 3,
        Support = 4,
        SquadLeader = 5,
        Boss = 6
    }

    [Flags]
    public enum BattlePos{                                                                                                                                                                        
        HeroOne = 0b_001000,
        HeroTwo = 0b_010000,                                                                            
        HeroThree = 0b_100000,
        EnemyOne = 0b_000100,
        EnemyTwo = 0b_000010,
        EnemyThree = 0b_000001
    }                                                                                                    

    [Flags]
    public enum TargetingRule{
        None = 0,
        Self = 1,
        SingleEnemyMelee = 2,
        SingleEnemyRanged = 3,
        SingleEnemyReach = 4,
        AllEnemy = 5,
        AllHero = 6,                                                                                                                           
        SingleHeroMelee = 7,
        SingleHeroRanged = 8,
        SingleHeroReach = 9,
        HeroOne = 10,
        HeroTwo = 11,
        HeroThree = 12,
        EnemyOne = 13,
        EnemyTwo = 14,
        EnemyThree = 15,
        All = 16
    }

    /*
    public BattlePos TargetingRuleToBattlePos(TargetingRule rule){
        switch(rule){
            //case 
        }
    }
    */

    [Flags]
    public enum StatusEffect{
        None,
        Burn,
        Freeze,
        Poisoned,
        Stunned,
        Flying,
        Silenced,
        Jinxed,
        Overcharged,
        Empowered,
        Invisible,
        PhasedOut,
        Taunting
    }
}
