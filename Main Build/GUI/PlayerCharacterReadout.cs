using Godot;
using System;

public class PlayerCharacterReadout : TextureRect
{
    Label HP, SP;
    Label maxHPLabel, maxSPLabel;
    AnimatedTexture hpIconTexture, spIconTexture;

    [Export]
    NodePath pathToDebugCharacter; //TODO replace this with real solution
    public override void _Ready()
    {
        HP = GetNode<Label>("HP");
        SP = GetNode<Label>("SP");
        maxHPLabel = GetNode<Label>("Max HP");
        maxSPLabel = GetNode<Label>("Max SP");
        hpIconTexture = (AnimatedTexture)this.GetNode<TextureRect>("HP Icon").Texture;
        spIconTexture = (AnimatedTexture)this.GetNode<TextureRect>("SP Icon").Texture;
        GetNode<PMPlayerCharacter>(pathToDebugCharacter).SetupReadout(); //TODO replace this with real solution
    }
    public void UpdateHP(int newHP, int newMaxHP){
        HP.Text = "" + newHP;
        maxHPLabel.Text = "" + newMaxHP;
        //Updates the little icon next to the HP number
        if(newHP <= 0){
            hpIconTexture.CurrentFrame = 2;
        }else if(newHP <= newMaxHP/2){
            hpIconTexture.CurrentFrame = 1;
        }else{
            hpIconTexture.CurrentFrame = 0;
        }
    }

    public void UpdateSP(int newSP, int newMaxSP){
        SP.Text = "" + newSP;
        maxSPLabel.Text = "" + newMaxSP;
        //Updates the little icon next to the SP number
        if(newSP <= 0){
            spIconTexture.CurrentFrame = 2;
        }else if(newSP <= newMaxSP/2){
            spIconTexture.CurrentFrame = 1;
        }else{
            spIconTexture.CurrentFrame = 0;
        }
    }
}
