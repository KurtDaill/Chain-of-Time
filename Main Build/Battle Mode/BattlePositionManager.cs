using Godot;
using System;

public class BattlePositionManager : Spatial
{
        public Spatial[] battleSpots = new Spatial[6];
        public Spatial[] battleSpotsBack = new Spatial[6];
        public int[] backgroundedCombatants;
        public int[] foregroundedCombatants;

        public float moveSpeed = 1F;

        public float moveTimer = 0F;

        public enum State{
            Standby,
            ResetBattlefield,
            Backgrounding
        }

        public State currentState = State.Standby;
        private Battle parent;
        public override void _Ready(){
            battleSpots[0] = (Spatial) GetNode("Hero 1");
            battleSpots[1] = (Spatial) GetNode("Hero 2"); 
            battleSpots[2] = (Spatial) GetNode("Hero 3"); 
            battleSpots[3] = (Spatial) GetNode("Enemy 1"); 
            battleSpots[4] = (Spatial) GetNode("Enemy 2"); 
            battleSpots[5] = (Spatial) GetNode("Enemy 3");    
            battleSpotsBack[0] = (Spatial) GetNode("Hero 1 Back");
            battleSpotsBack[1] = (Spatial) GetNode("Hero 2 Back"); 
            battleSpotsBack[2] = (Spatial) GetNode("Hero 3 Back"); 
            battleSpotsBack[3] = (Spatial) GetNode("Enemy 1 Back"); 
            battleSpotsBack[4] = (Spatial) GetNode("Enemy 2 Back"); 
            battleSpotsBack[5] = (Spatial) GetNode("Enemy 3 Back");
            
            parent = (Battle) this.GetParent(); 
        }

        public override void _Process(float delta){
            switch(currentState){
                case State.ResetBattlefield :
                    for(int i = 0; i < parent.activeCombatants.GetLength(0); i++){
                        if(parent.activeCombatants[i] != null){
                            parent.activeCombatants[i].Transform = parent.activeCombatants[i].Transform.InterpolateWith(battleSpots[i].Transform, delta * moveSpeed);
                        }
                    }
                    if(moveTimer < 0){
                        SetToDefaultPositions();
                        currentState = State.Standby;
                        moveTimer = 0;
                    }
                    moveTimer -= delta;
                    break;
                case State.Backgrounding :
                    foreach(int i in foregroundedCombatants){
                        parent.activeCombatants[i].Transform = parent.activeCombatants[i].Transform.InterpolateWith(battleSpots[i].Transform, delta * moveSpeed);
                    }
                    foreach(int i in backgroundedCombatants){
                        parent.activeCombatants[i].Transform = parent.activeCombatants[i].Transform.InterpolateWith(battleSpotsBack[i].Transform, delta * moveSpeed);
                    }
                    if(moveTimer < 0){
                        SetForegroundBackground(foregroundedCombatants);
                        currentState = State.Standby;
                        moveTimer = 0;
                    }
                    moveTimer -= delta;
                    break;
            }
        }

        public void SetToDefaultPositions(){
            for(int i = 0; i < 6; i++){
                if(parent.activeCombatants[i] != null){
                    parent.activeCombatants[i].Transform = battleSpots[i].Transform;
                    //TODO Set the combatant to standby, facing the correct way
                }
            }
            currentState = State.Standby;
        }

        public void InterpolateToDefaultPositions(float speed = 2F, float timer = 2F){
            currentState = State.ResetBattlefield;
            moveSpeed = speed;
            moveTimer = timer;
        }

        public void SetForegroundBackground(int[] foregroundCharacters){
            foregroundCharacters = Array.FindAll(foregroundCharacters, element => parent.activeCombatants[element] != null);
            for(int i = 0; i < 6; i++){
                if(parent.activeCombatants[i] != null){
                    if(Array.Exists(foregroundCharacters, element => element == i)) parent.activeCombatants[i].Transform = battleSpots[i].Transform;
                    else parent.activeCombatants[i].Transform = battleSpotsBack[i].Transform;
                    //TODO Set the combatant to standby, facing the correct way
                }
            }
            currentState = State.Standby;
        }

        public void InterpolateToForegroundBackground(int[] foregroundCharacters, float speed = 2F, float timer = 2F){
            backgroundedCombatants = new int[6];
            foregroundedCombatants  = new int[6];

            foregroundedCombatants = Array.FindAll(foregroundCharacters, element => parent.activeCombatants[element] != null);
            for(int i = 0; i < 6; i++){
                if(parent.activeCombatants[i] != null){
                    if(Array.Find(foregroundedCombatants, element => element == i) == -1){
                        backgroundedCombatants[i] = i;
                    }
                }
            }
            currentState = State.Backgrounding;
            moveSpeed = speed;
            moveTimer = timer;
        }
}
