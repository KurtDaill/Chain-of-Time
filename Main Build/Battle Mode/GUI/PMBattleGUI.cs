using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static BattleMenu;

public partial class PMBattleGUI : Control
{

	[Signal]
	public delegate void PlayerFinishedCommandInputEventHandler();
	public BattleMenu currentMenu;
	public BattleMenu lastMenu;
	public Battle parentBattle;

	private Godot.Collections.Array<PlayerCombatant> playersInQuestion;
	private CombatEventData[] abilitiesQueued;

	private bool noAbilityExit = false;

	private bool active = false;
	private ReadoutContainer playerCharacterReadouts;
	public BattleMenu[] menus = new BattleMenu[7];
	public override void _Ready(){
		currentMenu = (BattleMenu) GetNode("Top Menu");
		parentBattle = (Battle) GetNode("../..");    
		menus[0] = (BattleMenu) GetNode("Top Menu");
		menus[1] = (BattleMenu) GetNode("Party Menu");
		menus[2] = (BattleMenu) GetNode("Item Menu");
		menus[3] = (BattleMenu) GetNode("Attack Menu");
		menus[4] = (BattleMenu) GetNode("Skill Menu");
		menus[5] = (BattleMenu) GetNode("Targeting Menu");
		menus[6] = (BattleMenu) GetNode("Swap Menu");
		playerCharacterReadouts = GetNode<ReadoutContainer>("Readouts");
	}

	public override void _Process(double delta)
	{
		if(active){
			base._Process(delta);
			var returnedAbility = currentMenu.HandleInput(ReadInput(), playersInQuestion[abilitiesQueued.Count(x => x != null)], parentBattle);
			if(returnedAbility != null){
				abilitiesQueued[abilitiesQueued.Count(x => x != null)] = returnedAbility.ReadyOnCombatantAndGetData();
				if(abilitiesQueued.Count(x => x != null) == playersInQuestion.Count){//When we have all of our abilities, we emit the signal
					EmitSignal(nameof(PlayerFinishedCommandInputEventHandler));
					this.active = false;
				}
			}
		}
	}


	//Due to Godot Signals being kind of screwy with emi
	public CombatEventData[] PickUpQueuedActions(){
		var output = abilitiesQueued;
		abilitiesQueued = null;
		return output;
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
	public bool ResetGUIStateAndStart(PlayerCombatant[] characters, Battle caller){
		abilitiesQueued = new CombatEventData[3];
		playersInQuestion = new Godot.Collections.Array<PlayerCombatant>(characters);
		currentMenu.Visible = false;
		lastMenu = currentMenu;
		currentMenu = (BattleMenu) GetNode("Top Menu");
		if(playersInQuestion[0].IsAbleToAct()){
			playersInQuestion[0].SelectMe();
		}else if(playersInQuestion.Count > 1 && playersInQuestion[1].IsAbleToAct()){
			playersInQuestion[1].SelectMe();
			playersInQuestion.Remove(playersInQuestion[0]);
		}else if (playersInQuestion.Count > 2 && playersInQuestion[2].IsAbleToAct()){
			playersInQuestion[2].SelectMe();
			playersInQuestion.Remove(playersInQuestion[0]);
			playersInQuestion.Remove(playersInQuestion[1]);
		}else{
			return false;
		}
		currentMenu.OnOpen(playersInQuestion[abilitiesQueued.Count(x => x != null)], caller);
		ShowGUI();
		active = true;
		return true;
	}

	//Changes menu. Pass -1 into newMenuIndex to goto the last menu
	public void ChangeMenu(int newMenuIndex, PlayerCombatant character, Battle caller){
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

	public void ExitWithoutQueueingAbility(PlayerCombatant character){
		//var temp = playersInQuestion.ToList();
		//temp.RemoveAt(character);
		//playersInQuestion = temp.ToArray();
		noAbilityExit = true;
	}

	public void GotoPreviousCharacter(Battle caller){
		if(abilitiesQueued.Count(x => x != null) > 0){
			playersInQuestion[abilitiesQueued.Count(x => x != null)].UnselectMe();
			var resetQueue = abilitiesQueued.ToList<CombatEventData>();
			resetQueue.Remove(resetQueue.Last<CombatEventData>());
			abilitiesQueued = resetQueue.ToArray();
			
			playersInQuestion[abilitiesQueued.Count(x => x != null)].SelectMe();
			ChangeMenu(0, playersInQuestion[abilitiesQueued.Count(x => x != null)], caller);
		}
	}

	public MenuInput ReadInput(){
		if(Input.IsActionJustPressed("ui_back")){ return MenuInput.Back; }
		if(Input.IsActionJustPressed("ui_proceed")){ return MenuInput.Select; }
		if(Input.IsActionJustPressed("ui_up")){ return MenuInput.Up; }
		if(Input.IsActionJustPressed("ui_right")){ return MenuInput.Right; }
		if(Input.IsActionJustPressed("ui_down")){ return MenuInput.Down; }
		if(Input.IsActionJustPressed("ui_left")){ return MenuInput.Left; }
		return MenuInput.None;
	}    
}



	/* Old "Core Loop"
	//Returns the finished Queue when complete
	public Queue<PlayerAbility> Execute(MenuInput input, Battle caller){
			
			This function used the statement playersInQuestion[abilitiesQueued.Count] to refer to the current character
			this is due to the fact that the number of abilites we've already queued is always the index of the next character who hasn't
			specified what ability they want to use, or equals the length of playersInQuestion when we've gotten every characters action recorded
			
		
		var temp = currentMenu.HandleInput(input, playersInQuestion[abilitiesQueued.Count], caller);
		if(temp != null){ 
			playersInQuestion[abilitiesQueued.Count].UnselectMe(); //The previous character should exit their excited idle/readout highlight
			abilitiesQueued.Enqueue(temp);
			
			if(abilitiesQueued.Count == playersInQuestion.Count) return abilitiesQueued; //If all players have set an ability, we go to the next step in the battle

			if(playersInQuestion[abilitiesQueued.Count].IsAbleToAct()){
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
	*/
