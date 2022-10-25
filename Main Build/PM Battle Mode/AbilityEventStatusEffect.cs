using System;
using Godot;
using static PMBattleUtilities;

public class AbilityEventStatusEffect : AbilityEvent {
    [Export(PropertyHint.File)]
    public string coreStatusEffect = "<Insert Resource Path>";

    [Export]
    private int setCustomDuration = -1;
    [Export]
    private int setCustomMagnitude = -1;    
    //TODO Load from _Ready()
    protected StatusEffect statusEffectType;
    private PackedScene statusEffectPS;

    public override void _Ready(){
        statusEffectPS = GD.Load<PackedScene>(coreStatusEffect);
        eventType = EventType.Status;
    }

    public PackedScene GetStatusEffectPS(){
        return statusEffectPS;
    }

    public PMStatus InstanceStatusEffect(PMCharacter target){
        var status = statusEffectPS.Instance<PMStatus>();
        status.SetCustom(setCustomDuration, setCustomMagnitude);
        status.Setup(target);
        return status;
    }
}