using Godot;
using System;
using System.Collections.Generic;
using static AbilityUtilities;

public class CatoCombatant : PlayerCombatant {
    
    public int strength = 10;
    [Export]
	public float jumpForce = 200F;
	[Export]
	public float runSpeed = 150F;
	[Export]
	public float dashBoost = 200F;
	[Export]
	public float diveSpeed = 3F;
	[Export]
	public float slideDrag = 2F;
	[Export]
	public float footDrag = 9F;
	[Export]
	public float dashDrag = 5F;
    //How many frames does the player have to wait between their first and second attack
    [Export]
    public int secondAttackTimer = 10;
    //At what frame does the player have to throw their second attack at in order to get extra damage (a "Critical")
    [Export]
    public int secondAttackCriticalCutoff = 15;

    public override void _Ready()
    {
        SetCombatantData();
        animTree = (AnimationTree) GetNode("./AnimationTree");
        animSM = (AnimationNodeStateMachinePlayback) animTree.Get("parameters/playback");
        preparedSkills[0] = new SkillStateDebug("[center]Heavy Strike", "[center][Combo 3] Deals 17 Damage", 2, (PlayerAbilityType.Attack | PlayerAbilityType.Normal));
        preparedSkills[1] = new SkillStateDebug("[center]Shield", "[center][color=blue]Sheilds[/color] the active character for 24 Damage", 4, PlayerAbilityType.Spell);
    }
   

    //TODO Clean up Animation Solution!
    public override void updateAnimationTree(){
        base.updateAnimationTree();
        animTree.Set("parameters/conditions/isCrouching", data.GetBool("crouching"));
        animTree.Set("parameter/conditions/isDashing", data.GetBool("dashing"));
        animTree.Set("parameters/conditions/isNotCrouching", !data.GetBool("crouching"));
        animTree.Set("parameter/conditions/isNotDashing", !data.GetBool("dashing"));
    }

    
    public override void spawnHitbox(String requestedHitbox){//TODO Refactor Attack Data
        switch(requestedHitbox){
            case "AttackOne":
                Godot.PackedScene hitboxResource = (PackedScene) GD.Load("res://Battle Mode/Combatants/Player Characters/Cato/Cato Atk 1 Hitbox.tscn");
                var hitbox = (Hitbox) hitboxResource.Instance();
                AddChild(hitbox);
                hitbox.SetDamage(strength);
                hitbox.SetKnockback(new Vector3(facing, .5F, 0) * 35);
                hitBoxes.Add(hitbox);
                break;
            case "AttackTwo":
                break;
        }
    }

    public override void clearHitboxes(){ //Figure out how to make this visible to function tracks on animations without reimplementing it in the child object
        for(int i = 0; i < hitBoxes.Count; i++){          
            hitBoxes[i].QueueFree();
            hitBoxes.Remove(hitBoxes[i]);
        }
    }

    public override bool MoveAndAttack(EnemyCombatant[] targets, int[] damageRecord, float delta)
    {
            if(state is CombatantStateStandby) return true;
            Move(delta);
            if(state is PlayerCombatantStateGround && Input.IsActionJustPressed("com_atk")){
                state.Exit(this);
                 CatoStateAttackOne attackState = new CatoStateAttackOne(damageRecord, targets);
                CombatantState temp = state;
                state = attackState;
                state.Enter(this, temp);
            }
            return false;
    }

    

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if(hSpeed > 0){
            facing = 1;
        }else if (hSpeed < 0){
            facing = -1;
        }
        updateAnimationTree();
    }


    

    //Ran when the character in question is able to move and attack. Returns true once they're finished.
    //Targets & damageRecord are used to track who is damaged this attack and how much
    //They're given from the command running move and attack, and their ultimate values are stores in that command
        /*
            case attackStatus.PreAttack :
                Move();
                if(Input.IsActionJustPressed("com_attack")){
                    //atkStatus = attackStatus.ComboOne
                    //animation = attack one
                    //comboTimer = 0;
                }
                break;
            case attackStatus.ComboOne :
                    if(attackLocked){ //Player hit the key too early...
                        //Wait until the animation is done, then return true;
                        return false;
                    }
                    if(Input.IsActionJustPressed("com_attack") && comboTimer != 0){
                        if(comboTimer < secondAttackTimer){               //Player hit key too early...
                            attackLocked = true;
                        }else if(comboTimer < secondAttackCriticalCutoff){//Player hit key in time for a crit
                            atkStatus = attackStatus.ComboTwoCritical;
                        }else{                                           //Player hit key in time, normal hit
                            atkStatus = attackStatus.ComboTwo;
                        }
                    }
                    /*
                        if(animation == done){

                        }
                    
                    comboTimer ++;
                break;

            case attackStatus.ComboTwo :
                //If Attack 2 animation is done : return true;
                break;
            case attackStatus.ComboTwoCritical :
                break;
        }
        */
    
    protected override void SetCombatantData(){
        base.SetCombatantData();
        data.SetFloat("strength", strength);
        data.SetFloat("jumpForce", jumpForce);
        data.SetFloat("runSpeed", runSpeed);
        data.SetFloat("dashBoost", dashBoost);
        data.SetFloat("diveSpeed", diveSpeed);
        data.SetFloat("slideDrag", slideDrag);
        data.SetFloat("footDrag", footDrag);
        data.SetFloat("dashDrag", dashDrag);
        data.SetFloat("secondAttackTimer", secondAttackTimer);
        data.SetFloat("secondAttackerCriticalCutoff", secondAttackCriticalCutoff);
        data.SetBool("crouching", false);
        data.SetBool("dashing", false);
    }
}

