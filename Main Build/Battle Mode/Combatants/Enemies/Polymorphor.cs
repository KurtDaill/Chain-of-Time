using Godot;
using System;

public class Polymorphor : EnemyCombatant
{
    [Export]
    public float footDrag;
    public override void _Ready()
    {
        //hitbox = (Area2D) GetNode("Hitbox");
        //if(hitbox == null){
            //throw new NotImplementedException();
        //}
        animTree = (AnimationTree) GetNode("AnimationTree");
        animSM = (AnimationNodeStateMachinePlayback) animTree.Get("parameters/playback");
        SetCombatantData();
        abilitiesKnown = new CombatantAbilityState[]{};
    }
    public override void _Process(float delta)
    {
        updateAnimationTree();
    }

    public override void spawnHitbox(String requestedHitbox){//TODO Refactor Attack Data
        switch(requestedHitbox){
            case "Slide":
                Godot.PackedScene hitboxResource = (PackedScene) GD.Load("res://Battle Mode/Combatants/Enemies/Polymorphor Slide Hitbox.tscn");
                var hitbox = (Hitbox) hitboxResource.Instance();
                AddChild(hitbox);
                hitbox.SetDamage(5);
                hitbox.SetKnockback(new Vector3(-1, .1F, 0) * 30);
                hitBoxes.Add(hitbox);
                break;
        }
    }

    public override void clearHitboxes(){ //Figure out how to make this visible to function tracks on animations without reimplementing it in the child object
        for(int i = 0; i < hitBoxes.Count; i++){          
            hitBoxes[i].QueueFree();
            hitBoxes.Remove(hitBoxes[i]);
        }
    }

    public override int TakeDamage(int incomingDamage, Vector3 knockback){
        int dmg = base.TakeDamage(incomingDamage, knockback);
        GD.Print(Name + " hit! : " + dmg + " Damage Dealt!");
        return dmg;
    }
    
    public override void updateAnimationTree(){
        animTree.Set("parameters/conditions/inPainState", data.GetBool("painState"));
    }

    protected override void SetCombatantData(){
        data.SetFloat("footDrag", footDrag);
        base.SetCombatantData();
    }

    public override void DecideAbility()
    {
        SetState(new EnemyCombatantStateSlideAttack(20F, 0.5F, 3F));
    }
}
