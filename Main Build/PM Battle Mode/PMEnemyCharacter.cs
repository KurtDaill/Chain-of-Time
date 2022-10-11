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
                    foreach(PMPlayerCharacter pc in parentBattle.GetPlayerCharacters()){
                        if(pc.GetHP() <= 0){
                            isSomeoneDead = true;
                        }
                    }
                    if(!isSomeoneDead) continue;
                }
                if(req.Contains(SpecialRequirement.SelfDamagedThisTurn)){
                    if(this.damageTakenThisTurn == 0) continue;
                    
                }
                if(req.Contains(SpecialRequirement.SelfUndamagedThisTurn)){
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
                    foreach(PMEnemyCharacter enemy in parentBattle.GetEnemyCharacters()){
                        if(System.Object.ReferenceEquals(this, enemy)) continue;  //If the character is me, I don't care. Remember that this continue only effects this inner for loop.
                        if(enemy.IsHurt()) enemyHurt = true;
                        if(enemy.IsBloodied()) enemyBloodied = true;
                    }
                    if(req.Contains(SpecialRequirement.EnemyHurt)) if(!enemyHurt) continue;
                    if(req.Contains(SpecialRequirement.EnemyBloodied)) if(!enemyBloodied) continue;
                }
                if(req.Contains(SpecialRequirement.NoEnemies) || req.Contains(SpecialRequirement.ThreeEnemies)){
                    int count = parentBattle.GetEnemyCharacters().Length;
                    if(req.Contains(SpecialRequirement.NoEnemies)) if(count > 1) continue; //If there's only 1 enemy (me), we fail the "no other enemies" condition
                    if(req.Contains(SpecialRequirement.ThreeEnemies)) if(count != 3) continue; //If there's not 3 enemies total, we fail the 3 enemies condition
                }
            }
            //Picks Optimal Target(s)
            //Check if any of those targets are legal, if not pick again
            /*
                Pick the first target that matches the rule and preference
                If it's Melee/Ranged/Reach/HeroOne/HeroTwo/HeroThree...
                    Check if they're invsible or phased out, if so it's not legal
                Otherwise
                    Hey, is this attack ground only? If so check flying the same way as phased out next.
                    Check if they're phased out
                    Enemies will never execute an attack if all affected characters are phased out
                
            */
        }
    }
} 