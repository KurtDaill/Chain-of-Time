using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static BattleUtilities;

public partial class TargetingMenu : BattleMenu {
	private PlayerAbility abilityInQuestion;
	private AudioStreamPlayer targetingErrorSound;
	private List<Combatant> plannedTargets;
	private TargetingLogic workingRule;
	private SkillCard skillCardGUI;
	private TextureRect attackBackboard;
	private Label attackName;
	private RichTextLabel rulesText;
	private int spRefund = 0;
	private bool decisionRequired = true;

	public override void _Ready()
	{
		base._Ready();
		targetingErrorSound = GetNode<AudioStreamPlayer>("SelectError");
		skillCardGUI = GetNode<SkillCard>("Skill");
		attackBackboard = GetNode<TextureRect>("Basic Attack");
		attackName = attackBackboard.GetNode<Label>("Attack Name");
		rulesText = attackBackboard.GetNode<RichTextLabel>("Rules Text");
		this.Visible = false;
	}

	//Because we want to avoid having to add a special case to the BattleGUI class for opening this menu, we instead
	//Have the previous menu call this command on the targeting menu for setting up the current target instead
	public void SetAbilityForTargeting(PlayerAbility newAbility){
		abilityInQuestion = newAbility;
		if(newAbility is PlayerSkill){
			PlayerSkill newSkill = (PlayerSkill) newAbility;
			if(newSkill.GetSPCost() != -1) spRefund = newSkill.GetSPCost();
		}
	}

	//Run when this menu is opened, resets values as needed from previous uses
	public override void OnOpen(PlayerCombatant character, Battle caller, BattleGUI parentGUI){
		base.OnOpen(character, caller, parentGUI);
		this.Visible = true;
		if(abilityInQuestion == null) throw new NotImplementedException();//TODO write custom targetingError Exception
		workingRule = abilityInQuestion.GetTargetingLogic();
		plannedTargets = new List<Combatant>();
		//Checks if we have to ask the player for input on who to target:
		//If not, we mark the targets and tell handle input to not worry about it, otherwise we use everything in handle input
				switch(workingRule){
					case TargetingLogic.Self :
						SetPointers(character, caller);
						plannedTargets = new List<Combatant>(){character};
						decisionRequired = false;
						break;
					case TargetingLogic.AllEnemies :
						SetNewTargets(caller.GetRoster().GetAllEnemyCombatants().ToList<Combatant>(), caller);
						decisionRequired = false;
						break;
					case TargetingLogic.AllHeroes :
						SetNewTargets(plannedTargets = caller.GetRoster().GetAllPlayerCombatants().ToList<Combatant>(), caller);
						decisionRequired = false;
						break;
					case TargetingLogic.All :
						SetNewTargets(plannedTargets = caller.GetRoster().GetAllCombatants().ToList(), caller);
						decisionRequired = false;
						break;
					case TargetingLogic.Melee : //Only legal if we're in the front slot
						if(caller.GetRoster().GetPositionOfCombatant(character) != BattlePosition.HeroFront){
							//Do the "This attack is super invalid" notification
							RejectSelection();
						}else{
							decisionRequired = false;
							SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.EnemyFront), caller);
						}
						break;
					case TargetingLogic.Reach : //Only legal if we're in the front or middle slot
						if(character.GetPosition() != BattlePosition.HeroFront && character.GetPosition() != BattlePosition.HeroMid){
							//Do the "This attack is super invalid" notification
							RejectSelection();
						}else{
							SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.EnemyFront), caller);
							decisionRequired = true;
						}
						break;
					case TargetingLogic.Ranged : //Always legal if there's an enemy targetable (we check for targetability later)
						SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.EnemyFront), caller);
						decisionRequired = true;
						break;
					case TargetingLogic.AnyAlly :
						//Logic for Abilities that target Allies
						decisionRequired = true;
						break;
					default :   //TODO write custom targetingError exception, if we've gotten here, there's a rule we aren't accounting for...
						throw new NotImplementedException();
				}
			if(abilityInQuestion is PlayerSkill){
				skillCardGUI.Visible = true;
				attackBackboard.Visible = false;
				PlayerSkill skill = (PlayerSkill)abilityInQuestion;
				if(!skill.GetEnabledPositions().Contains(character.GetPosition())){
					skillCardGUI.SetDisplay(skill.Name, "Out of Position!", skill.GetSkilType(), skill.GetAbilityAlignment(), skill.GetSPCost(), skill.GetEnabledPositions());
				}
				skillCardGUI.SetDisplay(skill.Name, skill.GetRulesText(), skill.GetSkilType(), skill.GetAbilityAlignment(), skill.GetSPCost(), skill.GetEnabledPositions());
			}else{
				skillCardGUI.Visible = false;
				attackBackboard.Visible = true;
				attackName.Text = abilityInQuestion.GetName();
				rulesText.Text = abilityInQuestion.GetRulesText();
			}
	}

	//Handles input from the core Menu Command
	//Returns a new menu in the scenario we have to switch between menus
	public override PlayerAbility HandleInput(MenuInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI){ 
		
		if(decisionRequired){
			if(input == MenuInput.Right || input == MenuInput.Left){
				switch(workingRule){
					case TargetingLogic.Melee :
						//Melee only has one potential target, therefore you can't switch 'em
						//Play Nuh-no sound effect
						break;
					case TargetingLogic.Reach :
						if(caller.GetRoster().GetCombatant(BattlePosition.EnemyMid) != null && plannedTargets[0].GetPosition() == BattlePosition.EnemyFront 
						&& input == MenuInput.Right && character.GetPosition() == BattlePosition.HeroFront){
						//You can only target the enemy in the second rank with a reach attack if you're right on the front (reach only gives you 2 slots of range)
							SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.EnemyMid), caller);
						}
						else if(plannedTargets[0].GetPosition() == BattlePosition.EnemyMid && input == MenuInput.Left){
						//We don't check if enemy one exists because they have to in order for there to still be a battle    
							SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.EnemyFront), caller);
						}
						break;
					case TargetingLogic.Ranged :
						switch(plannedTargets[0].GetPosition()){
							case BattlePosition.EnemyFront :
								if(input == MenuInput.Right && caller.GetRoster().GetCombatant(BattlePosition.EnemyMid) != null){
									SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.EnemyMid), caller);
								}else{
									RejectSelection(); //We know input == Left and there's no left target or there's no right target at all
								}
								break;
							case BattlePosition.EnemyMid :
								if(input == MenuInput.Right && caller.GetRoster().GetCombatant(BattlePosition.EnemyBack) != null){
									SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.EnemyBack), caller);
								}
								else if(input == MenuInput.Left){//We don't check if there's an enemy one because there must be for battle to be happening
									SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.EnemyFront), caller);
								}else{
									RejectSelection(); //We know input == Left and there's no left target or there's no right target at all
								}
								break;
							case BattlePosition.EnemyBack :
								if(input == MenuInput.Left){//We don't check if there's an enemy two, because there must be if we're targeting enemy three
									SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.EnemyMid), caller);
								}
								break;
						}
						break;
					case TargetingLogic.AnyAlly :
						switch(plannedTargets[0].GetPosition()){
							case BattlePosition.HeroFront :
								if(input == MenuInput.Right && caller.GetRoster().GetCombatant(BattlePosition.HeroMid) != null){
									SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.HeroMid), caller);
								}else{
									RejectSelection(); //because we know input == Left and there's no left target or there's no right target at all
								}
								break;
							case BattlePosition.HeroMid :
								if(input == MenuInput.Right && caller.GetRoster().GetCombatant(BattlePosition.HeroBack) != null){
									SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.HeroBack), caller);
								}
								else if(input == MenuInput.Left){//We don't check if there's an Hero one because there must be for battle to be happening
									SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.HeroFront), caller);
								}else{
									RejectSelection();//We know input == Left and there's no left target or there's no right target at all
								}
								break;
							case BattlePosition.HeroBack :
								if(input == MenuInput.Left){//We don't check if there's an Hero two, because there must be if we're targeting Hero three
									SetNewTargets(caller.GetRoster().GetCombatant(BattlePosition.HeroMid), caller);
								}
								break;
						}
						break;
				}
			}
		}
		
		if(input == MenuInput.Select){//Need to Double Check if that target is legal!
				if(CheckTargetLegality(plannedTargets, caller, out var realTargets)){
					abilityInQuestion.SetTargets(realTargets.ToArray<Combatant>());
					var result = abilityInQuestion;
					abilityInQuestion = null;
					HidePointers(caller);
					this.Visible = false;
					return result;
				}else{ //Our target isn't legal (Oh noes!)
					RejectSelection();
				}
		}else if(input == MenuInput.Back){
			abilityInQuestion = null;
			parentGUI.ChangeMenu(-1, character);
			character.GainSP(spRefund);
			SetPointers(new List<Combatant>(), caller); //Clears all pointers by passing an empty list
			return null;
		}
		return null;
	}

	//returns true if there are ANY valid targets
	public bool CheckTargetLegality(List<Combatant> desiredTargets, Battle battle, out List<Combatant> actualTargets){
		actualTargets = new List<Combatant>();
		if(workingRule == TargetingLogic.Self){ //You can always hit yourself
			actualTargets = desiredTargets;
			return true;
		}
		bool hitsInvisble = (workingRule == TargetingLogic.All || workingRule == TargetingLogic.AllHeroes || workingRule == TargetingLogic.AllEnemies);
		//var legaltargets = battle.GetCharacters(abilityInQuestion.CanTargetFliers(), hitsInvisble, false);
		Combatant[] legaltargets = battle.GetRoster().GetLegalEnemyTargets();
		//legaltargets.Add(battle.GetRoster().GetLegalHeroTargets());
		foreach(Combatant character in desiredTargets){
			if(legaltargets.Contains(character)){
				actualTargets.Add(character);
			}
		}
		foreach(EnemyCombatant enemy in battle.GetRoster().GetAllEnemyCombatants()){
			foreach(StatusTaunting taunt in enemy.GetStatusEffects().Where(x => x is StatusTaunting)){
				taunt.ShowNotification();
			}
		}
		if(actualTargets.Count() == 0) return false;
		return true;
	}

	public void RejectSelection(){
		targetingErrorSound.Play();
	}
	public void SetNewTargets(List<Combatant> cList, Battle battle){
		plannedTargets = cList;
		SetPointers(cList, battle);
	}
	public void SetNewTargets(Combatant ch, Battle battle){
		plannedTargets = new List<Combatant>(){ch};
		SetPointers(ch, battle);
	}
	public void SetPointers(Combatant character, Battle battle){
		foreach(Combatant ch in battle.GetRoster().GetAllCombatants()){
			ch.SetTargetGUIElements(false);
		}
		character.SetTargetGUIElements(true);
	}
	public void SetPointers(List<Combatant> characters, Battle battle){
		foreach(Combatant ch in battle.GetRoster().GetAllCombatants()){
			if(characters.Contains(ch)) ch.SetTargetGUIElements(true);
			else    ch.SetTargetGUIElements(false);
		}
	}
	public void HidePointers(Battle battle){
		var empty = new List<Combatant>();
		SetPointers(empty, battle);
	}
}

