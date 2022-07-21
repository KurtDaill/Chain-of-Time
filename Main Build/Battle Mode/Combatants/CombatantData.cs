using Godot;
using System;
using System.Collections.Generic;

public class CombatantData
{
    Dictionary<String, float> dataFloat = new Dictionary<String, float>();
    Dictionary<String, bool> dataBool = new Dictionary<String, bool>();

    public void SetFloat(String key, float value){
            dataFloat.Remove(key);
            dataFloat.Add(key, value);
    }

    public float GetFloat(String key){
        var value = 0F;
        if(dataFloat.TryGetValue(key, out value)){
            return value;
        }else{
            throw new InvalidCombatantDataAccessed("Uninitialized CombatantData Float Accessed: " + key);
            //return 0;
        }
    }

    public void SetBool(String key, bool value){
            dataBool.Remove(key);
            dataBool.Add(key, value);
    }

    public bool GetBool(String key){
        var value = false;
        if(dataBool.TryGetValue(key, out value)){
            return value;
        }else{
            throw new InvalidCombatantDataAccessed("Uninitialized CombatantData Bool Accessed: " + key);
            //return false;
        }
    }

    //Used when the combatant checks for a bool that may not be set
    //Generally used to check for options that are rarely relevant, so 'false' is default and easily handled.
    public bool CheckBool(String key){
        var value = false;
        if(dataBool.TryGetValue(key, out value)){}
        else GD.Print("Unset Bool Checked: " + key);
        return value;
    }
}

public class InvalidCombatantDataAccessed : Exception
{
    public InvalidCombatantDataAccessed()
    {
    }

    public InvalidCombatantDataAccessed(string message)
        : base(message)
    {
    }

    public InvalidCombatantDataAccessed(string message, Exception inner)
        : base(message, inner)
    {
    }
}