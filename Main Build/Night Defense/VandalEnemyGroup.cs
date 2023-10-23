using Godot;
using System;

public partial class VandalEnemyGroup : EnemyGroup{
    [Export]
    int vandalismRange;
    protected override void PickNewMovementPoint(){
        //Pick a Building
        //Need a Building Directory: Probably should call from city.
        //Walk Towards it using the Nav Map
    }
}