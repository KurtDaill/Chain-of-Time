using Godot;
using System;
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

    List<PMStatus> upkeepEffects = new List<PMStatus>();
    Stack<PMStatus> effectStack;

    
    PMPlayerCharacter[] playerCharacters = new PMPlayerCharacter[3];
    PMEnemyCharacter[] enemyCharacters = new PMEnemyCharacter[3];
    Queue<PMCharacter> enemyBench = new Queue<PMCharacter>();

    //Tracks the amount of damage done this turn by each character in order HeroOne, HeroTwo, HeroThree, EnemyOne, EnemyTwo, EnemyThree
    private int[] damageScoreboard = new int[6];
    //Tracks healing in the same way as damageScoreboard
    private int[] healingScoreboard = new int[6];

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
    public override void _Ready()
    {
        //Normal Goto upkeep functions
        currentPhase = TurnPhase.Upkeep;
        effectStack = new Stack<PMStatus>(upkeepEffects);
        gui = (PMBattleGUI) GetNode(battleGUI);
        playerCharacters[0] = GetNode<PMPlayerCharacter>(debugPlayerOne);
        enemyCharacters[0] = GetNode<PMEnemyCharacter>(debugEnemyOne);
        heroTauntUp = false;
        enemyTauntUp = false;
    }

    public override void _Process(float delta)
    {
        switch(currentPhase){
            case TurnPhase.Upkeep :
                foreach(PMCharacter hero in playerCharacters){
                    hero.NewTurnUpkeep();
                }
                foreach(PMCharacter enemy in enemyCharacters){
                    enemy.NewTurnUpkeep();
                }
                if(upkeepEffects.Count == 0){ //If there's no more effects to resolve, continue
                    //TODO Check for Taunts
                    currentPhase = TurnPhase.PlayerMenu;
                    playerAttacks = new Queue<PMPlayerAbility>();
                    gui.ShowGUI();
                    break;            
                } 

                if(effectStack.Peek().Execute()){ //Execute the Effect, if it's done...
                    effectStack.Pop();  //Remove it  
                }
                
                for(int i = 0; i < 3; i++){ //Reset the player half of the damage and healing scoreboard
                    damageScoreboard[i] = 0;
                    healingScoreboard[i] = 0;
                }
                //If Players should die, they do
                break;
            case TurnPhase.PlayerMenu :
                //Send Input to the Battle GUI
                gui.currentMenu.HandleInput(GetPlayerInput(),out PMPlayerAbility ability);
                if(ability != null){//Receive a Player Attack or Null
                    playerAttacks.Enqueue(ability);
                    if(gui.playerCharacterSelected == 3 || playerCharacters[gui.playerCharacterSelected + 1] == null){ //If there's no other character to input commands for...
                        currentPhase = TurnPhase.PlayerAction;
                        gui.HideGUI();
                        playerAttacks.Peek().Begin();
                    }
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
                for(int i = 3; i < 6; i++){ //Reset the enemy half of the damage and healing scoreboard
                    damageScoreboard[i] = 0;
                    healingScoreboard[i] = 0;
                }
                //If Enemies should die, they do
                currentPhase = TurnPhase.EnemyAction;
                break;
            case TurnPhase.EnemyAction :
                //Basically the same loop as PlayerAction but with the enemy stack 
                break;
        }
    }

    public PMCharacter TargetLookup(Targeting target){
        switch(target){
            case Targeting.HeroOne: 
                return playerCharacters[0];
            case Targeting.HeroTwo: 
                return playerCharacters[1];
            case Targeting.HeroThree: 
                return playerCharacters[2];
            case Targeting.EnemyOne:
                return enemyCharacters[0];
            case Targeting.EnemyTwo:
                return enemyCharacters[1];
            case Targeting.EnemyThree:
                return enemyCharacters[2];
            default :
                return null;
        }
    }

    public PMPlayerCharacter GetPlayerCharacter(int index){
        return playerCharacters[index];
    }

    public PMEnemyCharacter GetEnemyCharacter(int index){
        return enemyCharacters[index];
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

    public void GetEnemyAttackPlan(){
        enemyAttacks = new Queue<PMEnemyAbility>();
        for(int i = 0; i < 3; i++){
            enemyAttacks.Enqueue(enemyCharacters[i].DecideAttack());
        }
    }

    public void UpdateDamageScoreboard(int damage, PMCharacter character){ //TODO can we make this less hideous?
        for(int i = 0; i < 3; i++){
            if(System.Object.ReferenceEquals(playerCharacters[i], character)){
                damageScoreboard[i] += damage;
            }
        }
        for(int i = 3; i < 6; i++){
            if(System.Object.ReferenceEquals(enemyCharacters[i - 3], character)){
                damageScoreboard[i] += damage;
            }
        }
    }

    public void UpdateHealingScoreboard(int heal, PMCharacter character){ //TODO can we make this less hideous?
        for(int i = 0; i < 3; i++){
            if(System.Object.ReferenceEquals(playerCharacters[i], character)){
                healingScoreboard[i] += heal;
            }
        }
        for(int i = 3; i < 6; i++){
            if(System.Object.ReferenceEquals(enemyCharacters[i - 3], character)){
                healingScoreboard[i] += heal;
            }
        }
    }

    public PMPlayerCharacter GetPlayerDamageLeader(){
        int lead = 0;
        PMPlayerCharacter leader = null;
        for(int i = 0; i < 3; i++){
            if(damageScoreboard[i] > lead){
                leader = playerCharacters[i];
            }
        }
        return leader;
    }

    public PMPlayerCharacter GetPlayerHealingLeader(){
        int lead = 0;
        PMPlayerCharacter leader = null;
        for(int i = 0; i < 3; i++){
            if(healingScoreboard[i] > lead){
                leader = playerCharacters[i];
            }
        }
        return leader;
    }

    public Targeting GetLegalTargets(){ //TODO Refactor this, maybe associate Targeting with the bigger system?
        uint legal = 0b_000000;
        if(playerCharacters[0] != null && playerCharacters[0].IsTargetable()) legal = legal | 0b_001000;
        if(playerCharacters[1] != null && playerCharacters[1].IsTargetable()) legal = legal | 0b_010000;
        if(playerCharacters[2] != null && playerCharacters[2].IsTargetable()) legal = legal | 0b_100000;
        if(enemyCharacters[0] != null && enemyCharacters[0].IsTargetable()) legal = legal | 0b_000100;  
        if(enemyCharacters[1] != null && enemyCharacters[1].IsTargetable()) legal = legal | 0b_000010;
        if(enemyCharacters[2] != null && enemyCharacters[2].IsTargetable()) legal = legal | 0b_000001;
        return (Targeting) legal;
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
    [Flags]
    public enum Targeting{                                                                                                                                                                         
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
        SingleAlly = 6,
        AllAlly = 7,
        AllyOne = 10,
        AllyTwo = 11,
        AllyThree = 12,
        EnemyOne = 13,
        EnemyTwo = 14,
        EnemyThree = 15
    }

    [Flags]
    public enum StatusEffect{
        None,
        Burn,
        Freeze,
        Poisoned,
        Stunned,
        Flying,
        TargetLocked,
        Silenced,
        Busted,
        Overcharged,
        Invisible,
        PhasedOut,
        Taunting
    }
}
