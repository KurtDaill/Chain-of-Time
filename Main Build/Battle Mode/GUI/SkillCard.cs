using System;
using Godot;
using static BattleUtilities;

public partial class SkillCard : TextureRect{
    private RichTextLabel name, rules;
    private Label abilityType, cost;
    private AnimationPlayer anim;
    AnimationPlayer pipsAnimPlay;
    private TextureRect[] pips;
    public override void _Ready(){
        name = (RichTextLabel)GetNode("Name");
        rules = (RichTextLabel)GetNode("Rules Text");
        cost = (Label)GetNode("Cost");
        abilityType = (Label)GetNode("Type");
        anim = (AnimationPlayer)GetNode("AnimationPlayer");
        pips = new TextureRect[]{
            this.GetNode<TextureRect>("Pips/Friend 3"),
            this.GetNode<TextureRect>("Pips/Friend 2"),
            this.GetNode<TextureRect>("Pips/Friend 1"),
            this.GetNode<TextureRect>("Pips/Enemy 1"),
            this.GetNode<TextureRect>("Pips/Enemy 2"),
            this.GetNode<TextureRect>("Pips/Enemy 3")
        };
        pipsAnimPlay = this.GetNode<AnimationPlayer>("Pips/AnimationPlayer");
        foreach(TextureRect pip in pips){ pip.Visible = false;}
    }
    public void SetDisplay(string abilityName, string rulesText, string type, AbilityAlignment align, int spCost, Godot.Collections.Array<BattlePosition> positions){
        this.Visible = true;
        name.Text = "[center]" + abilityName;
        cost.Text = "" + spCost;
        abilityType.Text = type;
        
        pips[0].Visible = positions.Contains(BattlePosition.HeroBack);
        pips[1].Visible = positions.Contains(BattlePosition.HeroMid);
        pips[2].Visible = positions.Contains(BattlePosition.HeroFront);
        pips[3].Visible = positions.Contains(BattlePosition.EnemyFront);
        pips[4].Visible = positions.Contains(BattlePosition.EnemyMid);
        pips[5].Visible = positions.Contains(BattlePosition.EnemyBack);
        //Developers can specify the font size for their rules text within the rules text string, this parses that information
        string textSize = "";
        if(rulesText.StartsWith("[textSize]")){
            rulesText = rulesText.Remove(0,10);
            if(rulesText.StartsWith("smallest")){
                textSize = "smallest";
                rulesText = rulesText.Remove(0,8);
            }
            if(rulesText.StartsWith("small")){
                textSize = "small";
                rulesText = rulesText.Remove(0,5);
            }
        }
        rules.Text = rulesText;

        switch(align){ //Assigns the card's graphic and text theme to match it's alignment
            case AbilityAlignment.Normal :
                Texture = (Texture2D) GD.Load("res://GUI/Battle Menu Assets/Skill Menu/Skill Card.png");
                Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Normal.tres");
                if(textSize == "small"){
                    rules.Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Normal Small.tres");
                }else if(textSize == "smallest"){
                    rules.Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Normal Smallest.tres");
                } 
                break;
            case AbilityAlignment.Magic :
                Texture = (Texture2D) GD.Load("res://GUI/Battle Menu Assets/Skill Menu/Spell Card.png");
                Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Spell.tres");
                if(textSize == "small"){
                    rules.Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Spell Small.tres");
                }else if(textSize == "smallest"){
                    rules.Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Spell Smallest.tres");
                } 
                break;
            case AbilityAlignment.Tech :
                Texture = (Texture2D) GD.Load("res://GUI/Battle Menu Assets/Skill Menu/Tech Card.png");
                Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Tech.tres");
                if(textSize == "small"){
                    rules.Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Tech Small.tres");
                }else if(textSize == "smallest"){
                    rules.Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Tech Smallest.tres");
                }
                break;
            default :
                throw new NotImplementedException(); //TODO write custom exception
        }
    }

    public void Select(){
        anim.Play("Select");
    }
    new public void Draw(){
        anim.Play("Draw");
    }
    public void Stow(){
        anim.Play("Stow");
    }
    public void Deselect(){
        anim.Stop();
    }

    public void FlashFriendlyPips(){
        pipsAnimPlay.Play("Flash Friendlies");
    }
}
