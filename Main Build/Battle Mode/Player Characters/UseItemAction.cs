using System;
using Godot;

public partial class UseItemAction : PlayerAbility
{
    private ConsumableItem itemBeingUsed;

    public override void _EnterTree()
    {
        base._EnterTree();
        //Flag 0 is for animation completion as per usual, the second flag is for the item's logic to finish running.
        flagsRequiredToComplete = new bool[]{false, false};
    }

    public override void Begin(){
        base.Begin();
        PlayCoreAnimation();
    }

    public override async void AnimationTrigger(int phase)
    {
        if(phase != 0 ) throw new Exception("Item animatinons can only have 1 trigger on phase 0!"); 
        await itemBeingUsed.Consume((PlayerCombatant)source, target);
        itemBeingUsed.QueueFree();
        flagsRequiredToComplete[1] = true;
    }

    public void SetItemAndDetails(ConsumableItem item){
        itemBeingUsed = item;
        name = item.GetDisplayName();
        animation = item.GetAnimation();
    }
}