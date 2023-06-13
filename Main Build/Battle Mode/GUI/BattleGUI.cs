using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static BattleMenu;

public partial class BattleGUI : Control
{

	[Signal]
	public delegate void PlayerFinishedCommandInputEventHandler();
	private BattleMenu currentMenu;
	private BattleMenu lastMenu;
	private Battle parentBattle;

	private PlayerCombatant[] playersInQuestion;
	private CombatEventData[] abilitiesQueued;
	private ActionChain chainGUI;

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
		chainGUI = (ActionChain) GetNode("Action Chain");
	}

	public override void _Process(double delta)
	{
		if(active){
			base._Process(delta);
			if(!playersInQuestion[abilitiesQueued.Count(x => x != null)].IsAbleToAct()){
				GoToNextCharacter();
				return;
			}
			var returnedAbility = currentMenu.HandleInput(ReadInput(), playersInQuestion[abilitiesQueued.Count(x => x != null)], parentBattle, this);
			if(returnedAbility != null){
				abilitiesQueued[abilitiesQueued.Count(x => x != null)] = returnedAbility.GetEventData();
				chainGUI.LogAbility(returnedAbility.GetName(), abilitiesQueued.Count(x => x != null) == playersInQuestion.Length);
				GoToNextCharacter();
			}
		}
	}


	//Due to Godot Signals being kind of screwy with emiting with a return value, we just have battle pick up the queued actions manually
	public CombatEventData[] PickUpQueuedActions(){
		var output = abilitiesQueued;
		abilitiesQueued = null;
		return output.Where(x => x != null).ToArray();
	}

	public void ShowGUI(){
		currentMenu.Visible = true;
		playerCharacterReadouts.Visible = true;
		chainGUI.Visible = true;
	}

	public void HideGUI(bool keepReadouts = true){
		currentMenu.Visible = false;
		if(!keepReadouts) playerCharacterReadouts.Visible = false;
		chainGUI.Visible = false; //TODO: Actually have the Gain GUI stay as long as it's supposed to.
	}
	
	//returns true if we have any characters able to act, false otherwise
	//This needs an overhaul
	public bool ResetGUIStateAndStart(PlayerCombatant[] characters){
		abilitiesQueued = new CombatEventData[3];
		playersInQuestion = characters;
		if(playersInQuestion.Contains(null)) throw new ArgumentException("Cannot Sent a PlayerCombatant[] with null entries swhen Reseting GUI state!");
		currentMenu.Visible = false;
		lastMenu = currentMenu;
		currentMenu = (BattleMenu) GetNode("Top Menu");
		currentMenu.OnOpen(playersInQuestion[abilitiesQueued.Count(x => x != null)], parentBattle, this);
		chainGUI.ResetActionChain(playersInQuestion);
		ShowGUI();
		active = true;
		return true;
	}

	//Changes menu. Pass -1 into newMenuIndex to goto the last menu
	public void ChangeMenu(int newMenuIndex, PlayerCombatant character){
		currentMenu.Visible = false;
		if(newMenuIndex == -1){
			var temp = currentMenu;
			currentMenu = lastMenu;
			lastMenu = temp;
		}else{
			lastMenu = currentMenu;
			currentMenu = menus[newMenuIndex];
		}
		currentMenu.OnOpen(character, parentBattle, this);
	}

	/*
	public void ExitWithoutQueueingAbility(PlayerCombatant character){
		//var temp = playersInQuestion.ToList();
		//temp.RemoveAt(character);
		//playersInQuestion = temp.ToArray();
		//noAbilityExit = true;
	}
	*/
	public void GotoPreviousCharacter(){
		if(abilitiesQueued.Count(x => x != null) > 0){
			playersInQuestion[abilitiesQueued.Count(x => x != null)].UnselectMe();
			abilitiesQueued[abilitiesQueued.Count(x => x != null) - 1] = null;
			//var resetQueue = abilitiesQueued.ToList<CombatEventData>().Where(x => x != null).ToList();
			//resetQueue.Remove(resetQueue.Last<CombatEventData>());
			//abilitiesQueued = resetQueue.ToArray();
			
			playersInQuestion[abilitiesQueued.Count(x => x != null)].SelectMe();
			chainGUI.StepBack();
			ChangeMenu(0, playersInQuestion[abilitiesQueued.Count(x => x != null)]);
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

	public void GoToNextCharacter(){
		//If Next player isn't able to act, fill in a null in abilities queue then repeat this logic.
		while(abilitiesQueued.Count(x => x != null) < playersInQuestion.Length){
			if(playersInQuestion[abilitiesQueued.Count(x => x != null)].IsAbleToAct()){
				ChangeMenu(0, playersInQuestion[abilitiesQueued.Count(x => x != null)]);
				return; 
			}else{
				abilitiesQueued[abilitiesQueued.Count(x => x != null)] = new CombatEventData("NoAction", playersInQuestion[abilitiesQueued.Count(x => x != null)], null);
				chainGUI.LogAbility("Can't Act", abilitiesQueued.Count(x => x != null) == playersInQuestion.Length);
			}
		}
		//If we've reached this block of code, we have CED for every player, and can send it all back.
		EmitSignal(BattleGUI.SignalName.PlayerFinishedCommandInput);
		this.active = false;
		HideGUI();
	}   
}
