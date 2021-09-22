using Godot;
using System;

public class BattlePlayerStateFree : BattlePlayerState
{
    public BattlePlayerStateFree(BattlePlayer player){
        self = player;
    }
    public override BattleState HandleInput(BattleManager bm){
        float deltaX = 0;

        if(Input.IsActionPressed("ui_left")){
            deltaX = -1;
            self.direction = 1;
        }else if(Input.IsActionPressed("ui_right")){
            deltaX = 1;
            self.direction = 0;
        }
        /* This Block was used to test Returning New States. It worked!
        if(Input.IsActionPressed("ui_accept")){
            return new BattleStateStandby();
        }
        */
        self.velocity = new Vector2(deltaX, 0);
        self.velocity = self.velocity.Normalized();
        return null;
    }

    public override BattleState Process(BattleManager bm){
        self.velocity *= self.moveSpeed;

        if(self.velocity.Length() != 0){ //If we're moving: animate the character facing that movement's direction      
            float degrees = Mathf.Rad2Deg(Vector2.Right.AngleTo(self.velocity)); //Gets the angle of our current movement in radians, then converts to degrees
            self.direction = (int)Mathf.Round(degrees/90);
            self.anim.Animation = "Move" + self.direction;
        }else{
            self.anim.Animation = "Idle" + self.direction;
        }
        self.MoveAndSlide(self.velocity, Vector2.Zero, false, 4, 0, false);
        return null;
    }
}
