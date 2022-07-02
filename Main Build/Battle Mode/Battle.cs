using Godot;
using System;
using System.Collections.Generic;

//Class used to represent and run turn based battles\
public class Battle : Node
{
    //Tracks all active combatants in the scene
    public Combatant[] activeCombatants = new Combatant[6];
    //Tracks Enemies that may enter in waves as active enemies are defeated;
    public List<Combatant> enemyBench = new List<Combatant>();

    [Export]
    public NodePath debugPlayer;

    [Export]
    public NodePath cameraPath;

    public Camera camera;

    [Export]
    public NodePath debugEnemy;
    public Spatial[] battleSpots = new Spatial[6];

    public List<BattleEffect> startOfRoundEffects = new List<BattleEffect>();
    public List<BattleEffect> endOfRoundEffects = new List<BattleEffect>();

    [Export]
    public NodePath GUI;

    public BattleGUI gui;
    private List<BattleCommand> commandList;
    private int currentCommandIndex;
    public override void _Ready()
    {
        if(debugPlayer != null){
            activeCombatants[0] = (Combatant) GetNode(debugPlayer);
        }
        if(debugEnemy != null){
            activeCombatants[3] = (Combatant) GetNode(debugEnemy);
        }
        gui = (BattleGUI) GetNode(GUI);
        commandList = new List<BattleCommand>();
        commandList.Add(new PlayerMenuSelection());
        commandList[currentCommandIndex].Enter(this);
        battleSpots[0] = (Spatial) GetNode("./Battle Positions/Hero 1");
        battleSpots[1] = (Spatial) GetNode("./Battle Positions/Hero 2"); 
        battleSpots[2] = (Spatial) GetNode("./Battle Positions/Hero 3"); 
        battleSpots[3] = (Spatial) GetNode("./Battle Positions/Enemy 1"); 
        battleSpots[4] = (Spatial) GetNode("./Battle Positions/Enemy 2"); 
        battleSpots[5] = (Spatial) GetNode("./Battle Positions/Enemy 3");    
    }
    public override void _Process(float delta)
    {
        commandList[currentCommandIndex].Execute(delta, this);
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
