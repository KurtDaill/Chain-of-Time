using Godot;
using System;

public class CatoBasicAttackOld : BattleCommand
{
    int damageDealt = 0;
    public Combatant target;
    private PlayerCombatant cato;

    private int phase = 0;

    private int framesPassed = 0;

    //How many frames can pass between the player being able to release their attack and when they do so to get bonus damage.
    private int sweetSpotFrames = 30;
    public CatoBasicAttackOld(PlayerCombatant cato, Combatant tgt){
        this.cato = cato;
        this.target = tgt;
    }
    public override void Execute(){
        switch(phase){
            case 0 : //Phase One: Cato Plays his 'step in' animation
                if(cato.GetAnimatedSprite().Animation == "Attack Step In Hold"){
                    phase = 1;
                    break;
                }
                if(cato.GetAnimatedSprite().Animation != "Attack Step In"){
                    cato.setSprite("Attack Step In");
                    cato.queueSprite("Attack Step In Hold");
                }
                break;
            case 1 : //Phase Two: Player is prompted to hold Up, once they do: cato raises his sword
                //TODO "Hold Up" GUI Element.
                if(Input.IsActionJustPressed("ui_up")){
                    cato.setSprite("Attack Windup");
                    cato.queueSprite("Attack Windup Hold");
                }else if(!Input.IsActionPressed("ui_up")){
                    cato.setSprite("Attack Step In Hold");
                    cato.queueSprite(null);
                }
                if(cato.GetAnimatedSprite().Animation == "Attack Windup Hold") phase = 2;
                break;
            case 2 : //Phase Three: Player is prompted to release up, once they do Cato attacks. if they do it on time the attack goes off for more damage.
                //TODO "Press Down" GUI Element.
                if(Input.IsActionJustReleased("ui_up")){
                    phase = 3;    
                    cato.setSprite("Attack Swing and Recover");
                    cato.queueSprite("Idle");   
                    break;
                }
                framesPassed ++;
                break;
            case 3 :
                //TODO Damaging the target.
                if(cato.GetAnimatedSprite().Animation == "Idle"){
                    parent.NextCommand();
                }
                break;
            
        }
        
         
        
    }
    public override void Undo()
    {
        throw new NotImplementedException();
    }
}
