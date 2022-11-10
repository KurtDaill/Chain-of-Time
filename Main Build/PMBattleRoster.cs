using Godot;
using System;
using static PMBattleUtilities;
using System.Collections.Generic;
using System.Linq;


//Designed to handle where characters are standing in the battle

public class PMBattleRoster : Spatial
{
    enum DefeatStage{
        Check,
        Animate,
        Swap,
        Done
    }
    DefeatStage stage = DefeatStage.Check;
    bool waitingOnSwaps;
    [Export]
    bool debugMode = false;
    //Note: given how BattlePos' are bitwise flagged, using "log(2)" on a battle pos gets the correct position in the characters array
    private PMCharacter[] characters = new PMCharacter[6];
    private List<PMCharacter> deadPool = new List<PMCharacter>();
    uint currentSwap;
    AnimationPlayer animPlay;
    Transform[] swapTrans = new Transform[3];
    Spatial[] swapNode = new Spatial[3];
    PMCharacter[] swapChar = new PMCharacter[3];
    BattlePos[] originalPosition = new BattlePos[3]; 

    //Used to handle whether we're in the special case where slots 2 & 3 have to movce forward at the same time: a "crunch"
    bool crunch;
    public override void _Ready(){
        animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
        if(debugMode){
            for(int i = 0; i < 6; i++){
                var character = this.GetChild(i).GetChildOrNull<PMCharacter>(0);
                if(character != null) SetCharacter(character, (BattlePos)(Mathf.Pow(2, (5-i))));
            }
        }
    }
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
        var result = new List<PMCharacter>();
        foreach(PMCharacter ch in characters){
            if(ch == null) continue;
            List<StatusEffect> statuses = ch.GetMyStatuses();
            if(!includeFlying && statuses.Contains(StatusEffect.Flying)) continue;
            if(!includeInvisible && statuses.Contains(StatusEffect.Flying)) continue;
            if(!includePhasedOut && statuses.Contains(StatusEffect.PhasedOut)) continue;
            if(!includeDefeated && ch.GetHP() <= 0) continue;
            result.Add(ch); //If none of the previous conditions are met, we need this character
        } 
        return result.ToArray();
    }

    //Returns an array indicating whether there's an active player on each slot: used for when we need to push inactive player character to the rear
    public bool[] GetEnemiesForPushing(){
        var result = new bool[3];
        for(int j = 0; j < 3; j++){
            if(characters[j] == null || characters[j].GetHP() <= 0){
                result[2-j]  = false;
            }else{
                result[2-j] = true;
            }
        }  
        return result;
    }
    
    public bool[] GetPlayersForPushing(){
        bool[] result = new bool[3];
        for(int j = 5; j > 2; j--){
            if(characters[j] == null || characters[j].GetHP() <= 0){
                result[j-3] = false;
            }else{
                result[j-3] = true;
            }
        }  
        return result;
    }
    /*
        Start Position Swap begins swapping postitions of two player character.
        It triggers an animation based on the given positions, and at the end of those
        animations, the "CompletePositionSwap" function is called using the "Call Method" track
    */ 
    public void StartPositionSwap(BattlePos PositionOne, BattlePos PositionTwo){
        currentSwap = (uint)PositionOne | (uint)PositionTwo;
        switch(currentSwap){
            case 0b_011000:
                animPlay.Play("SwapH12");
                swapNode[0] = this.GetNode<Spatial>("Hero 1");
                originalPosition[0] = BattlePos.HeroOne;
                swapNode[1] = this.GetNode<Spatial>("Hero 2");
                originalPosition[1] = BattlePos.HeroTwo;
                break;
            case 0b_101000:
                animPlay.Play("SwapH13");
                swapNode[0] = this.GetNode<Spatial>("Hero 1");
                originalPosition[0] = BattlePos.HeroOne;
                swapNode[1] = this.GetNode<Spatial>("Hero 3");
                originalPosition[1] = BattlePos.HeroThree;
                break;
            case 0b_110000:
                animPlay.Play("SwapH23");
                swapNode[0] = this.GetNode<Spatial>("Hero 2");
                originalPosition[0] = BattlePos.HeroTwo;
                swapNode[1] = this.GetNode<Spatial>("Hero 3");
                originalPosition[1] = BattlePos.HeroThree;
                break;
            case 0b_000110:
                animPlay.Play("SwapE12");
                swapNode[0] = this.GetNode<Spatial>("Enemy 1");
                originalPosition[0] = BattlePos.EnemyOne;
                swapNode[1] = this.GetNode<Spatial>("Enemy 2");
                originalPosition[1] = BattlePos.EnemyTwo;
                break;
            case 0b_000101:
                animPlay.Play("SwapE13");
                swapNode[0] = this.GetNode<Spatial>("Enemy 1");
                originalPosition[0] = BattlePos.EnemyOne;
                swapNode[1] = this.GetNode<Spatial>("Enemy 3");
                originalPosition[1] = BattlePos.EnemyThree;
                break;
            case 0b_000011:
                animPlay.Play("SwapE23");
                swapNode[0] = this.GetNode<Spatial>("Enemy 2");
                originalPosition[0] = BattlePos.EnemyTwo;
                swapNode[1] = this.GetNode<Spatial>("Enemy 3");
                originalPosition[1] = BattlePos.EnemyThree;
                break;
            default:
                throw new NotImplementedException();
        }
        swapTrans[0] = swapNode[0].Transform;
        swapChar[0] = swapNode[0].GetChild<PMCharacter>(0);

        swapTrans[1] = swapNode[1].Transform;
        swapChar[1] = swapNode[1].GetChild<PMCharacter>(0);
    }

    public void StartPositionCrunch(bool hero){
        crunch = true;
        if(hero){
            animPlay.Play("SwapHCrunch");
            swapNode[0] = this.GetNode<Spatial>("Hero 1");
            swapNode[1] = this.GetNode<Spatial>("Hero 2");
            swapNode[2] = this.GetNode<Spatial>("Hero 3");
        }else{
            animPlay.Play("SwapECrunch");
            swapNode[0] = this.GetNode<Spatial>("Enemy 1");
            swapNode[1] = this.GetNode<Spatial>("Enemy 2");
            swapNode[2] = this.GetNode<Spatial>("Enemy 3");
        }

        for(int i = 0; i < 3; i++){
            swapTrans[i] = swapNode[i].Transform;
            swapChar[i] = swapNode[i].GetChild<PMCharacter>(0);
            originalPosition[i] = swapChar[i].myPosition;
        }
    }

    public void FinishPositionSwap(){//TODO make this less ugly
        if(crunch){
            SetCharacter(swapChar[2], originalPosition[1]);
            SetCharacter(swapChar[1], originalPosition[0]);
            SetCharacter(swapChar[0], originalPosition[2]);
            for(int i = 0; i < 3; i++){
                swapNode[i].RemoveChild(swapChar[i]);
                swapNode[i].Transform = swapTrans[i];
            }
            swapNode[0].AddChild(swapChar[1]);
            swapNode[1].AddChild(swapChar[2]);
            swapNode[2].AddChild(swapChar[0]);
            crunch = false;
        }else{
            if(swapChar[1] != null)SetCharacter(swapChar[0], originalPosition[1]);
            if(swapChar[0] != null)SetCharacter(swapChar[1], originalPosition[0]);

            if(swapChar[0] != null)swapNode[0].RemoveChild(swapChar[0]);
            if(swapChar[1] != null)swapNode[1].RemoveChild(swapChar[1]);

            swapNode[0].Transform = swapTrans[0];
            swapNode[1].Transform = swapTrans[1];

            if(swapChar[0] != null)swapNode[1].AddChild(swapChar[0]);
            if(swapChar[1] != null)swapNode[0].AddChild(swapChar[1]);
        }
        currentSwap = 0;
        CheckForNeededSwaps();
    }
    //Returns true when we're in a state to continue the battle
    public bool HandleDefeat(){
        switch(stage){
            case DefeatStage.Check :
                deadPool.Clear();
                foreach(PMCharacter character in characters){
                    if(character != null){
                        if(character.GetHP() <= 0){
                            deadPool.Add(character);
                        }
                    }
                }
                if(deadPool.Count == 0) return true;
                else{
                     stage = DefeatStage.Animate;
                     return false;
                } 
            case DefeatStage.Animate :
                var done = true;
                foreach(PMCharacter character in deadPool){
                    if(!character.DefeatMe()){
                        return false;
                    }
                }
                if(done){
                    CheckForNeededSwaps();
                    stage = DefeatStage.Swap;
                }
                return false;
            case DefeatStage.Swap :
                if(!waitingOnSwaps){
                    stage = DefeatStage.Check;
                    foreach(PMCharacter character in deadPool){
                        if(character is PMEnemyCharacter){
                             character.QueueFree();
                        }
                    }
                    for(int k = 0; k < 6; k++){
                        if(characters[k] != null && characters[k].IsQueuedForDeletion()){ 
                            characters[k] = null;
                        }
                    }
                    return true;
                }
                return false;
        }
        return false;
    }

    public void CheckForNeededSwaps(){
        waitingOnSwaps = false;
        var checkSlots = GetPlayersForPushing();
        if(checkSlots[0] == false){
            if(checkSlots[1] == false){
                if(checkSlots[2]){
                    StartPositionSwap(BattlePos.HeroOne, BattlePos.HeroThree);
                    waitingOnSwaps = true;
                }else{
                    waitingOnSwaps = false;
                }
            }else{
                if(checkSlots[2]){
                    StartPositionCrunch(true);
                    waitingOnSwaps = true;
                }else{
                    StartPositionSwap(BattlePos.HeroTwo, BattlePos.HeroOne);
                    waitingOnSwaps = true;
                }
            }
        }else{
            if(checkSlots[1] == false && checkSlots[2] == true){
                StartPositionSwap(BattlePos.HeroThree, BattlePos.HeroTwo);
                    waitingOnSwaps = true;
            }
        }

        checkSlots = GetEnemiesForPushing();
        if(checkSlots[0] == false){
            if(checkSlots[1] == false){
                if(checkSlots[2]){
                    StartPositionSwap(BattlePos.EnemyOne, BattlePos.EnemyThree);
                    waitingOnSwaps = true;
                }else{
                    waitingOnSwaps = false;
                }
            }else{
                if(checkSlots[2]){
                    StartPositionCrunch(false);
                    waitingOnSwaps = true;
                }else{
                    StartPositionSwap(BattlePos.EnemyTwo, BattlePos.EnemyOne);
                    waitingOnSwaps = true;
                }
            }
        }else{
            if(checkSlots[1] == false && checkSlots[2] == true){
                StartPositionSwap(BattlePos.EnemyThree, BattlePos.EnemyTwo);
                waitingOnSwaps = true;
            }
        } 
    }
}