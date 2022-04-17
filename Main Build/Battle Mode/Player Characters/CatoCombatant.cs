using Godot;
using System;

public class CatoCombatant : PlayerCombatant
{
    private int comboTimer = 0;
    private bool attackLocked;

    //How many frames does the player have to wait between their first and second attack
    [Export]
    public int secondAttackTimer;

    //At what frame does the player have to throw their second attack at in order to get extra damage (a "Critical")
    [Export]
    public int secondAttackCriticalCutoff;
    public override bool MoveAndAttack(EnemyCombatant[] targets, int[] damageRecord)
    {
            Move();
            if(state is PlayerCombatantStateGround && Input.IsActionJustPressed("com_atk")){
                CatoStateAttackOne attackState = new CatoStateAttackOne();
                attackState.acceptAttackData(damageRecord, targets);
                PlayerCombatantState temp = state;
                state = attackState;
                state.Enter(this, temp);
            }
            if(state is PlayerCombatantStateExit) return true;
            else return false;
        
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
