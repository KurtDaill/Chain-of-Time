using System;
using Godot;

public class ResponseLine : Line{
    protected Response[] responses;

    public Response[] GetResponses(){
            return responses;
    }

    public ResponseLine(Response[] responses){
        this.text = "{OPT}";
        this.speaker = "{OPT}";
        this.effects = null;
        this.mod = null;
        this.animation = null;
        this.gotoIndex = -1;
        this.responses = responses;
    }
}