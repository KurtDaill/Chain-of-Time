using Godot;
using System;

public partial class StoryState : Resource
{
    [Export]
    Godot.Collections.Dictionary<string, int> values;
    [Export]
    Godot.Collections.Dictionary<string, bool> flags;

    public bool TryGetFlag(string flag, out bool value){
        if(flags.TryGetValue(flag, out var result)){
            value = result;
            return true;
        }else{
            value = false;
            return false;
        }
    }

    public bool TryGetValue(string flag, out int value){
        if(values.TryGetValue(flag, out var result)){
            value = result;
            return true;
        }else{
            value = 0;
            return false;
        }
    }


    public bool HandleModifier(ScreenplayModifier mod){
        if(mod.IsInt()){
            if(mod.IsSetter()){
                return TrySetValue(mod.GetKey(), mod.GetValue());
            }
            return TryModValue(mod.GetKey(), mod.GetValue());
        }
        return TrySetFlag(mod.GetKey(), mod.GetValue() == 1);
    }
    
    public bool TrySetFlag(string flag, bool set){
        if(flags.ContainsKey(flag)){
            flags.Remove(flag);
            flags.Add(flag, set);
            return true;
        }
        else return false;
    }

    public bool TrySetValue(string flag, int setValue){
        if(values.ContainsKey(flag)){
            values.Remove(flag);
            values.Add(flag, setValue);
            return true;
        }
        else return false;
    }

    public bool TryModValue(string flag, int modValue){
        if(values.ContainsKey(flag)){
            values.TryGetValue(flag, out var temp);
            values.Remove(flag);
            values.Add(flag, temp + modValue);
            return true;
        }
        else return false;
    }
}
