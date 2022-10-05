using Godot;
using System;
using System.Collections.Generic;
using static PMBattleUtilities;

public class PMCharacter : Node{

    public List<PMStatus> statusEffects;
    public PMBattle parentBattle;

    private int HP;

    [Export(PropertyHint.Enum)]
    public Dictionary<AbilityAlignment, float> DamageModifiers = new Dictionary<AbilityAlignment, float>();
    public override void _Ready(){
        statusEffects = new List<PMStatus>();
        parentBattle = (PMBattle) GetNode("/root/Battle");
        
    } 

    public override void _Process(float delta){
        
    }

    public int GetHP(){
        return HP;
    }

    public void AddStatus(PMStatus newEffect){
        //Checks if the newEffect is another instance of a current effect, if so we keep the instance with more duration
        foreach(PMStatus oldEffect in statusEffects){
            if(newEffect.GetEventType() == oldEffect.GetEventType()){
                if(newEffect.GetDuration() > oldEffect.GetDuration()){
                    statusEffects.Remove(oldEffect);
                    statusEffects.Add(newEffect);
                    return;
                }else{
                    return;
                }
            }
        }

        statusEffects.Add(newEffect);
    }

    public void GetAbility(string name){

    }

    public void TakeDamage(int damage, AbilityAlignment alignment){
        foreach(KeyValuePair<AbilityAlignment, float> mod in DamageModifiers){
            damage = Mathf.RoundToInt(damage * mod.Value);
        }
    }
}
