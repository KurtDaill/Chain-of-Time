using System;

public class ScreenplayModifier{
    private bool modIsInt = true;
    private int mod;
    private string valueKey;

    bool isSetter;

    public ScreenplayModifier(bool modIsInt, int mod, string valueKey, bool isSetter = false){
        this.modIsInt = modIsInt;
        this.mod = mod;
        this.valueKey = valueKey;
        this.isSetter = isSetter;
    }

    public bool DoesThisUseIntegers(){
        return modIsInt;
    }
}