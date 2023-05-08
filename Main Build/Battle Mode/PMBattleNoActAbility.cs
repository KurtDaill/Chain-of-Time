using Godot;
using System;

//Used for when the character cannot act and must use a notification to show why it can't act
public partial class PMBattleNoActAbility : PMBattleAbility
{
    [Export]
    StatusNotification abilitySelectionDefault;

    StatusNotification queuedNotification;

    public override void _Ready(){
        if(queuedNotification == null) queuedNotification = abilitySelectionDefault;
        base._Ready();
    }
    public void QueueNotification(StatusNotification notification){
        queuedNotification = notification;
    }

    public void SendNotification(){
        queuedNotification.PlayNotification(source.GetBodyRegion(PMCharacterUtilities.BodyRegions.Head));
    }

    public void ResetState(){
        queuedNotification = abilitySelectionDefault;
    }
}
