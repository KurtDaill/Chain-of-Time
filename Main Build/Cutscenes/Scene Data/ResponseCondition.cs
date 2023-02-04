using System;

//A Class design to hold data relevant to whether or not a dialogue response is premitted at this time

public class ResponseCondition{
    private string conditionKey;

    private ConditionType type = ConditionType.ExactMatch;

    private int conditionValue;

    public ResponseCondition(string conditionKey, ConditionType type, int conditionValue){
        this.conditionKey = conditionKey;
        this.type = type;
        this.conditionValue = conditionValue;
    }

    public string GetKey(){
        return conditionKey;
    }

    public bool isConditionMet(int response){
        switch(type){
            case ConditionType.IntGreaterThan : return (response > conditionValue);
            case ConditionType.IntGreaterThanOrEquals: return (response >= conditionValue);
            case ConditionType.IntLessThan : return (response < conditionValue);
            case ConditionType.IntLessThanOrEquals : return (response < conditionValue);
            case ConditionType.ExactMatch : return (response == conditionValue); 
            default : return false;
        }
    }
    public enum ConditionType{
        IntGreaterThan,
        IntGreaterThanOrEquals,
        IntLessThan,
        IntLessThanOrEquals,
        ExactMatch
    }
}