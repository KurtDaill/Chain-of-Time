using System;
using Godot;

public partial class PartyMenu : BattleMenu{
    //Used to control whether we display the 'enabled' or 'disabled' textures
    
    
    //The buttons are always in the order "Run, Swap, Combo, Ally"
    private int optionSelected = 0;
    private int lastFrameSelection = 0;
    private TextureRect[] buttonHighlights;
    private bool[] buttonsEnabled = new bool[]{false, false, false, false};
    private AnimatedTexture[] buttonTextures;
    public override void _Ready()
    {
        base._Ready();
        buttonTextures = new AnimatedTexture[]{(AnimatedTexture) this.GetNode<TextureRect>("Hub/Run!").Texture, (AnimatedTexture) this.GetNode<TextureRect>("Hub/Swap").Texture
        ,(AnimatedTexture) this.GetNode<TextureRect>("Hub/Combo").Texture, (AnimatedTexture) this.GetNode<TextureRect>("Hub/Ally").Texture};

        buttonHighlights = new TextureRect[]{this.GetNode<TextureRect>("Hub/Run!/Highlight"), this.GetNode<TextureRect>("Hub/Swap/Highlight"), 
        this.GetNode<TextureRect>("Hub/Combo/Highlight"), this.GetNode<TextureRect>("Hub/Ally/Highlight")};
    }

    public override void OnOpen(PMPlayerCharacter character, PMBattle caller)
    {
        base.OnOpen(character, caller);
        optionSelected = 1;
        lastFrameSelection = 1;
        buttonsEnabled = new bool[]{false, true, false, false}; //TODO actually tie this to whether an option is permissable using the PMBattle
        //The Full Party Menu Refactor will touch on other functionality not required for the Oct 31 Demo
        for(int i = 0; i < 4; i++){
            if(buttonsEnabled[i] == false){
                buttonTextures[i].CurrentFrame = 1; //1 is the disabled frame on these AnimatedTextures
            }
            buttonHighlights[i].Visible = false;
        }
        buttonHighlights[optionSelected].Visible = true;

    }
    public override PMPlayerAbility HandleInput(MenuInput input, PMPlayerCharacter character, PMBattle caller)
    {
        switch(input){
            case MenuInput.Left :
                if(lastFrameSelection != 0 && buttonsEnabled[0]) SwitchSelection(0);
                lastFrameSelection = 0;
                break;
            case MenuInput.Down :
                if(lastFrameSelection != 1 && buttonsEnabled[1]) SwitchSelection(1);
                lastFrameSelection = 1;
                break;
            case MenuInput.Right :
                if(lastFrameSelection != 2 && buttonsEnabled[2]) SwitchSelection(2);
                lastFrameSelection = 2;
                break;
            case MenuInput.Up :
                if(lastFrameSelection != 3 && buttonsEnabled[3]) SwitchSelection(3);
                lastFrameSelection = 3;
                break;
            case MenuInput.Select :
                switch(optionSelected){
                    case 1 :
                        if(parentGUI.parentBattle.GetPlayerCharacters().Length > 1) //If there's more than one player...
                            parentGUI.ChangeMenu(6, character, caller); //Go to the Swap Menu
                        else
                            this.GetNode<AudioStreamPlayer>("SelectError").Play();
                        break;
                }
                break;
            case MenuInput.Back :
                parentGUI.ChangeMenu(0, character, caller); //Goes back to top menu
                break;
        }
        return null;
    }

    public void SwitchSelection(int select){
        buttonHighlights[lastFrameSelection].Visible = false;
        optionSelected = select;
        buttonHighlights[optionSelected].Visible = true;
    }
}