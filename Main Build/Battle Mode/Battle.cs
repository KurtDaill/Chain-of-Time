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

    public Spatial[] battleSpots = new Spatial[6];

    [Export]
    public NodePath GUI;

    public BattleGUI gui;
    private List<BattleCommand> commandList;
    private int currentCommandIndex;
    public override void _Ready()
    {
        gui = (BattleGUI) GetNode(GUI);
        commandList = new List<BattleCommand>();
        activeCombatants[0] = (PlayerCombatant)GetNode("BattlePlayer");
        activeCombatants[3] = (EnemyCombatant)GetNode("Polymorphor");
        //battleSpots[0] = (Node3D) GetNode("PositionsMap/Hero1");
        //battleSpots[1] = (Node3D) GetNode("PositionsMap/Hero2");
        //battleSpots[2] = (Node3D) GetNode("PositionsMap/Hero3");
        //battleSpots[3] = (Node3D) GetNode("PositionsMap/Enemy1");
        //battleSpots[4] = (Node3D) GetNode("PositionsMap/Enemy2");
        //battleSpots[5] = (Node3D) GetNode("PositionsMap/Enemy3");

        commandList.Add(new PlayerMenuSelection());
        commandList[currentCommandIndex].Enter(this);   
    }
    public override void _Process(float delta)
    {
        commandList[currentCommandIndex].Execute();
    }

    public void NextCommand(){//Called by commands when they're completed
        commandList[currentCommandIndex].Exit();
        if((currentCommandIndex + 1) == commandList.Count){ //Checks if current Index is the last entry, if so...
            //add something so the next line goto the 'default' command: PlayerMenuSelection
            AddCommand(new PlayerMenuSelection());
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
