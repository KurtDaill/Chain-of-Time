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
    public void SetDisplay(string abilityName, string rulesText, PlayerAbilityType type, int spCost){
        this.Visible = true;
        name.BbcodeText = abilityName;
        rules.BbcodeText = rulesText;
        switch(type & (PlayerAbilityType.Skill | PlayerAbilityType.Attack)){ //bitwise operation to sync up with switch logic
            case PlayerAbilityType.Skill :
                abilityType.Text = "Skill";
                break;
            case PlayerAbilityType.Attack :
                abilityType.Text = "Attack";
                break;
        }
        switch(type & (PlayerAbilityType.Normal | PlayerAbilityType.Spell | PlayerAbilityType.Tech)){ //bitwise operation to sync up with switch logic
            case PlayerAbilityType.Normal :
                Texture = (Texture) GD.Load("res://GUI/Battle Menu Assets/Skill Card.png");
                Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Normal.tres");
                break;
            case PlayerAbilityType.Spell :
                Texture = (Texture) GD.Load("res://GUI/Battle Menu Assets/Spell Card.png");
                if(abilityType.Text == "Skill") abilityType.Text = "Spell";
                Theme = (Theme) GD.Load("res://GUI/Themes/Skill Card Spell.tres");
                break;
            case PlayerAbilityType.Tech :
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