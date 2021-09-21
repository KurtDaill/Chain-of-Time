using Godot;
using System;

public class ExplorePlayerStateFree : ExplorePlayerState
{
    public override void HandleInput(ExplorePlayer self){
        float deltaX = 0;
        float deltaY = 0;
        if(Input.IsActionPressed("ui_up")){
            deltaY = -1;
        }else if(Input.IsActionPressed("ui_down")){
            deltaY = 1;
        }

        if(Input.IsActionPressed("ui_left")){
            deltaX = -1;
        }else if(Input.IsActionPressed("ui_right")){
            deltaX = 1;
        }
        self.velocity = new Vector2(deltaX, deltaY);
        self.velocity = self.velocity.Normalized();
        


        /*
        Position = new Vector2(
            x : Mathf.Clamp(Position.x, 0, )

        )
        */
    }

    public override void Process(ExplorePlayer self){
        self.velocity *= self.moveSpeed;

        if(self.velocity.Length() != 0){ //If we're moving: animate the character facing that movement's direction      
            float degrees = Mathf.Rad2Deg(Vector2.Right.AngleTo(self.velocity)); //Gets the angle of our current movement in radians, then converts to degrees
            self.direction = (int)Mathf.Round(degrees/90);
            self.direction = WrapInteger4D(self.direction);
            self.anim.Animation = "Walk" + self.direction;
        }else{
            self.anim.Animation = "Idle" + self.direction;
        }
        self.MoveAndSlide(self.velocity, Vector2.Zero, false, 4, 0, false);
    }
    private int WrapInteger4D(int original){
        if(original == -2) return 3;
        if(original == -1) return 3;
        return original;
    }
}
