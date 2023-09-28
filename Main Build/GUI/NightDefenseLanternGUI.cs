using Godot;
using System;

public partial class NightDefenseLanternGUI : TextureRect
{
    TextureProgressBar leftBar;
    TextureProgressBar rightBar;
    AnimationPlayer lightAnimPlay;

    public override void _Ready(){
        base._Ready();
        leftBar = this.GetNode<TextureProgressBar>("Left Bar");
        rightBar = this.GetNode<TextureProgressBar>("Right Bar");
        lightAnimPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
    }

    //Light ratio is how much of the players light remains expressed as a ratio of time remaining/starting time
    public void UpdateLight(float lightRatio, ExplorePlayer explorePlayer){
        if(lightRatio == 0){
            lightAnimPlay.Play("No Torch");
            explorePlayer.SetTorchLight(0);
        }else if(lightRatio < 0.33F){
            lightAnimPlay.Play("Burn Low");
            leftBar.TintProgress = new Color("#940606");
            rightBar.TintProgress = new Color("#940606");
            explorePlayer.SetTorchLight(1);
        }else if (lightRatio < 0.66){
            lightAnimPlay.Play("Burn Mid");
            leftBar.TintProgress = new Color("#c99595");
            rightBar.TintProgress = new Color("#c99595");
            explorePlayer.SetTorchLight(2);
        }else{ //Light has to be between 66% and 100% remaining
            lightAnimPlay.Play("Burn High");
            leftBar.TintProgress = new Color("#ffffff");
            rightBar.TintProgress = new Color("#ffffff");
            explorePlayer.SetTorchLight(3);
        }
        leftBar.Value = (double) lightRatio;
        rightBar.Value = (double) lightRatio;
    }
}
