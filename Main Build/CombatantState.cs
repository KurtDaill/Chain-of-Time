using Godot;
using System;

public abstract class CombatantState : BattleState
{
    /*
    The Combatant State is specificly used for combatants, and defines the combatant it's originating from so that member methods can easily access it;
    */
   protected Combatant self;

   public CombatantState(Combatant myself){
       self = myself;
   }

   public CombatantState(){
       
   }
}
