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
        EnemyAction,
        HandleDefeat
    }

    [Export]
    private NodePath battleGUI;
    [Export]
    private NodePath debugPlayerOne;
    [Export]
    private NodePath debugPlayerTwo;
    [Export]
    private NodePath debugEnemyOne;
    [Export]
    private NodePath debugEnemyTwo;
    [Export]
    private NodePath debugEnemyThree;
    private PMBattleGUI gui;

    TurnPhase currentPhase;
    TurnPhase returnPhase; //What phase do we return to after we've handled defeats

    List<PMStatus> trackedStatusEffects = new List<PMStatus>();
    Stack<PMStatus> effectStack;

    Queue<PMCharacter> enemyBench = new Queue<PMCharacter>(); //Holds onto what enemies are waiting to join the encounter once there's space
    List<PMCharacter> deadPool = new List<PMCharacter>(); //Holds onto what characters are defeated and currently playing their defeat animations

    //Tracks the amount of damage done this turn by each character in order HeroOne, HeroTwo, HeroThree, EnemyOne, EnemyTwo, EnemyThree
    Dictionary<PMCharacter, int> damageScoreboard = new Dictionary<PMCharacter, int>();
    //Tracks healing in the same way as damageScoreboard
    Dictionary<PMCharacter, int> healingScoreboard = new Dictionary<PMCharacter, int>();

    PMBattlePositionManager posManager;
    //Used to store which two positions we're switching between
    uint currentSwap = 0;
    float timer = 0;
    AnimationPlayer animPlay;
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

    //Used to track what character we know are defeated, so they don't come up every time we check for defeats
    List<PMPlayerCharacter> knownDownedCharacters = new List<PMPlayerCharacter>();
    public override void _Ready()
    {
        //Normal Goto upkeep functions
        currentPhase = TurnPhase.Upkeep;
        effectStack = new Stack<PMStatus>(trackedStatusEffects);
        gui = (PMBattleGUI) GetNode(battleGUI);
        //Add Debugs to BattleRosters
        roster.SetCharacter(GetNode<PMCharacter>(debugPlayerOne), BattlePos.HeroOne);
        roster.SetCharacter(GetNode<PMCharacter>(debugPlayerTwo), BattlePos.HeroTwo);
        roster.SetCharacter(GetNode<PMCharacter>(debugEnemyOne), BattlePos.EnemyOne);
        roster.SetCharacter(GetNode<PMCharacter>(debugEnemyTwo), BattlePos.EnemyTwo);
        roster.SetCharacter(GetNode<PMCharacter>(debugEnemyThree), BattlePos.EnemyThree);
        heroTauntUp = false;
        enemyTauntUp = false;
        animPlay = this.GetNode<AnimationPlayer>("Battle Positions/AnimationPlayer");
        posManager = this.GetNode<PMBattlePositionManager>("Battle Positions");
    }

    public override void _Process(float delta)
    {
        switch(currentPhase){
            case TurnPhase.Upkeep :
                foreach(PMCharacter ch in roster.GetCharacters()){
                    ch.NewTurnUpkeep();
                }

                if(effectStack.Count == 0){ //If there's no more effects to resolve, continue
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
                    CheckForNewDefeats();
                    effectStack.Pop();  //Remove it 
                }
                /*TODO: "Specified Cast Not Valid" on these functions: Check out
                foreach(PMPlayerCharacter character in roster.GetPlayerCharacters()){ //Reset the Player Half of the Scoreboards
                    damageScoreboard.Remove(character);
                }
                foreach(PMPlayerCharacter character in roster.GetPlayerCharacters()){
                    healingScoreboard.Remove(character);
                }
                */
                //TODO If Players should die, they do
                break;
            case TurnPhase.PlayerMenu :
                //Send Input to the Battle GUI
                var temp = gui.Execute(GetPlayerInput(), this);
                if(temp != null){
                    playerAttacks = temp;
                    gui.HideGUI();
                    if(temp.Count == 0){
                        currentPhase = TurnPhase.TurnOverPause;
                    }else{
                        playerAttacks.Peek().Begin();
                        currentPhase = TurnPhase.PlayerAction;
                    }
                }
                break;
            case TurnPhase.PlayerAction : 
                if(playerAttacks.Peek().CheckForCompletion()){//Peek Player Attack Stack, get notice whether the attack is still running or not
                    playerAttacks.Dequeue();
                    if(playerAttacks.Count == 0){//Is there any more attacks?
                        if(CheckForNewDefeats()){
                            returnPhase = TurnPhase.TurnOverPause;
                            currentPhase = TurnPhase.HandleDefeat;
                        }
                        else currentPhase = TurnPhase.TurnOverPause;
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
            
                if(timer < 2){
                    timer += delta;
                    return;
                }else{
                    timer = 0;
                }
            
                //If Enemies should die, they do
                enemyAttacks = new Queue<PMEnemyAbility>();
                foreach(PMEnemyCharacter en in roster.GetEnemyCharacters()){
                    var enAb = en.DecideAttack();
                    if(enAb != null){
                        enemyAttacks.Enqueue(enAb);
                    }
                }

                enemyAttacks.Peek().Begin();
                currentPhase = TurnPhase.EnemyAction;
                break;
            case TurnPhase.EnemyAction :
                //Basically the same loop as PlayerAction but with the enemy stack 
                if(enemyAttacks.Peek().CheckForCompletion()){//Peek Player Attack Stack, get notice whether the attack is still running or not
                    enemyAttacks.Dequeue();
                    if(enemyAttacks.Count == 0){//Is there any more attacks?
                        //Setup the status effect stack, then turn it over to the next turn
                        effectStack = new Stack<PMStatus>();
                        foreach(PMStatus status in trackedStatusEffects){
                            if(status == null){ trackedStatusEffects.Remove(status); return; }
                            effectStack.Push(status);
                        }
                        
                        if(CheckForNewDefeats()){
                            returnPhase = TurnPhase.Upkeep;
                            currentPhase = TurnPhase.HandleDefeat;
                        }else{
                            if(effectStack.Count != 0) effectStack.Peek().StartUpkeep();
                            currentPhase = TurnPhase.Upkeep;
                        }
                    }else{
                        enemyAttacks.Peek().Begin(); //Start the next attack, the previous attack should have reset itself
                    }
                }
                break;
            case TurnPhase.HandleDefeat :

                var defeatDone = true;
                foreach(PMCharacter character in deadPool){
                    if(character.DefeatMe() == false){
                        defeatDone = false;
                    }
                }
                if(defeatDone){
                    if(GetPlayerCharacters(true, true, true, false).Count() == 0){ //if there are no undefeated heroes
                        //End the Battle with a Game Over
                        GD.Print("Game Over");
                        GetTree().Quit();
                    }
                    if(GetEnemyCharacters(true, true, true, false).Count() == 0){
                        //End the Battle with Victory
                        GD.Print("You Win!");
                        GetTree().Quit();
                    }
                    //TODO make this \/ less hideous   
                    var checkSlots = roster.GetPlayersForPushing();
                    if(checkSlots[0] == false){
                        if(checkSlots[1] == false){
                            if(checkSlots[2]){
                                StartPositionSwap(BattlePos.HeroOne, BattlePos.HeroThree);
                            }else{
                                throw new NotImplementedException(); //TODO Custom Exception : Why are we here if every hero is dead or missing
                            }
                        }else{
                            if(checkSlots[2]){
                                posManager.StartPositionCrunch(true);
                            }else{
                                posManager.StartPositionSwap(BattlePos.HeroTwo, BattlePos.HeroOne);
                            }
                        }
                    }else{
                        if(checkSlots[1] == false){
                            StartPositionSwap(BattlePos.HeroThree, BattlePos.HeroTwo);
                        }
                    }
                    checkSlots = roster.GetEnemiesForPushing();
                    if(checkSlots[0] == false){
                        if(checkSlots[1] == false){
                            if(checkSlots[2]){
                                StartPositionSwap(BattlePos.EnemyOne, BattlePos.EnemyThree);
                            }else{
                                throw new NotImplementedException(); //TODO Custom Exception : Why are we here if every hero is dead or missing
                            }
                        }else{
                            if(checkSlots[2]){
                                posManager.StartPositionCrunch(false);
                            }else{
                                posManager.StartPositionSwap(BattlePos.EnemyTwo, BattlePos.EnemyOne);
                            }
                        }
                    }else{
                        if(checkSlots[1] == false){
                            StartPositionSwap(BattlePos.EnemyThree, BattlePos.EnemyTwo);
                        }
                    } 
                    currentPhase = returnPhase; //Only reaches here if there's still a fight
                    //Add something so we're not dependant on the turnover pause to handle death...
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

    public PMPlayerCharacter[] GetPlayerCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true, bool includeDefeated = true){
        return roster.GetPlayerCharacters(includeFlying, includeInvisible, includePhasedOut, includeDefeated);
    }

    public PMEnemyCharacter[] GetEnemyCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = false, bool includeDefeated = false){
        return roster.GetEnemyCharacters(includeFlying, includeInvisible, includePhasedOut, includeDefeated);
    }

    public PMCharacter[] GetCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true, bool includeDefeated = false){
        return roster.GetCharacters(includeFlying, includeInvisible, includePhasedOut, includeDefeated);
    }

    public void LogStatusEffect(PMStatus stat){
        trackedStatusEffects.Add(stat);
    }

    public bool CheckForNewDefeats(){
        deadPool.Clear();
        foreach(PMCharacter character in GetCharacters(true, true, true, true)){
            if(character.GetHP() <= 0){
                if(knownDownedCharacters.Contains(character)){
                    continue;
                }
                deadPool.Add(character);
                if(character is PMPlayerCharacter) knownDownedCharacters.Add((PMPlayerCharacter)character);
            }
        }
        return (deadPool.Count > 0);
    }

    public void StartPositionSwap(BattlePos one, BattlePos two){
        posManager.StartPositionSwap(one, two);
    }

    public void FinishPositionSwap(){
        posManager.FinishPositionSwap(roster);
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

    //Returns all player characters, allowing to filter them by invisible, flying, and phased out.
    public PMPlayerCharacter[] GetPlayerCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true, bool includeDefeated = false){
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
                if(!includeDefeated){
                    if(ch.GetHP() <= 0){
                        continue;
                    }
                }
                result.Add((PMPlayerCharacter)ch);
            }
        }
        return result.ToArray();
    }

    public PMEnemyCharacter[] GetEnemyCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true, bool includeDefeated = false){
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
                if(!includeDefeated){
                    if(ch.GetHP() <= 0){
                        continue;
                    }
                }
                result.Add((PMEnemyCharacter)ch);
            }
        }
        return result.ToArray();
    }

    public PMCharacter[] GetCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true, bool includeDefeated = true){
        List<PMCharacter> temp = characters.ToList<PMCharacter>();
        temp.RemoveAll(x => x == null);
        foreach(PMCharacter ch in temp){
            List<StatusEffect> statuses = ch.GetMyStatuses();
            if(!includeFlying && statuses.Contains(StatusEffect.Flying)) temp.Remove(ch);
            if(!includeInvisible && statuses.Contains(StatusEffect.Flying)) temp.Remove(ch);
            if(!includePhasedOut && statuses.Contains(StatusEffect.PhasedOut)) temp.Remove(ch);
            if(!includeDefeated && ch.GetHP() <= 0) temp.Remove(ch);
        } 
        return temp.ToArray<PMCharacter>();
    }

    //Returns an array indicating whether there's an active player on each slot: used for when we need to push inactive player character to the rear
    public bool[] GetPlayersForPushing(){
        var result = new bool[3];
        for(int j = 0; j < 3; j++){
            if(characters[j] == null || characters[j].GetHP() <= 0){
                result[j]  = false;
            }else{
                result[j] = true;
            }
        }  
        return result;
    }
    
    public bool[] GetEnemiesForPushing(){
        bool[] result = new bool[3];
        for(int j = 5; j > 2; j--){
            if(characters[j] == null || characters[j].GetHP() <= 0){
                var t = 5 - j;
                result[t] = false;
            }else{
                var t = 5 - j;
                result[t] = true;
            }
        }  
        return result;
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
    //NOTE: Targeting rules less than ten require some choices, Targeting rules above ten are preset
    public enum TargetingRule{
        None = 0,
        SingleEnemyMelee = 1,
        SingleEnemyRanged = 2,
        SingleEnemyReach = 3,                                                                                                                         
        SingleHeroMelee = 4,
        SingleHeroRanged = 5,
        SingleHeroReach = 6,
        Self = 10,
        AllEnemy = 18,
        AllHero = 19,
        All = 20
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
