using Godot;
using System;
using System.Threading.Tasks;
using static ItemUtilities;
using static BattleUtilities;
public abstract partial class ConsumableItem : Item
{
    protected TargetingLogic targeting = TargetingLogic.Self;
    public abstract Task Consume(PlayerCombatant user, Combatant[] targets);
    protected string onCharacterAnimation;

    protected bool slatedForUse = false;
    public override void _Ready(){
        base._Ready();
        type = ItemType.Consumable;
    }
    
    public TargetingLogic GetTargetingLogic(){
        return targeting;
    }
    
    public string GetAnimation(){
        return onCharacterAnimation;
    }

    public void SetForUse(){
        slatedForUse = true;
    }

    public bool IsThisInUse(){
        return slatedForUse;
    }

    public void SetAsUnused(){
        slatedForUse = false;
    }
}