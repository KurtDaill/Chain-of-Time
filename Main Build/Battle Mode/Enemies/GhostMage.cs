using Godot;
using System;
using static BattleUtilities;
public partial class GhostMage : EnemyCombatant
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	private EnemyAbility attack;
	[Export]
	private EnemyAbility confusion;
	[Export]
	private EnemyAbility fireBlast;

	bool hasConfused = false;

	public override void _Ready()
	{
        name = "Ghost Mage";
        base._Ready();
		attack.Setup(this);
		confusion.Setup(this);
		fireBlast.Setup(this);
	}

	public override CombatEventData DecideAction(Battle parentBattle){
			if(parentBattle.GetRoster().GetCombatant(BattlePosition.HeroFront).GetName() == "Cato" && !hasConfused && parentBattle.GetRoster().GetAllEnemyCombatants().Length == 3){
				hasConfused = true;
				return confusion.GetEventData();
			}
			else if(this.GetPosition() == BattlePosition.EnemyFront){
				return attack.GetEventData();
			}else{
				return fireBlast.GetEventData();
			}
	}

}
