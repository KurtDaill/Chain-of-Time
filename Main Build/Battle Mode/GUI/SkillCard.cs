using System;
using Godot;
using static AbilityUtilities;

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
    public void SetDisplay(string abilityName, string rulesText, PlayerAbilityQualities type, int spCost){
        this.Visible = true;
        name.BbcodeText = abilityName;
        rules.BbcodeText = rulesText;
        switch(type & (PlayerAbilityQualities.Skill | PlayerAbilityQualities.Attack)){ //bitwise operation to sync up with switch logic
            case PlayerAbilityQualities.Skill :
                abilityType.Text = "Skill";
                break;
            case PlayerAbilityQualities.Attack :
                abilityType.Text = "Attack";
                break;
        }
        switch(type & (PlayerAbilityQualities.Normal | PlayerAbilityQualities.Spell | PlayerAbilityQualities.Tech)){ //bitwise operation to sync up with switch logic
            case PlayerAbilityQualities.Normal :
                Texture = (Texture) GD.Load("res://GUI/Battle Menu Assets/Skill Card.png");
                Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Normal.tres");
                break;
            case PlayerAbilityQualities.Spell :
                Texture = (Texture) GD.Load("res://GUI/Battle Menu Assets/Spell Card.png");
                if(abilityType.Text == "Skill") abilityType.Text = "Spell";
                Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Spell.tres");
                break;
            case PlayerAbilityQualities.Tech :
                Texture = (Texture) GD.Load("res://GUI/Battle Menu Assets/Tech Card.png");
                if(abilityType.Text == "Skill") abilityType.Text = "Tech";
                Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Tech.tres");
                break;
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