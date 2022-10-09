using Godot;
using System;
using System.Collections.Generic;
using static BattleEnemyAI;
using static PMBattleUtilities;

public class PMEnemyCharacter : PMCharacter{
    [Export]
    protected NodePath[] abilitiesByPriority;
    protected PMEnemyAbility[] abilities;

    public override void _Ready(){
        base._Ready();
        abilities = new PMEnemyAbility[abilitiesByPriority.Length];
        for(int i = 0; i < abilitiesByPriority.Length; i++){
            abilities[i] = GetNode<PMEnemyAbility>(abilitiesByPriority[i]);
        }
    }
    public PMEnemyAbility DecideAttack(){
        return null; //Remove when Done
        PMEnemyAbility chosenAttack;
        for(int i = 0; i < abilities.Length; i++){

            //Checks for Special Conditions, going by kinds of conditions the list could have, and continuing to the next ability if any of them aren't met
            //TODO Test that the requirements work
            List<SpecialRequirement> req = abilities[i].GetRequirements();
            if(req.Count != 0){ //Does this one have any special conditions
                if(req.Contains(SpecialRequirement.NoHeroTank)){
                    if(parentBattle.heroTauntUp){
                        continue;
                    }
                }
                if(req.Contains(SpecialRequirement.HeroDown)){
                    bool isSomeoneDead = false;
                    if(parentBattle.TargetLookup(Targeting.HeroThree) != null){ //Does Hero 3 exist?
                        if(parentBattle.TargetLookup(Targeting.HeroThree).GetHP() == 0){ //Are they dead?
                            isSomeoneDead = true;
                        }
                    }
                    if(parentBattle.TargetLookup(Targeting.HeroTwo) != null){ //Does Hero 2 exist?
                        if(parentBattle.TargetLookup(Targeting.HeroTwo).GetHP()!= 0){ //Are they dead?
                            isSomeoneDead = true;
                        }
                    }
                    //The battle can't still be happening if Hero 1 is dead, so we assume they aren't
                    if(!isSomeoneDead) continue;
                }
                if(req.Contains(SpecialRequirement.SelfDamagedThisTurn)){
                    if(this.damageTakenThisTurn == 0) continue;
                    
                }if(req.Contains(SpecialRequirement.SelfUndamagedThisTurn)){
                       if(this.damageTakenThisTurn !=0 ) continue;
                }
                if(req.Contains(SpecialRequirement.SelfHurt)){
                    if(!IsHurt()) continue;
                }
                if(req.Contains(SpecialRequirement.SelfBloodied)){
                    if(!IsBloodied()) continue;
                }
                if(req.Contains(SpecialRequirement.EnemyBloodied) || req.Contains(SpecialRequirement.EnemyHurt)){
                    bool enemyBloodied = false;
                    bool enemyHurt = false;
                    PMCharacter ch;
                    for(uint j = 0b_000100; j != 0b_000000; j = j << 1){
                        ch = parentBattle.TargetLookup((Targeting)j); //using the bitwise flags in Targeting (See ParentBattle)
                        if(ch != null && !System.Object.ReferenceEquals(ch, this)){ //If Character exists and isn't me
                            if(ch.IsHurt()) enemyHurt = true;
                            if(ch.IsBloodied()) enemyBloodied = true;
                        }
                    }
                    if(req.Contains(SpecialRequirement.EnemyHurt)) if(!enemyHurt) continue;
                    if(req.Contains(SpecialRequirement.EnemyBloodied)) if(!enemyBloodied) continue;
                }
                if(req.Contains(SpecialRequirement.NoEnemies) || req.Contains(SpecialRequirement.ThreeEnemies)){
                    bool noEnemy = true;
                    bool threeEnemy = true;
                    for(uint j = 0b_000100; j != 0b_000000; j = j << 1){
                        if(parentBattle.TargetLookup((Targeting)j) == null){
                            threeEnemy = false;
                        }else{
                            noEnemy = false;
                        }
                    }
                    if(req.Contains(SpecialRequirement.NoEnemies)) if(!noEnemy) continue;
                    if(req.Contains(SpecialRequirement.ThreeEnemies)) if(!threeEnemy) continue;
                }
            }
            //Check if there are Legal Targets
            if(((uint)parentBattle.GetLegalTargets() & 0b_111000) == 0b_000000){
                TargetingRule legal = abilities[i].GetTargetingRule();
                //if(legal == Tar)
            }
            /*
                Check for Taunt - Only Melee Hero Is Legal
                Check for Invisible, PhasedOut, or missing characters: They're not legal targets
            */
            //Picks Optimal Target
        }
    }
} 