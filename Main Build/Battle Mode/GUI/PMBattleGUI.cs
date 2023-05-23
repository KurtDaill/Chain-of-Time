using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
//using static BattleMenu;
/*
public partial class PMBattleGUI : Control //TODO Migrate a lot of this functionality to PlayerMenuSelection
{

    public BattleMenu currentMenu;
    public BattleMenu lastMenu;
    public PMBattle parentBattle;

    private Godot.Collections.Array<PMPlayerCharacter> playersInQuestion;
    private Queue<PMPlayerAbility> abilitiesQueued;

    private bool noAbilityExit = false;

    private ReadoutContainer playerCharacterReadouts;
    public BattleMenu[] menus = new BattleMenu[7];
    public override void _Ready(){
        currentMenu = (BattleMenu) GetNode("Top Menu");
        parentBattle = (PMBattle) GetNode("../..");    
        menus[0] = (BattleMenu) GetNode("Top Menu");
        menus[1] = (BattleMenu) GetNode("Party Menu");
        menus[2] = (BattleMenu) GetNode("Item Menu");
        menus[3] = (BattleMenu) GetNode("Attack Menu");
        menus[4] = (BattleMenu) GetNode("Skill Menu");
        menus[5] = (BattleMenu) GetNode("Targeting Menu");
        menus[6] = (BattleMenu) GetNode("Swap Menu");
        playerCharacterReadouts = GetNode<ReadoutContainer>("Readouts");
    }

    //Returns the finished Queue when complete
    public Queue<PMPlayerAbility> Execute(MenuInput input, PMBattle caller){
        /*
            This function used the statement playersInQuestion[abilitiesQueued.Count] to refer to the current character
            this is due to the fact that the number of abilites we've already queued is always the index of the next character who hasn't
            specified what ability they want to use, or equals the length of playersInQuestion when we've gotten every characters action recorded
        */
        /*
        var temp = currentMenu.HandleInput(input, playersInQuestion[abilitiesQueued.Count], caller);
        if(temp != null){ 
            playersInQuestion[abilitiesQueued.Count].UnselectMe(); //The previous character should exit their excited idle/readout highlight
            abilitiesQueued.Enqueue(temp);
            
            if(abilitiesQueued.Count == playersInQuestion.Count) return abilitiesQueued; //If all players have set an ability, we go to the next step in the battle

            if(playersInQuestion[abilitiesQueued.Count].IsAbleToAct(true)){
                playersInQuestion[abilitiesQueued.Count].SelectMe();
                ChangeMenu(0, playersInQuestion[abilitiesQueued.Count], caller);
            }else{
                playersInQuestion.Remove(playersInQuestion[abilitiesQueued.Count]);
                //This is checked twice to cover the case that the last character in the order wasn't able to go
                if(abilitiesQueued.Count == playersInQuestion.Count) return abilitiesQueued;
            }
        } 
        if(noAbilityExit){ 
            playersInQuestion[abilitiesQueued.Count].UnselectMe(); //The previous character should exit their excited idle
            abilitiesQueued.Enqueue(null);
            if(abilitiesQueued.Count == playersInQuestion.Count){
                noAbilityExit = false;
                return abilitiesQueued; //If all players have set an ability, we go to the next step in the battle
            }
            else{
                playersInQuestion[abilitiesQueued.Count].SelectMe();
                ChangeMenu(0, playersInQuestion[abilitiesQueued.Count], caller);
            }
            noAbilityExit = false;
        }
        
        return null;
    }
    public void ShowGUI(){
        currentMenu.Visible = true;
        playerCharacterReadouts.Visible = true;
    }

    public void HideGUI(bool keepReadouts = true){
        currentMenu.Visible = false;
        if(!keepReadouts) playerCharacterReadouts.Visible = false;
    }

    //returns true if we have any characters able to act, false otherwise
    public bool ResetGUIState(PMPlayerCharacter[] characters, PMBattle caller){
        abilitiesQueued = new Queue<PMPlayerAbility>();
        playersInQuestion = new Godot.Collections.Array<PMPlayerCharacter>(characters);
        currentMenu.Visible = false;
        lastMenu = currentMenu;
        currentMenu = (BattleMenu) GetNode("Top Menu");
        if(playersInQuestion[0].IsAbleToAct(true)){
            playersInQuestion[0].SelectMe();
        }else if(playersInQuestion.Count > 1 && playersInQuestion[1].IsAbleToAct(true)){
            playersInQuestion[1].SelectMe();
            playersInQuestion.Remove(playersInQuestion[0]);
        }else if (playersInQuestion.Count > 2 && playersInQuestion[2].IsAbleToAct(true)){
            playersInQuestion[2].SelectMe();
            playersInQuestion.Remove(playersInQuestion[0]);
            playersInQuestion.Remove(playersInQuestion[1]);
        }else{
            return false;
        }
        currentMenu.OnOpen(playersInQuestion[abilitiesQueued.Count], caller);
        ShowGUI();
        return true;
    }

    //Changes menu. Pass -1 into newMenuIndex to goto the last menu
    public void ChangeMenu(int newMenuIndex, PMPlayerCharacter character, PMBattle caller){
        currentMenu.Visible = false;
        if(newMenuIndex == -1){
            var temp = currentMenu;
            currentMenu = lastMenu;
            lastMenu = temp;
        }else{
            lastMenu = currentMenu;
            currentMenu = menus[newMenuIndex];
        }
        currentMenu.OnOpen(character, caller);
    }

    public void ExitWithoutQueueingAbility(PMPlayerCharacter character){
        //var temp = playersInQuestion.ToList();
        //temp.RemoveAt(character);
        //playersInQuestion = temp.ToArray();
        noAbilityExit = true;
    }

    public void GotoPreviousCharacter(PMBattle caller){
        if(abilitiesQueued.Count > 0){
            playersInQuestion[abilitiesQueued.Count].UnselectMe();
            var resetQueue = abilitiesQueued.ToList<PMPlayerAbility>();
            resetQueue.Remove(resetQueue.Last<PMPlayerAbility>());
            abilitiesQueued = new Queue<PMPlayerAbility>();
            foreach(PMPlayerAbility ab in resetQueue){
                abilitiesQueued.Enqueue(ab);
            }
            
            playersInQuestion[abilitiesQueued.Count].SelectMe();
            ChangeMenu(0, playersInQuestion[abilitiesQueued.Count], caller);
        }
    }
}
*/