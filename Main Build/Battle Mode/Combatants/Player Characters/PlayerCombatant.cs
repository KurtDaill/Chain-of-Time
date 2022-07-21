using Godot;
using System;
using static AbilityUtilities;
public class PlayerCombatant : Combatant
{
    [Export]
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
    [Export]
    public String AttackHitBoxResource = "res://Battle Mode/Combatants/Player Characters/Cato/Cato Atk 1 Hitbox.tscn";
    protected PlayerCombatantSkillState[] preparedSkills = new PlayerCombatantSkillState[4];

    public override void _Ready()
    {
        base._Ready();
        SetCombatantData();
        animTree = (AnimationTree) GetNode("./AnimationTree");
        animSM = (AnimationNodeStateMachinePlayback) animTree.Get("parameters/playback");
        preparedSkills[0] = new SkillStateDebug("[center]Heavy Strike", "[center][Combo 3] Deals 17 Damage", 2, (PlayerAbilityType.Attack | PlayerAbilityType.Normal));
        preparedSkills[1] = new SkillStateDebug("[center]Shield", "[center][color=blue]Sheilds[/color] the active character for 24 Damage", 4, PlayerAbilityType.Spell);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if(hSpeed > 0){
            facing = 1;
        }else if (hSpeed < 0){
            facing = -1;
        }
        updateAnimationTree();
    }
    public virtual void Move(float delta)
    {
        if(state == null){
            throw new ArgumentNullException();
        }
        
        newState = state.Process(this, delta);
        if(newState != null) SetState(newState);
        newState = null;
    }

    public virtual bool MoveAndAttack(EnemyCombatant[] targets, int[] damageRecord, float delta){ //Generalize this!
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

    public PlayerCombatantSkillState GetSkill(int i){
        return preparedSkills[i];
    }

    public PlayerCombatantSkillState[] GetSkills(){
        return preparedSkills;
    }

    public override void spawnHitbox(String requestedHitbox){//TODO Refactor Attack Data
        switch(requestedHitbox){
            case "AttackOne":
                Godot.PackedScene hitboxResource = (PackedScene) GD.Load(AttackHitBoxResource);
                var hitbox = (Hitbox) hitboxResource.Instance();
                AddChild(hitbox);
                hitbox.SetDamage(strength);
                hitbox.SetKnockback(new Vector3(facing, .5F, 0) * 35);
                hitBoxes.Add(hitbox);
                break;
        }
    }

    public override void clearHitboxes(){
        for(int i = 0; i < hitBoxes.Count; i++){          
            hitBoxes[i].QueueFree();
            hitBoxes.Remove(hitBoxes[i]);
        }
    }

    public override void updateAnimationTree(){
        base.updateAnimationTree();
        animTree.Set("parameters/conditions/isCrouching", data.GetBool("crouching"));
        animTree.Set("parameter/conditions/isDashing", data.GetBool("dashing"));
        animTree.Set("parameters/conditions/isNotCrouching", !data.GetBool("crouching"));
        animTree.Set("parameter/conditions/isNotDashing", !data.GetBool("dashing"));
    }

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
        data.SetBool("crouching", false);
        data.SetBool("dashing", false);
    }
}