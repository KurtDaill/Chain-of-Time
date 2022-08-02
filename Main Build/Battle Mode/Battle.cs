using Godot;
using System;
using System.Collections.Generic;

//Class used to represent and run turn based battles
public class Battle : Node
{
    //Tracks all active combatants in the scene
    public Combatant[] activeCombatants = new Combatant[6];
    //Tracks Enemies that may enter in waves as active enemies are defeated;
    public List<Combatant> enemyBench = new List<Combatant>();

    [Export]
    public NodePath debugPlayer;
    [Export]
    public NodePath debugPlayer2;

    [Export]
    public NodePath cameraPath;

    public MovingCamera camera;

    [Export]
    public NodePath debugEnemy;

    public List<BattleEffect> startOfRoundEffects = new List<BattleEffect>();
    public List<BattleEffect> endOfRoundEffects = new List<BattleEffect>();

    [Export]
    public NodePath GUI;

    public BattleGUI gui;
    private List<BattleCommand> commandList;
    private int currentCommandIndex;
    
    public BattlePositionManager positionManager;
    public override void _Ready()
    {
        if(debugPlayer != null){
            activeCombatants[0] = (Combatant) GetNode(debugPlayer);
        }
        if(debugEnemy != null){
            activeCombatants[3] = (Combatant) GetNode(debugEnemy);
        }
        if(debugPlayer2 != null){
            activeCombatants[1] = (Combatant) GetNode(debugPlayer2);
        }
        camera = (MovingCamera) GetNode(cameraPath);
        gui = (BattleGUI) GetNode(GUI);
        positionManager = (BattlePositionManager) GetNode("Battle Positions");
        commandList = new List<BattleCommand>();
        commandList.Add(new PlayerMenuSelection());
        commandList[currentCommandIndex].Enter(this);   
    }
    public override void _Process(float delta)
    {
        commandList[currentCommandIndex].Execute(delta, this);
    }

    public void NextCommand(bool runEnter = true){//Called by commands when they're completed
        commandList[currentCommandIndex].Exit();
        currentCommandIndex += 1;
        if(runEnter) commandList[currentCommandIndex].Enter(this);
    }

    public void InsertCommandNext(BattleCommand insertCom){
        commandList.Insert(currentCommandIndex + 1, insertCom);
    }

    public void AddCommand(BattleCommand newCom){
        commandList.Add(newCom);
    }

    public BattleCommand PeakCommand(){
        return commandList[currentCommandIndex + 1];
    }

    public BattleCommand CurrentCommand(){
        return commandList[currentCommandIndex];
    }
}

