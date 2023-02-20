using Godot;
using System;

public partial class ChronoEnviro : Node3D
{
    [Export(PropertyHint.File)]
    string presentEnvironmentRes;

    [Export(PropertyHint.File)]
    string pastEnvironmentRes;
    [Export]
    WorldEnvironment realEnvironment;
    [Export]
    int enviornmentTrackIndex;
    [Export]
    int sunTrackIndex;

    Godot.Environment pastEnvironment;
    Godot.Environment presentEnvironment;
    
    [Export]
    Node3D TimeTravelCato;
    [Export]
    CSGPrimitive3D subtractSphere;
    [Export]
    CSGPrimitive3D visibleSphere;
    [Export]
    DirectionalLight3D pastSun;
    [Export]
    DirectionalLight3D presentSun;

    Godot.Collections.Array<Godot.Collections.Dictionary> pastSunProperties;
    Godot.Collections.Array<Godot.Collections.Dictionary> presentSunProperties;

    AnimationPlayer animPlay;

    private ExplorePlayer explorer;
    private TimeFragment currentFragment;

    private bool inPast;

    public override void _Ready(){
        animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
        inPast = false;
        pastEnvironment = GD.Load<Godot.Environment>(pastEnvironmentRes);
        presentEnvironment = GD.Load<Godot.Environment>(presentEnvironmentRes);
        this.realEnvironment.Environment = presentEnvironment;
        
        presentSunProperties = presentSun.GetPropertyList();
        pastSunProperties = pastSun.GetPropertyList();
        presentSun.Visible = true;
        pastSun.Visible = false; 
    }

    public void DoTheTimeWarp(TimeFragment frag, ExplorePlayer ePlayer){
        //Set TimeWarp-Cutscene Cato's position to match frag
        TimeTravelCato.GlobalPosition = frag.GlobalPosition;
        //Set Spheres to center on Cato
        subtractSphere.GlobalPosition = frag.GlobalPosition;
        visibleSphere.GlobalPosition = frag.GlobalPosition;

        //Hanldes our transition between environments.
        SetupEnvironmentAnimation();
        //Set the visible sphere to the right past scene
        visibleSphere.GetParent().RemoveChild(visibleSphere);
        frag.GetTargetEnvironment().AddChild(visibleSphere);
        frag.GetTargetEnvironment().Visible = true;

        //TEMP ANIMATION HARD CODING, WAITING ON FULL FIX
        GD.Print(visibleSphere.GetPath());
        animPlay.GetAnimation("Timewarp").TrackSetPath(8, visibleSphere.GetPath() + ":size:x");
        animPlay.GetAnimation("Timewarp").TrackSetPath(9, visibleSphere.GetPath() + ":size:y");
        animPlay.GetAnimation("Timewarp").TrackSetPath(10, visibleSphere.GetPath() + ":size:z");

        frag.DisarmTimeFragment();
        //Hide the Explore Player
        ePlayer.Visible = false;
        ePlayer.exploreCamera.Current = false;
        TimeTravelCato.GetNode<Camera3D>("Time Travel Camera Ref/Time Travel Camera").Current = true;

        explorer = ePlayer;
        currentFragment = frag;
        inPast = true;
        //Start the Timewarp animation
        animPlay.Play("TimewarpTest"); //TODO change me back
    }

    public void StopDoingTheTimeWarp(){
        explorer.Visible = true;
        TimeTravelCato.Visible = false;

        explorer.exploreCamera.Current = false;
        TimeTravelCato.GetNode<Camera3D>("Time Travel Camera Ref/Time Travel Camera").Current = true;
    }

    public void ReturnToThePresent(){
        //Hide the Explore Player
        explorer.Visible = false;
        explorer.exploreCamera.Current = false;
        TimeTravelCato.Visible = true;
        inPast = false;
        currentFragment.DisarmTimeFragment();
        //Play the Return Animation
        animPlay.Play("Return");
    }

    public void CompleteReturn(){
        explorer.Visible = true;
        TimeTravelCato.Visible = false;
        currentFragment.GetTargetEnvironment().Visible = false;
        explorer.exploreCamera.Current = false;
        TimeTravelCato.GetNode<Camera3D>("Time Travel Camera Ref/Time Travel Camera").Current = true;
    }

    public bool IsInPast(){
        return inPast;
    }

    public void SetupEnvironmentAnimation(){
        //Read the track we set up to give us the Bezier Curve of Environment Animations
        //Compare the properties of the Past and Present, if any are different, add an animation track to the transition based on 'presentToPast'
        Godot.Collections.Array<Godot.Collections.Dictionary> presentEnvProperites = presentEnvironment.GetPropertyList();
        Godot.Collections.Array<Godot.Collections.Dictionary> pastEnvProperites = pastEnvironment.GetPropertyList();

        SetupSunAnimation();

        for(int i = 0; i < pastEnvProperites.Count; i++){
            Godot.Collections.Dictionary pastDict = pastEnvProperites[i];
            Godot.Collections.Dictionary presentDict = presentEnvProperites[i];
            pastDict.TryGetValue("name", out var name); //Should be the same between environments, they're the same resource type!

            var pastVar = pastEnvironment.Get((string)name);
            var presentVar = presentEnvironment.Get((string)name);            

            pastDict.TryGetValue("type", out var typeVar); //Should be the same between environments, they're the same resource type!
            Variant.Type type = (Variant.Type)(int)typeVar;
            if(type == Variant.Type.Object && (string)name == "sky"){
                HandleSky(pastEnvironment.Sky, presentEnvironment.Sky);
                continue;
            }
            AssignPropertyToAnimationTrack((string)name, type, pastVar, presentVar, ":environment", realEnvironment, enviornmentTrackIndex);
        }
    
        //Delete those two Bezier Curve Reference Tracks to save us some error messages
        animPlay.GetAnimation("TimewarpTest").RemoveTrack(enviornmentTrackIndex);
        animPlay.GetAnimation("TimewarpTest").RemoveTrack(sunTrackIndex);
    }

    private void SetupSunAnimation(){
        foreach(Godot.Collections.Dictionary property in pastSunProperties){
            property.TryGetValue("name", out var name);
            property.TryGetValue("type", out var typeVar);
            Godot.Variant.Type type = (Godot.Variant.Type)(int) typeVar;
            var pastVar = pastSun.Get((string)name);
            var presentVar = presentSun.Get((string)name);
            if((string)name == "visible") continue;
            if((string)name == "light_cull_mask") continue;
            AssignPropertyToAnimationTrack((string)name, type, pastVar, presentVar, "", presentSun, sunTrackIndex);
        }
    }

    private void SetToggleAnimationTrack(Variant pastValue, Variant presentValue, string propertyPath, Node real){
        Animation timeWarp = animPlay.GetAnimation("TimewarpTest"); //TODO Change me
        int index = timeWarp.AddTrack(Animation.TrackType.Value);
        timeWarp.TrackSetPath(index, real.GetPath() + propertyPath);
        timeWarp.TrackInsertKey(index, 4, pastValue);
        //timeWarp.TrackInsertKey(index, timeWarp.Length, pastValue);
    }

    private void SetBezierAnimationTrack(float pastValue, float presentValue, string propertyPath, Node real, int templateIndex){
        Animation timeWarp = animPlay.GetAnimation("TimewarpTest"); //TODO Change me
        int index = timeWarp.AddTrack(Animation.TrackType.Bezier);
        timeWarp.TrackSetPath(index, real.GetPath() + propertyPath);
        timeWarp.BezierTrackInsertKey(index, timeWarp.TrackGetKeyTime(templateIndex, 0), presentValue);
        //timeWarp.BezierTrackSetKeyInHandle(index, 0, timeWarp.BezierTrackGetKeyInHandle(enviornmentTrackIndex, 0));
        //timeWarp.BezierTrackSetKeyOutHandle(index, 0, timeWarp.BezierTrackGetKeyOutHandle(enviornmentTrackIndex, 0));

        timeWarp.BezierTrackInsertKey(index, timeWarp.TrackGetKeyTime(templateIndex, 1), presentValue);
        //timeWarp.BezierTrackSetKeyInHandle(index, 1, timeWarp.BezierTrackGetKeyInHandle(enviornmentTrackIndex, 1));
        //timeWarp.BezierTrackSetKeyOutHandle(index, 1, timeWarp.BezierTrackGetKeyOutHandle(enviornmentTrackIndex, 1));   
        
        timeWarp.BezierTrackInsertKey(index, timeWarp.TrackGetKeyTime(templateIndex, 2), pastValue);
        //timeWarp.BezierTrackSetKeyInHandle(index, 2, timeWarp.BezierTrackGetKeyInHandle(enviornmentTrackIndex, 2));
        //timeWarp.BezierTrackSetKeyOutHandle(index, 2, timeWarp.BezierTrackGetKeyOutHandle(enviornmentTrackIndex, 2));

    }

    public void HandleSky(Sky pastSky, Sky presentSky){
        Godot.Collections.Array<Godot.Collections.Dictionary> presentSkyProperites = presentSky.SkyMaterial.GetPropertyList();
        Godot.Collections.Array<Godot.Collections.Dictionary> pastSkyProperites = pastSky.SkyMaterial.GetPropertyList();
        for(int i = 0; i < pastSkyProperites.Count; i++){
            Godot.Collections.Dictionary pastDict = pastSkyProperites[i];
            Godot.Collections.Dictionary presentDict = presentSkyProperites[i];
            pastDict.TryGetValue("name", out var name); //Should be the same between environments, they're the same resource type!

            var pastVar = pastEnvironment.Sky.SkyMaterial.Get((string)name);
            var presentVar = presentEnvironment.Sky.SkyMaterial.Get((string)name);            

            pastDict.TryGetValue("type", out var typeVar); //Should be the same between environments, they're the same resource type!
            Variant.Type type = (Variant.Type)(int)typeVar;
            AssignPropertyToAnimationTrack((string)name, type, pastVar, presentVar, ":environment:sky:sky_material", realEnvironment, enviornmentTrackIndex);
        }
    }

    public void AssignPropertyToAnimationTrack(string propertyName, Variant.Type type, Variant pastVar, Variant presentVar, string propertyPath, Node real, int templateIndex){
        switch(type){
                case Variant.Type.Nil : 
                    //propertyPath = ":environment:" + ((string)name).ToLower();
                    break;
                case Variant.Type.Bool :
                    bool pastBool = (bool) pastVar;
                    bool presentBool = (bool) presentVar;
                    if(pastBool != presentBool) SetToggleAnimationTrack(pastBool, presentBool, propertyPath + ":" + propertyName, real);
                    break;
                case Variant.Type.Int :
                    int pastInt = (int) pastVar;
                    int presentInt = (int) presentVar;
                    if(pastInt != presentInt) SetToggleAnimationTrack(pastInt, presentInt, propertyPath + ":" + propertyName, real);
                    break;
                case Variant.Type.Float :
                    float pastFloat = (float) pastVar;
                    float presentFloat = (float) presentVar;
                    if(pastFloat != presentFloat) SetBezierAnimationTrack(pastFloat, presentFloat, propertyPath + ":" + propertyName, real, templateIndex);
                    break;
                case Variant.Type.Vector3   :
                    Godot.Vector3 pastVector = (Godot.Vector3) pastVar;
                    Godot.Vector3 presentVector = (Godot.Vector3) presentVar;
                    if(pastVector != presentVector){
                        SetBezierAnimationTrack(pastVector.x, presentVector.x, propertyPath + ":" + propertyName + ":x", real, templateIndex);
                        SetBezierAnimationTrack(pastVector.y, presentVector.y, propertyPath + ":" + propertyName + ":y", real, templateIndex);
                        SetBezierAnimationTrack(pastVector.z, presentVector.z, propertyPath + ":" + propertyName + ":z", real, templateIndex);
                    }
                    break;
                case Variant.Type.Quaternion :
                    Godot.Quaternion pastQuat = (Godot.Quaternion) pastVar;
                    Godot.Quaternion presentQuat = (Godot.Quaternion) presentVar;
                    if(pastQuat != presentQuat){
                        SetBezierAnimationTrack(pastQuat.x, presentQuat.x, propertyPath + ":" + propertyName + ":x", real, templateIndex);
                        SetBezierAnimationTrack(pastQuat.y, presentQuat.y, propertyPath + ":" + propertyName + ":y", real, templateIndex);
                        SetBezierAnimationTrack(pastQuat.z, presentQuat.z, propertyPath + ":" + propertyName + ":z", real, templateIndex);
                        SetBezierAnimationTrack(pastQuat.w, presentQuat.w, propertyPath + ":" + propertyName + ":z", real, templateIndex);
                    }
                    break; 
                case Variant.Type.Color :
                    Godot.Color pastColor = (Godot.Color) pastVar;
                    Godot.Color presentColor = (Godot.Color) presentVar;
                    if(pastColor != presentColor){
                        SetBezierAnimationTrack(pastColor.r, presentColor.r, propertyPath + ":" +  propertyName + ":r", real, templateIndex);
                        SetBezierAnimationTrack(pastColor.g, presentColor.g, propertyPath + ":" +  propertyName + ":g", real, templateIndex);
                        SetBezierAnimationTrack(pastColor.b, presentColor.b, propertyPath + ":" +  propertyName + ":b", real, templateIndex);
                        SetBezierAnimationTrack(pastColor.a, presentColor.a, propertyPath + ":" +  propertyName + ":a", real, templateIndex);
                    }
                    break;
            }
    }
}
