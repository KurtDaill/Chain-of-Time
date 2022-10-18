using System;
using Godot;
using static PMBattleUtilities;

public class SkillCard : TextureRect{
    private RichTextLabel name, rules;
    private Label abilityType, cost;
    private AnimationPlayer anim;
    public override void _Ready(){
        name = (RichTextLabel)GetNode("Name");
        rules = (RichTextLabel)GetNode("Rules Text");
        cost = (Label)GetNode("Cost");
        abilityType = (Label)GetNode("Type");
        anim = (AnimationPlayer)GetNode("AnimationPlayer");
    }
    public void SetDisplay(string abilityName, string rulesText, string type, AbilityAlignment align, int spCost){
        this.Visible = true;
        name.BbcodeText = "[center]" + abilityName;
        rules.BbcodeText = rulesText;
        abilityType.Text = type;
        switch(align){ //bitwise operation to sync up with switch logic
            case AbilityAlignment.Normal :
                Texture = (Texture) GD.Load("res://GUI/Battle Menu Assets/Skill Card.png");
                Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Normal.tres");
                break;
            case AbilityAlignment.Magic :
                Texture = (Texture) GD.Load("res://GUI/Battle Menu Assets/Spell Card.png");
                if(abilityType.Text == "Skill") abilityType.Text = "Spell";
                Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Spell.tres");
                break;
            case AbilityAlignment.Tech :
                Texture = (Texture) GD.Load("res://GUI/Battle Menu Assets/Tech Card.png");
                Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Tech.tres");
                break;
            default :
                throw new NotImplementedException(); //TODO write custom exception
        }
        cost.Text = "" + spCost;
    }

    public void Select(){
        anim.Play("Select");
    }

    public void Draw(){
        anim.Play("Draw");
    }
    public void Stow(){
        anim.Play("Stow");
    }
    public void Deselect(){
        anim.Stop();
    }
}