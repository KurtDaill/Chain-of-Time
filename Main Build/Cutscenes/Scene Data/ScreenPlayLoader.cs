using System;
using System.IO;
using Godot;
using System.Collections.Generic;
using System.Linq;

public static class ScreenPlayLoader{
    public static ScreenPlay Load(string[] rawText){
        List<(string, int)> exchangeDirectory = new List<(string, int)>();
        List<Exchange> exchanges = new List<Exchange>();
        int currentDirectoryIndex = 0;
        
        for(int i = 0; i < rawText.Length; i++){ //Sets up the list so we can reference the written GOTO's to more code-friendlty indexes
            var textLine = rawText[i].Trim();
            if(textLine.StartsWith('~')){
                textLine = textLine.Remove(0, 1);
                textLine = textLine.Trim();
                exchangeDirectory.Add((textLine, currentDirectoryIndex));
                currentDirectoryIndex++;
            }
        }

        Queue<string> textQueue = new Queue<string>();
        for(int i = 0; i < rawText.Length; i++){ //Removes tab markers and populateas the textQueue with them
            while(rawText[i].Contains("\t")){
                rawText[i] = rawText[i].Remove(rawText[i].IndexOf("\t"), 1);
            } 
            textQueue.Enqueue(rawText[i]);
        }

        while(textQueue.Count > 0){ //Main Loop: goes through each text lines and parses.
            string textLine = textQueue.Dequeue().Trim();
            if(textLine.StartsWith("//") || textLine.StartsWith(" ")){ // "//" is used for comment lines
                continue;
            }

            if(textLine.Trim().StartsWith('~')){ //This is the start of an exchange block, which is processed as a block

                Queue<string> exchangeLines = new Queue<string>();
                while(textQueue.Peek() != ""){
                    exchangeLines.Enqueue(textQueue.Dequeue());
                }
                var newExchange = ParseExchange(exchangeLines, exchangeDirectory); 

                if(newExchange != null) exchanges.Add(newExchange);
            } 
        }
        return new ScreenPlay(exchanges.ToArray());
    }

    public static ScreenplayModifier HandleModifier(string fragment){
        bool isSetter = false;
        if(fragment.StartsWith("[SET")){
            isSetter = true;
        }

        fragment = fragment.Remove(0, fragment.IndexOf("(") + 1); //Removes "[MOD("
        fragment = fragment.Remove(fragment.Length - 2); //Removes the last ")]"
        fragment = fragment.Trim();
        string key = fragment.Substring(0, fragment.IndexOf(","));
        string value = fragment.Substring(fragment.IndexOf(",") + 1).Trim();
        if(value == "TRUE" || value == "FALSE"){
            if(!isSetter) throw new NotImplementedException(); //TODO custom exception, can't modify bools, only SET them
            else{
                var temp = (value == "TRUE");
                return new ScreenplayModifier(false, temp ? 1 : 0, key, isSetter);
                //exchangeLines.Add(new Line("[ACT]", "[SET]", null, new ScreenplayModifier[]{new ScreenplayModifier(true, temp ? 1 : 0, key)}, null));
            }
        }
        else{
            return new ScreenplayModifier(true, value.ToInt(), key, isSetter);
            //exchangeLines.Add(new Line("[ACT]", "[MOD]", null, new ScreenplayModifier[]{new ScreenplayModifier(false, value.ToInt(), key)}, null));
        }
    }

    private static Exchange ParseExchange(Queue<string> rawExchange, List<(string, int)> exchangeDirectory){
        Queue<Line> linesInExchange = new Queue<Line>();
        Response[] responses = null;
        while(rawExchange.Count > 0){ //This block processes the exchange as a unit
            var textLine = rawExchange.Dequeue().Trim();
            if(textLine.StartsWith("[")){ //Brackets indicate action lines
                if(textLine.StartsWith("[MOD") || textLine.StartsWith("[SET")){
                    linesInExchange.Enqueue(new Line("[SET/MOD]", "[ACT]", null, HandleModifier(textLine), null));
                }
                else if(textLine.StartsWith("[ANIM")){
                    textLine = textLine.Remove(0, textLine.IndexOf("(") + 1); //Removes "[ANIM("
                    textLine = textLine.Remove(textLine.Length - 2); //Removes the last ")]"
                    textLine = textLine.Trim();
                    linesInExchange.Enqueue(new Line("[ANIM]", "[ACT]", null, null, textLine));
                }
                else{
                    throw new NotImplementedException(); //TODO Custom exception, unknown action line in script
                }
            }else if(textLine.StartsWith("{")){
                responses = ParseResponses(textLine, rawExchange, exchangeDirectory);
                linesInExchange.Enqueue(new ResponseLine(responses));
                return new Exchange(linesInExchange);
            }else{ //This should be a dialogue line, possibly with textEffects
                string characterName = textLine.Substring(0, textLine.IndexOf(":"));
                textLine = textLine.Remove(0, textLine.IndexOf(":") + 1).Trim();
                var gotoIndex = -1; 
                if(textLine.Contains("GOTO:")){
                    gotoIndex = ParseGotoStatement(textLine, exchangeDirectory, out var newLine);
                    textLine = newLine;
                }
                textLine = textLine.Substring(1, textLine.Length - 2);
                string finalLine;
                TextEffect[] effects = ParseLineTextEffects(textLine, out finalLine);
                linesInExchange.Enqueue(new Line(finalLine, characterName, effects, null, null));
            }
        }
        return new Exchange(linesInExchange);
    }

    private static TextEffect[] ParseLineTextEffects(string lineIn, out string lineOut){
        List<TextEffect>effects = new List<TextEffect>();
        while(lineIn.Contains("[Wave]")){
            effects.Add(new TextEffect(lineIn.IndexOf("[Wave]"), lineIn.IndexOf("[/Wave]"), TextEffectUtilities.TextEffectType.Wave));
            lineIn = lineIn.Remove(lineIn.IndexOf("[Wave]"), 6);
            lineIn = lineIn.Remove(lineIn.IndexOf("[/Wave]"), 7);
        }
        while(lineIn.Contains("[Shake]")){
            effects.Add(new TextEffect(lineIn.IndexOf("[Shake]"), lineIn.IndexOf("[/Shake]"), TextEffectUtilities.TextEffectType.Shake));
            lineIn = lineIn.Remove(lineIn.IndexOf("[Shake]"), 7);
            lineIn = lineIn.Remove(lineIn.IndexOf("[/Shake]"), 8);
        }
        while(lineIn.Contains("[BigShake]")){
            effects.Add(new TextEffect(lineIn.IndexOf("[BigShake]"), lineIn.IndexOf("[/BigShake]"), TextEffectUtilities.TextEffectType.BigShake));
            lineIn = lineIn.Remove(lineIn.IndexOf("[BigShake]"), 10);
            lineIn = lineIn.Remove(lineIn.IndexOf("[/BigShake]"), 11);
        }
        while(lineIn.Contains("[Sparkle]")){
            effects.Add(new TextEffect(lineIn.IndexOf("[Sparkle]"), lineIn.IndexOf("[/Sparkle]"), TextEffectUtilities.TextEffectType.BigShake));
            lineIn = lineIn.Remove(lineIn.IndexOf("[Sparkle]"), 7);
            lineIn = lineIn.Remove(lineIn.IndexOf("[/Sparkle]"), 8);
        }

        lineOut = lineIn;
        return effects.ToArray();
    }

    private static int ParseGotoStatement(string lineIn, List<(string, int)> directory, out string lineOut){
        if(lineIn.Contains("GOTO:")){
            var temp = lineIn.Substring(lineIn.IndexOf("GOTO:")).Trim();
            foreach((string, int) directoryEntry in directory){
                var test = temp.Substring(5).Trim();
                if(directoryEntry.Item1 == test){
                    lineOut = lineIn.Remove(lineIn.IndexOf("GOTO:")).Trim();
                    return directoryEntry.Item2; 
                }
            }
        }
        lineOut = lineIn;
        return -1;
    }

    private static Response[] ParseResponses(string firstLine, Queue<string> responseLines, List<(string, int)> exchangeDirectory){
        List<Response> responses = new List<Response>();
        var textLine = firstLine;
        var tempQueue =  new Queue<string>();
        tempQueue.Enqueue(firstLine);
        foreach(string line in responseLines){ //TODO Make this lest ugly re: having to readd the first line to the queue
            tempQueue.Enqueue(line);
        }
        responseLines = tempQueue;
        while(responseLines.Count > 0){
            textLine = responseLines.Dequeue();
            textLine = textLine.Remove(0, 4).Trim(); //Removes "{OPT"
            
            ResponseCondition condition = null;
            
            if(textLine.StartsWith("[")){ //Checks for conditions to allow the response (i.e. "{OPT[silverRep > 5]}")
                string[] conditionData = textLine.Substring(1, textLine.IndexOf("]") - 1).Trim().Split(' ');//Splits the key from the operator from the value, i.e. {"silverRep", ">", "5"}
                for(int i = 0; i < conditionData.Length; i++) conditionData[i] = conditionData[i].Trim();
                
                if(conditionData[2] == "TRUE" || conditionData[2] == "FALSE"){ // Handle Booleans
                    bool temp = conditionData[2] == "TRUE";
                    if(conditionData[1] != "=") throw new NotImplementedException(); //we don't take greater/less than operators for bools
                    condition = new ResponseCondition(conditionData[0], ResponseCondition.ConditionType.ExactMatch, temp ? 1 :0);
                }
                else{ //Handles Integers
                    ResponseCondition.ConditionType type;
                    switch(conditionData[1]){
                        case "=" :
                            type = ResponseCondition.ConditionType.ExactMatch;
                            break;
                        case ">" :
                            type = ResponseCondition.ConditionType.IntGreaterThan;
                            break;
                        case ">=" :
                            type = ResponseCondition.ConditionType.IntGreaterThanOrEquals;
                            break;
                        case "<" :
                            type = ResponseCondition.ConditionType.IntLessThan;
                            break;
                        case "<=" :
                            type = ResponseCondition.ConditionType.IntLessThanOrEquals;
                            break;
                        default :
                            throw new NotImplementedException(); //TODO custom exception, conditions operator not recognized
                    }
                    condition = new ResponseCondition(conditionData[0], type, conditionData[2].ToInt());
                }
                textLine = textLine.Remove(0, textLine.IndexOf("]")+1);
            }
            textLine = textLine.Remove(0,1); //Removes "}"

            int nextExchangeIndex = -1;
            if(textLine.Contains("GOTO:")){
                nextExchangeIndex = ParseGotoStatement(textLine, exchangeDirectory, out textLine);
            }

            //The Substring call removes the extra "" from the text.
            string responseText =  textLine.Substring(1, textLine.Length - 1);
    
            responses.Add(new Response(responseText, condition, nextExchangeIndex)); 

            while(responseLines.Count > 0 && responseLines.Peek().Trim().StartsWith("//")){//Chews through any comments inside of the options
                responseLines.Dequeue();
            }
        }
        return responses.ToArray();
    }
}