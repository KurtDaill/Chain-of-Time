using Godot;
using System;
using System.Collections.Generic;

public class PMCharacter : Node{

    public List<PMStatus> statusEffects;
    public override void _Ready(){
        statusEffects = new List<PMStatus>();
    } 

    public override void _Process(float delta){
        
    }

    public void AddStatus(PMStatus newEffect){
        //Checks if the newEffect is another instance of a current effect, if so we keep the instance with more duration
        foreach(PMStatus oldEffect in statusEffects){
            if(newEffect.GetEffectType() == oldEffect.GetEffectType()){
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
}
