using Godot;
using System;
using System.Collections.Generic;

//Class used to represent and run turn based battles\
public class Battle : Node
{
    //Tracks all active combatants in the scene
    [Export]
    public Combatant[] activeCombatants = new Combatant[6];
    //Tracks Enemies that may enter in waves as active enemies are defeated;
    public List<Combatant> enemyBench = new List<Combatant>();
    private List<BattleCommand> commandList;
    private int currentCommandIndex;
    public override void _Ready()
    {
        commandList = new List<BattleCommand>();
        activeCombatants[0] = (DodgePlayer)GetChild(0);
        commandList.Add(new EnemyAttacks());
        commandList[currentCommandIndex].Enter(this);   
    }
    public override void _Process(float delta)
    {
        commandList[currentCommandIndex].Execute();
    }
    private void NextCommand(){//Called by commands when they're completed
        commandList[currentCommandIndex].Exit();
        if((currentCommandIndex - 1) == commandList.Count){ //Checks if current Index is the last entry, if so...
            //goto the 'default' command: PlayerActMenu
            throw new NotImplementedException();
        }
        currentCommandIndex += 1;
        commandList[currentCommandIndex].Enter(this);
    }
    public void AddCommand(BattleCommand newCom){
        commandList.Add(newCom);
    }
    public BattleCommand PeakCommand(){
        return commandList[currentCommandIndex + 1];
    }
    public BattleCommand CurrentCommand(){
        return commandList[currentCommandIndex + 1];
    }
}
