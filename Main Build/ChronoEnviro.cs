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
    CronoScene presentCronoScene;
    [Export]
    int enviornmentTrackIndex;
    [Export]
    int sunTrackIndex;
    [Export]
    int returnEnvironmentTrackIndex;
    [Export]
    int returnSunTrackIndex;

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
    private DirectionalLight3D realSun;

    Godot.Collections.Array<Godot.Collections.Dictionary> sunProperties;

    AnimationPlayer animPlay;

    private ExplorePlayer explorer;
    private TimeFragment currentFragment;

    private bool inPast;

    public override void _Ready(){
        animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
        inPast = false;
        pastEnvironment = GD.Load<Godot.Environment>(pastEnvironmentRes);
        presentEnvironment = GD.Load<Godot.Environment>(presentEnvironmentRes);
        this.realEnvironment.Environment = (Godot.Environment)presentEnvironment.Duplicate(true);
        
        sunProperties = pastSun.GetPropertyList();
        realSun = (DirectionalLight3D)presentSun.Duplicate();
        this.AddChild(realSun);
        realSun.Visible = true;
        presentSun.Visible = false;
        pastSun.Visible = false; 
    }

    public void DoTheTimeWarp(TimeFragment frag, ExplorePlayer ePlayer){
        //Set TimeWarp-Cutscene Cato's position to match frag
        TimeTravelCato.GlobalPosition = frag.GlobalPosition;
        //Set Spheres to center on Cato
        subtractSphere.GlobalPosition = frag.GlobalPosition;
        visibleSphere.GlobalPosition = frag.GlobalPosition;
        //Set the visible sphere to the right past scene
        visibleSphere.GetParent().RemoveChild(visibleSphere);
        frag.GetTargetCronoScene().AddChild(visibleSphere);
        frag.GetTargetCronoScene().Visible = true;

        pastEnvironment = frag.GetTargetEnvironment();
        pastSun = frag.GetTargetSun();

        //TEMP ANIMATION HARD CODING, WAITING ON FULL FIX
        GD.Print(visibleSphere.GetPath());
        animPlay.GetAnimation("TimewarpTest").TrackSetPath(5, visibleSphere.GetPath() + ":size:x");
        animPlay.GetAnimation("TimewarpTest").TrackSetPath(6, visibleSphere.GetPath() + ":size:y");
        animPlay.GetAnimation("TimewarpTest").TrackSetPath(7, visibleSphere.GetPath() + ":size:z");

        frag.DisarmTimeFragment();
        //Hide the Explore Player
        ePlayer.Visible = false;
        //ePlayer.exploreCamera.Current = false;
        //TimeTravelCato.GetNode<Camera3D>("Time Travel Camera Ref/Time Travel Camera").Current = true;
        GetNode<CameraManager>("/root/CameraManager").SwitchCamera(TimeTravelCato.GetNode<Camera3D>("Time Travel Camera Ref/Time Travel Camera"));

        explorer = ePlayer;
        currentFragment = frag;
        inPast = true;

        //Hanldes our transition between environments.
        SetupEnvironmentAnimation(false);

        //Start the Timewarp animation
        animPlay.Play("TimewarpTest"); //TODO change me back
    }

    //Called by the transition animations to have the proper objects spawn/despawn for the time travel transition
    public void SetupCronoScenes(bool isReturnRun){
        if(isReturnRun){
            currentFragment.GetTargetCronoScene().HideChildModules();
            presentCronoScene.ShowChildModules();
        }else{
            currentFragment.GetTargetCronoScene().ShowChildModules();
            presentCronoScene.HideChildModules();
        }
    }

    public void StopDoingTheTimeWarp(){
        TimeTravelCato.Visible = false; 
        TimeTravelCato.GetNode<Camera3D>("Time Travel Camera Ref/Time Travel Camera").Current = false;
        presentCronoScene.Visible = false;
        if(currentFragment.HasCutsceneArmed()){
            currentFragment.PlayCutscene();
        }else{    
            explorer.Visible = true;
            explorer.exploreCamera.Current = true;
        }
    }

    public void ReturnToThePresent(){
        //Hide the Explore Player
        explorer.SetActive(false);
        GetNode<CameraManager>("/root/CameraManager").SwitchCamera(TimeTravelCato.GetNode<Camera3D>("Time Travel Camera Ref/Time Travel Camera"));
        TimeTravelCato.Visible = true;
        inPast = false;
        currentFragment.DisarmTimeFragment();
        presentCronoScene.Visible = true;
        SetupEnvironmentAnimation(true);
        //Play the Return Animation
        animPlay.Play("Return");
    }

    public void CompleteReturn(){
        explorer.SetActive(true);
        GetNode<CameraManager>("/root/CameraManager").SwitchCamera(explorer.exploreCamera);
        TimeTravelCato.Visible = false;
        currentFragment.GetTargetCronoScene().Visible = false;
    }

    public bool IsInPast(){
        return inPast;
    }

    public void SetupEnvironmentAnimation(bool isReturnRun){
        //Read the track we set up to give us the Bezier Curve of Environment Animations
        //Compare the properties of the Past and Present, if any are different, add an animation track to the transition based on 'presentToPast'
        Godot.Collections.Array<Godot.Collections.Dictionary> presentEnvProperites = presentEnvironment.GetPropertyList();
        Godot.Collections.Array<Godot.Collections.Dictionary> pastEnvProperites = pastEnvironment.GetPropertyList();

        SetupSunAnimation(isReturnRun);

        for(int i = 0; i < pastEnvProperites.Count; i++){
            Godot.Collections.Dictionary pastDict = pastEnvProperites[i];
            Godot.Collections.Dictionary presentDict = presentEnvProperites[i];
            pastDict.TryGetValue("name", out var name); //Should be the same between environments, they're the same resource type!

            var pastVar = pastEnvironment.Get((string)name);
            var presentVar = presentEnvironment.Get((string)name);            

            pastDict.TryGetValue("type", out var typeVar); //Should be the same between environments, they're the same resource type!
            Variant.Type type = (Variant.Type)(int)typeVar;
            if(type == Variant.Type.Object && (string)name == "sky"){
                if(isReturnRun) HandleSky(presentEnvironment.Sky, pastEnvironment.Sky, isReturnRun);
                else HandleSky(pastEnvironment.Sky, presentEnvironment.Sky, isReturnRun);
                continue;
            }
            if(isReturnRun) AssignPropertyToAnimationTrack((string)name, type, presentVar, pastVar, ":environment", realEnvironment, returnEnvironmentTrackIndex, "Return");
            else AssignPropertyToAnimationTrack((string)name, type, pastVar, presentVar, ":environment", realEnvironment, enviornmentTrackIndex, "TimewarpTest");
        }
    }

    private void SetupSunAnimation(bool isReturnRun){
        foreach(Godot.Collections.Dictionary property in sunProperties){
            property.TryGetValue("name", out var name);
            property.TryGetValue("type", out var typeVar);
            Godot.Variant.Type type = (Godot.Variant.Type)(int) typeVar;
            var pastVar = pastSun.Get((string)name);
            var presentVar = presentSun.Get((string)name);
            if((string)name == "visible") continue;
            if((string)name == "light_cull_mask") continue;
            if(isReturnRun) AssignPropertyToAnimationTrack((string)name, type, presentVar, pastVar, "", realSun, returnSunTrackIndex, "Return");
            else AssignPropertyToAnimationTrack((string)name, type, pastVar, presentVar, "", realSun, sunTrackIndex, "TimewarpTest");
        }
    }
    private void SetToggleAnimationTrack(Variant endValue, Variant startValue, string propertyPath, Node real, string animation){
        Animation timeWarp = animPlay.GetAnimation(animation); //TODO Change me
        int index = timeWarp.AddTrack(Animation.TrackType.Value);
        timeWarp.TrackSetPath(index, real.GetPath() + propertyPath);
        timeWarp.TrackInsertKey(index, 4, endValue);
        //timeWarp.TrackInsertKey(index, timeWarp.Length, endValue);
    }

    private void SetBezierAnimationTrack(float endValue, float startValue, string propertyPath, Node real, int templateIndex, string animation){
        Animation timeWarp = animPlay.GetAnimation(animation); //TODO Change me
        int index = timeWarp.AddTrack(Animation.TrackType.Bezier);
        timeWarp.TrackSetPath(index, real.GetPath() + propertyPath);
        timeWarp.BezierTrackInsertKey(index, timeWarp.TrackGetKeyTime(templateIndex, 0), startValue);
        //timeWarp.BezierTrackSetKeyInHandle(index, 0, timeWarp.BezierTrackGetKeyInHandle(enviornmentTrackIndex, 0));
        //timeWarp.BezierTrackSetKeyOutHandle(index, 0, timeWarp.BezierTrackGetKeyOutHandle(enviornmentTrackIndex, 0));

        timeWarp.BezierTrackInsertKey(index, timeWarp.TrackGetKeyTime(templateIndex, 1), startValue);
        //timeWarp.BezierTrackSetKeyInHandle(index, 1, timeWarp.BezierTrackGetKeyInHandle(enviornmentTrackIndex, 1));
        //timeWarp.BezierTrackSetKeyOutHandle(index, 1, timeWarp.BezierTrackGetKeyOutHandle(enviornmentTrackIndex, 1));   
        
        timeWarp.BezierTrackInsertKey(index, timeWarp.TrackGetKeyTime(templateIndex, 2), endValue);
        //timeWarp.BezierTrackSetKeyInHandle(index, 2, timeWarp.BezierTrackGetKeyInHandle(enviornmentTrackIndex, 2));
        //timeWarp.BezierTrackSetKeyOutHandle(index, 2, timeWarp.BezierTrackGetKeyOutHandle(enviornmentTrackIndex, 2));

    }

    public void HandleSky(Sky endSky, Sky startSky, bool isReturnRun){
        Godot.Collections.Array<Godot.Collections.Dictionary> startSkyProperites = startSky.SkyMaterial.GetPropertyList();
        Godot.Collections.Array<Godot.Collections.Dictionary> endSkyProperites = endSky.SkyMaterial.GetPropertyList();
        for(int i = 0; i < endSkyProperites.Count; i++){
            Godot.Collections.Dictionary endDict = endSkyProperites[i];
            Godot.Collections.Dictionary startDict = startSkyProperites[i];
            endDict.TryGetValue("name", out var name); //Should be the same between environments, they're the same resource type!

            var endVar = endSky.SkyMaterial.Get((string)name);
            var startVar = startSky.SkyMaterial.Get((string)name);            

            endDict.TryGetValue("type", out var typeVar); //Should be the same between environments, they're the same resource type!
            Variant.Type type = (Variant.Type)(int)typeVar;
            if(isReturnRun) AssignPropertyToAnimationTrack((string)name, type, endVar, startVar, ":environment:sky:sky_material", realEnvironment, enviornmentTrackIndex, "Return");
            else AssignPropertyToAnimationTrack((string)name, type, endVar, startVar, ":environment:sky:sky_material", realEnvironment, enviornmentTrackIndex, "TimewarpTest");
        }
    }

    public void AssignPropertyToAnimationTrack(string propertyName, Variant.Type type, Variant endVar, Variant startVar, string propertyPath, Node real, int templateIndex, string animation){
        switch(type){
                case Variant.Type.Nil : 
                    //propertyPath = ":environment:" + ((string)name).ToLower();
                    break;
                case Variant.Type.Bool :
                    bool endBool = (bool) endVar;
                    bool startBool = (bool) startVar;
                    if(endBool != startBool) SetToggleAnimationTrack(endBool, startBool, propertyPath + ":" + propertyName, real, animation);
                    break;
                case Variant.Type.Int :
                    int endInt = (int) endVar;
                    int startInt = (int) startVar;
                    if(endInt != startInt) SetToggleAnimationTrack(endInt, startInt, propertyPath + ":" + propertyName, real, animation);
                    break;
                case Variant.Type.Float :
                    float endFloat = (float) endVar;
                    float startFloat = (float) startVar;
                    if(endFloat != startFloat) SetBezierAnimationTrack(endFloat, startFloat, propertyPath + ":" + propertyName, real, templateIndex, animation);
                    break;
                case Variant.Type.Vector3   :
                    Godot.Vector3 endVector = (Godot.Vector3) endVar;
                    Godot.Vector3 startVector = (Godot.Vector3) startVar;
                    if(endVector != startVector){
                        SetBezierAnimationTrack(endVector.x, startVector.x, propertyPath + ":" + propertyName + ":x", real, templateIndex, animation);
                        SetBezierAnimationTrack(endVector.y, startVector.y, propertyPath + ":" + propertyName + ":y", real, templateIndex, animation);
                        SetBezierAnimationTrack(endVector.z, startVector.z, propertyPath + ":" + propertyName + ":z", real, templateIndex, animation);
                    }
                    break;
                case Variant.Type.Quaternion :
                    Godot.Quaternion endQuat = (Godot.Quaternion) endVar;
                    Godot.Quaternion startQuat = (Godot.Quaternion) startVar;
                    if(endQuat != startQuat){
                        SetBezierAnimationTrack(endQuat.x, startQuat.x, propertyPath + ":" + propertyName + ":x", real, templateIndex, animation);
                        SetBezierAnimationTrack(endQuat.y, startQuat.y, propertyPath + ":" + propertyName + ":y", real, templateIndex, animation);
                        SetBezierAnimationTrack(endQuat.z, startQuat.z, propertyPath + ":" + propertyName + ":z", real, templateIndex, animation);
                        SetBezierAnimationTrack(endQuat.w, startQuat.w, propertyPath + ":" + propertyName + ":z", real, templateIndex, animation);
                    }
                    break; 
                case Variant.Type.Color :
                    Godot.Color endColor = (Godot.Color) endVar;
                    Godot.Color startColor = (Godot.Color) startVar;
                    if(endColor != startColor){
                        SetBezierAnimationTrack(endColor.r, startColor.r, propertyPath + ":" +  propertyName + ":r", real, templateIndex, animation);
                        SetBezierAnimationTrack(endColor.g, startColor.g, propertyPath + ":" +  propertyName + ":g", real, templateIndex, animation);
                        SetBezierAnimationTrack(endColor.b, startColor.b, propertyPath + ":" +  propertyName + ":b", real, templateIndex, animation);
                        SetBezierAnimationTrack(endColor.a, startColor.a, propertyPath + ":" +  propertyName + ":a", real, templateIndex, animation);
                    }
                    break;
            }
    }
}
