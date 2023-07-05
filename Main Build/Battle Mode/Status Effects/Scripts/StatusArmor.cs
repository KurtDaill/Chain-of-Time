using Godot;
using System;

public partial class StatusArmor : StatusEffect{
    [Export]
    private int armorValue = 1;
    public override void _Ready(){
        name = "Armor";
        animation = null;
        remainingDuration = startingDuration;
        
        /*
            Flags:
                
        */
        flagsRequiredToComplete = new bool[1];
    } 

    public override void _Process(double delta){
        if(running){ running = false; }
    }

    public override void Setup(Combatant proposedSource){
        base.Setup(proposedSource);
        BattleNotification note = ShowNotification();
        note.GetNode<Label3D>("Shield Sprite/Label3D").Text = "" + armorValue;
    }

    public int GetArmorValue(){
        return armorValue;
    }
}