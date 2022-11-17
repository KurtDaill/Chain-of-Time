using Godot;
using System;

public class PlayerCharacterReadout : TextureRect
{
    Label HP, SP;
    Label maxHPLabel, maxSPLabel;
    TextureRect hpIconFull, hpIconHalf, hpIconEmpty, spIconFull, spIconHalf, spIconEmpty, highLight;
    public PMPlayerCharacter character;

    public override void _Ready()
    {
        HP = GetNode<Label>("HP");
        SP = GetNode<Label>("SP");
        maxHPLabel = GetNode<Label>("Max HP");
        maxSPLabel = GetNode<Label>("Max SP");
        hpIconFull = this.GetNode<TextureRect>("HP Icon Full");
        hpIconHalf = this.GetNode<TextureRect>("HP Icon Half");
        hpIconEmpty = this.GetNode<TextureRect>("HP Icon Empty");
        spIconFull = this.GetNode<TextureRect>("SP Icon Full");
        spIconHalf = this.GetNode<TextureRect>("SP Icon Half");
        spIconEmpty = this.GetNode<TextureRect>("SP Icon Empty");
        highLight = this.GetNode<TextureRect>("Highlight");
        //character.SetupReadout();
        //GetParent<ReadoutContainer>().Reorder();
    }
    public void UpdateHP(int newHP, int newMaxHP){
        HP.Text = "" + newHP;
        maxHPLabel.Text = "" + newMaxHP;
        //Updates the little icon next to the HP number
        hpIconFull.Visible = false;
        hpIconHalf.Visible = false;
        hpIconEmpty.Visible = false;
        if(newHP <= 0){
            hpIconEmpty.Visible = true;
        }else if(newHP <= newMaxHP/2){
            hpIconHalf.Visible = true;
        }else{
            hpIconFull.Visible = true;
        }
    }

    public void UpdateSP(int newSP, int newMaxSP){
        SP.Text = "" + newSP;
        maxSPLabel.Text = "" + newMaxSP;
        //Updates the little icon next to the SP number
        spIconFull.Visible = false;
        spIconHalf.Visible = false;
        spIconEmpty.Visible = false;
        if(newSP <= 0){
            spIconEmpty.Visible = true;
        }else if(newSP <= newMaxSP/2){
            spIconHalf.Visible = true;
        }else{
            spIconFull.Visible = true;
        }
    }

    public void EnableHighlight(){
        highLight.Visible = true;
    }

    public void DisableHighlight(){
        highLight.Visible = false;
    }
}
