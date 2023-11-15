using Godot;
using System;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class ResultsScreen : GameplayMode
{
    [Export]
    private VBoxContainer resultsTextContainer;
    [Export]
    private ExploreMode nextDayMode;
    private GameplayMode nextMode;
    private bool gameOver = false;
    private bool readyToQuit = false;

    public override void _Ready(){
        this.GetNode<Panel>("GUI").Visible = false;
        this.GetNode<DirectionalLight3D>("Result Screen Light").Visible = false;
        this.GetNode<DirectionalLight3D>("Result Screen Sky").Visible = false;
        resultsTextContainer = this.GetNode<VBoxContainer>("GUI/Background Element/Results Text Container");
        //this.Visible = false;
    }



    public override void HandleInput(GameplayUtilities.PlayerInput input)
    {
        base.HandleInput(input);
        if(gameOver){
            if(readyToQuit){
                if(input == GameplayUtilities.PlayerInput.Start){
                    GetTree().Quit();
                }
            }
        }else if(input == GameplayUtilities.PlayerInput.Start){
            nextMode = nextDayMode;
        }
    }

    public override async Task<GameplayMode> RemoteProcess(double delta)
    {
        if(nextMode != null){
            this.GetNode<GameMaster>("/root/GameMaster").NewDay();
        }
        return nextMode;
    }

    public override Task StartUp(GameplayMode oldMode){
        nextMode = null;
        if(oldMode is NightDefense){
            //Setup The Results Screen Reports!!
            NightDefense previousDefMode = oldMode as NightDefense;
            previousDefMode.EndNight();
            //TODO Add in some code to correctly write what night it is.
            Dictionary<string, int> enemyCounts = previousDefMode.GetRemainingEnemies();
            enemyCounts.TryGetValue("All", out int all);
            enemyCounts.TryGetValue("Wanderer", out int wanderer);
            enemyCounts.TryGetValue("Vandal", out int vandal);
            resultsTextContainer.GetNode<RichTextLabel>("Enemy Counts").Text = resultsTextContainer.GetNode<RichTextLabel>("Enemy Counts").Text.Replace("[ALL]", "" + all);
            resultsTextContainer.GetNode<RichTextLabel>("Enemy Counts").Text = resultsTextContainer.GetNode<RichTextLabel>("Enemy Counts").Text.Replace("[WAN]", "" + wanderer);
            resultsTextContainer.GetNode<RichTextLabel>("Enemy Counts").Text = resultsTextContainer.GetNode<RichTextLabel>("Enemy Counts").Text.Replace("[RAD]", "" + vandal);
            
            Dictionary<string, int> houseCounts = previousDefMode.GetHomeDestructionReport();
            houseCounts.TryGetValue("Overall", out int overall);
            houseCounts.TryGetValue("Tonight", out int tonight);
            houseCounts.TryGetValue("Remaining", out int remaining);
            resultsTextContainer.GetNode<RichTextLabel>("Home Counts").Text = resultsTextContainer.GetNode<RichTextLabel>("Home Counts").Text.Replace("[TN]", "" + tonight);
            resultsTextContainer.GetNode<RichTextLabel>("Home Counts").Text = resultsTextContainer.GetNode<RichTextLabel>("Home Counts").Text.Replace("[OV]", "" + overall);
            
            if(remaining >= 0){
                resultsTextContainer.GetNode<RichTextLabel>("Home Counts").Text = resultsTextContainer.GetNode<RichTextLabel>("Home Counts").Text.Replace("[US]", "" + remaining); 
            }else{ //Game over man, game over!
                resultsTextContainer.GetNode<RichTextLabel>("Home Counts").Text = resultsTextContainer.GetNode<RichTextLabel>("Home Counts").Text.Replace("[US]", "No");
                GameOver();
            }
            resultsTextContainer.GetNode<RichTextLabel>("Total Party Kill").Visible = previousDefMode.GetTotalPartyKill();
            resultsTextContainer.GetNode<RichTextLabel>("House Abandoned").Visible = previousDefMode.GetTotalPartyKill();
            
            //Setup the Scene's Aethstetics!
            this.GetNode<Camera3D>("Results Screen Camera").Current = true;
            this.GetNode<DirectionalLight3D>("Result Screen Light").Visible = true;
            this.GetNode<DirectionalLight3D>("Result Screen Sky").Visible = true;
            this.GetNode<Panel>("GUI").Visible = true;
            return Task.CompletedTask;
        }else{
            throw new ArgumentException("Results Screen can only be accessed from Night Defense Mode!");
        }
    }

    public override Task TransitionOut()
    {
        this.GetNode<Panel>("GUI").Visible = false;
        this.GetNode<DirectionalLight3D>("Result Screen Light").Visible = false;
        this.GetNode<DirectionalLight3D>("Result Screen Sky").Visible = false;
        return Task.CompletedTask;
    }

    private void GameOver(){
        this.gameOver = true;
        this.GetNode<AnimationPlayer>("GUI/AnimationPlayer").Play("GameOver");
        this.GetNode<Label>("GUI/PressStart").Visible = false;
    }

    public void OnGameOverAnimationComplete(){
        this.readyToQuit = true;
    }
}
 