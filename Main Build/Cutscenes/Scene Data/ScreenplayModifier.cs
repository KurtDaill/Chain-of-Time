using System;

public class ScreenplayModifier{
    private bool modIsInt = true;
    private int value;
    private string valueKey;

    bool isSetter;

    public ScreenplayModifier(bool modIsInt, int value, string valueKey, bool isSetter = false){
        this.modIsInt = modIsInt;
        this.value = value;
        this.valueKey = valueKey;
        this.isSetter = isSetter;
    }

    public bool IsInt(){
        return modIsInt;
    }

    public bool IsSetter(){
        return isSetter;
    }

    public string GetKey(){
        return valueKey;
    }

    public int GetValue(){
        return value;
    }
}