using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static BattleEnemyAI;
using static PMBattleUtilities;

public class PMEnemyCharacter : PMCharacter{
    [Export]
    protected List<NodePath> abilitiesByPriority;
    protected PMEnemyAbility[] abilities;

    public bool chargedUp = false;

    [Export]
    protected List<EnemyRole> combatRoles = new List<EnemyRole>{EnemyRole.Minion};

    public override void _Ready(){
        base._Ready();
        abilities = new PMEnemyAbility[abilitiesByPriority.Count];
        for(int i = 0; i < abilitiesByPriority.Count; i++){
            abilities[i] = GetNode<PMEnemyAbility>(abilitiesByPriority[i]);
        }
    }

    public List<EnemyRole> GetRoles(){
        return combatRoles;
    }

    public void ChargeUp(){
        chargedUp = false;
    }
    public PMEnemyAbility DecideAttack(){  //TODO use dependancy injection to make this shit suck less
        PMEnemyAbility chosenAbility;
        PMCharacter[] targets = Array.Empty<PMCharacter>();
        for(int i = 0; i < abilities.Length; i++){
            if(CheckAbilitySpecialRequirements(abilities[i])){ //If this attacks requirements are met...
                switch(abilities[i].GetTargetingRule()){ //We go to selecting a target. TargetRules of "All Enemy","All Hero", and "All" present special cases where we don't have to pick targets
                    case TargetingRule.AllEnemy:
                        targets = parentBattle.GetEnemyCharacters(true, true, false).ToArray<PMCharacter>();
                        break;
                    case TargetingRule.AllHero:
                        targets = parentBattle.GetPlayerCharacters(true, true, false).ToArray<PMCharacter>();
                        break;
                    case TargetingRule.All:
                        targets = parentBattle.GetCharacters(true, true, false).ToArray<PMCharacter>();
                        break;
                    default: //In any other case: we go on to pick targets using the normal GetTargetByPriority function
                        targets = GetTargetByPriority(abilities[i]);
                        break;
                }
            }
            if(targets == Array.Empty<PMCharacter>()) continue; //If we failed the requirements or couldn't pick a target, we go to the next ability
            else{
                chosenAbility = abilities[i];
                chosenAbility.SetTargets(targets);
                return chosenAbility;
            }   
        }
        return null; //We reach this if no usable ability could be found...
        //TODO add some kind of default "I can't do anything" ability to singal to the player that this enemy's got nothing?
    }

        //TODO make some kind of "I don't have anything" default attack to play a little animation that makes it obvious to the player somethings up
    public bool CheckAbilitySpecialRequirements(PMEnemyAbility able){//TODO Test that the requirements work
        List<SpecialRequirement> req = able.GetRequirements();
            if(req.Count != 0){ //Does this one have any special conditions
                if(req.Contains(SpecialRequirement.NoHeroTank)){
                    if(parentBattle.heroTauntUp){
                        return false;
                    }
                }
                if(req.Contains(SpecialRequirement.HeroDown)){
                    bool isSomeoneDead = false;
                    foreach(PMPlayerCharacter pc in parentBattle.GetPlayerCharacters()){
                        if(pc.GetHP() <= 0){
                            isSomeoneDead = true;
                        }
                    }
                    if(!isSomeoneDead) return false;
                }
                if(req.Contains(SpecialRequirement.SelfDamagedThisTurn)){
                    if(this.damageTakenThisTurn == 0) return false;
                    
                }
                if(req.Contains(SpecialRequirement.SelfUndamagedThisTurn)){
                       if(this.damageTakenThisTurn !=0 ) return false;
                }
                if(req.Contains(SpecialRequirement.SelfHurt)){
                    if(!IsHurt()) return false;
                }
                if(req.Contains(SpecialRequirement.SelfBloodied)){
                    if(!IsBloodied()) return false;
                }
                if(req.Contains(SpecialRequirement.EnemyBloodied) || req.Contains(SpecialRequirement.EnemyHurt)){
                    bool enemyBloodied = false;
                    bool enemyHurt = false;
                    foreach(PMEnemyCharacter enemy in parentBattle.GetEnemyCharacters()){
                        if(System.Object.ReferenceEquals(this, enemy)) continue;  //If the character is me, I don't care.
                        if(enemy.IsHurt()) enemyHurt = true;
                        if(enemy.IsBloodied()) enemyBloodied = true;
                    }
                    if(req.Contains(SpecialRequirement.EnemyHurt)) if(!enemyHurt) return false;
                    if(req.Contains(SpecialRequirement.EnemyBloodied)) if(!enemyBloodied) return false;
                }
                if(req.Contains(SpecialRequirement.NoEnemies) || req.Contains(SpecialRequirement.ThreeEnemies)){
                    int count = parentBattle.GetEnemyCharacters().Length;
                    if(req.Contains(SpecialRequirement.NoEnemies)) if(count > 1) return false; //If there's only 1 enemy (me), we fail the "no other enemies" condition
                    if(req.Contains(SpecialRequirement.ThreeEnemies)) if(count != 3) return false; //If there's not 3 enemies total, we fail the 3 enemies condition
                }
                if(req.Contains(SpecialRequirement.Charged)){
                    if(!chargedUp) return false;
                }
            }
        return true;
    }
    public PMCharacter[] GetTargetByPriority(PMEnemyAbility able){
        foreach(TargetPriority prio in able.GetTargetingPriorities()){
            switch(prio){
                case TargetPriority.Self:
                    return new PMCharacter[]{this};
                case TargetPriority.MeleeHero:
                    //We don't have to check whether HeroOne is null, someone has to hold that slot for the battle to be happening
                    if(myPosition == BattlePos.EnemyOne && parentBattle.PositionLookup(BattlePos.HeroOne).IsTargetable(able.CanTargetFliers())){
                        return new PMCharacter[]{parentBattle.PositionLookup(BattlePos.HeroOne)};
                    }
                    break;
                case TargetPriority.RangedHeroes:
                    if(able.GetTargetingRule() != TargetingRule.SingleHeroRanged) throw new NotImplementedException(); //TODO make custom exception
                    //Randomly pick a hero in slot 2 or 3
                    var temp = new Random().Next(1,3);
                    PMCharacter[] players = parentBattle.GetPlayerCharacters();
                    if(players[temp] != null){
                        return new PMCharacter[]{players[temp]};
                    }else if(players[3-temp] != null){
                        return new PMCharacter[]{players[3 - temp]};
                    }
                    break;
                case TargetPriority.MeleeEnemy:
                    if(parentBattle.PositionLookup(BattlePos.EnemyOne).IsTargetable(able.CanTargetFliers())){
                        return new PMCharacter[]{parentBattle.PositionLookup(BattlePos.EnemyOne)};
                    }
                    break;
                case TargetPriority.RangedEnemies:
                    if(able.GetTargetingRule() != TargetingRule.SingleHeroRanged) throw new NotImplementedException(); //TODO make custom exception
                    //Randomly pick a hero in slot 2 or 3
                    var rando = new Random().Next(1,3);
                    PMCharacter[] pcs = parentBattle.GetEnemyCharacters();
                    if(pcs[rando] != null){
                        return new PMCharacter[]{pcs[rando]};
                    }else if(pcs[3 - rando] != null){
                        return new PMCharacter[]{pcs[3 - rando]};
                    }
                    break;
                case TargetPriority.EnemyBoss | TargetPriority.EnemyMinion | TargetPriority.EnemyArtillery | TargetPriority.EnemySquadLeader | TargetPriority.EnemyBruiser | TargetPriority.EnemyTank:
                    Random rand = new Random();
                    PMEnemyCharacter[] randomizedPMCharacters = parentBattle.GetEnemyCharacters().OrderBy(x => rand.Next()).ToArray<PMEnemyCharacter>();
                    foreach(PMEnemyCharacter en in randomizedPMCharacters){
                        if(en.GetRoles().Contains((PMBattleUtilities.EnemyRole) prio)){
                            return new PMCharacter[]{en};
                        }
                    }
                    break;
            }
        }
        return Array.Empty<PMCharacter>(); //If nothing meets the targeting priorities, we send back null
    }

    public override void FinishDefeat()
    {
        base.FinishDefeat();
        //this.QueueFree();
    }
}    