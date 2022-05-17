using Godot;
using System;

public class CatoCombatant : PlayerCombatant
{
    //How many frames does the player have to wait between their first and second attack
    [Export]
    public int secondAttackTimer = 10;
    //At what frame does the player have to throw their second attack at in order to get extra damage (a "Critical")
    [Export]
    public int secondAttackCriticalCutoff = 15;
    public override bool MoveAndAttack(EnemyCombatant[] targets, int[] damageRecord)
    {
            if(state is CombatantStateExit) return true;
            Move();
            if(state is PlayerCombatantStateGround && Input.IsActionJustPressed("com_atk")){
                state.Exit();
                CatoStateAttackOne attackState = new CatoStateAttackOne(damageRecord, targets);
                CombatantState temp = state;
                state = attackState;
                state.Enter(this, temp);
            }
            return false;
        
        /*
            case attackStatus.PreAttack :
                Move();
                if(Input.IsActionJustPressed("com_attack")){
                    //atkStatus = attackStatus.ComboOne
                    //animation = attack one
                    //comboTimer = 0;
                }
                break;
            case attackStatus.ComboOne :
                    if(attackLocked){ //Player hit the key too early...
                        //Wait until the animation is done, then return true;
                        return false;
                    }
                    if(Input.IsActionJustPressed("com_attack") && comboTimer != 0){
                        if(comboTimer < secondAttackTimer){               //Player hit key too early...
                            attackLocked = true;
                        }else if(comboTimer < secondAttackCriticalCutoff){//Player hit key in time for a crit
                            atkStatus = attackStatus.ComboTwoCritical;
                        }else{                                           //Player hit key in time, normal hit
                            atkStatus = attackStatus.ComboTwo;
                        }
                    }
                    /*
                        if(animation == done){

                        }
                    
                    comboTimer ++;
                break;

            case attackStatus.ComboTwo :
                //If Attack 2 animation is done : return true;
                break;
            case attackStatus.ComboTwoCritical :
                break;
        }
        */
    }
}
