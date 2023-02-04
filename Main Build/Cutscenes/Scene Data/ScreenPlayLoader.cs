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

        //Sets up the list so we can reference the written GOTO's to more code-friendlty indexes
        for(int i = 0; i < rawText.Length; i++){
            var textLine = rawText[i].Trim();
            if(textLine.StartsWith('~')){
                textLine = textLine.Remove(0, 1);
                textLine = textLine.Trim();
                exchangeDirectory.Add((textLine, currentDirectoryIndex));
                currentDirectoryIndex++;
            }
        }

        //Look for a line that starts with~
            //Cut that line to just be the text after ~
            //read the next lines:
            //Does
        Queue<string> textQueue = new Queue<string>();
        for(int i = 0; i < rawText.Length; i++){
            while(rawText[i].Contains("\t")){
                rawText[i] = rawText[i].Remove(rawText[i].IndexOf("\t"), 1);
            } 
            textQueue.Enqueue(rawText[i]);
        }

        //TODO Add Parsing Error Exceptions?
        while(textQueue.Count > 0){
            string textLine = textQueue.Dequeue().Trim();
            if(textLine.StartsWith("//")){ // "//" is used for comment lines
                continue;
            }

            if(textLine.Trim().StartsWith('~')){
                textLine = textLine.Remove(0,1);
                string exchangeName = textLine.Trim();
                Queue<Line> exchangeLines = new Queue<Line>();
                while(true){
                    if(textQueue.Peek().StartsWith("~")) break;
                    if(textQueue.Peek() == ""){
                        textQueue.Dequeue();
                        continue;
                    }
                    textLine = textQueue.Dequeue().Trim();
                    if(textLine.StartsWith("[")){ //Brackets indicate action lines
                        if(textLine.StartsWith("[MOD") || textLine.StartsWith("[SET")){
                           exchangeLines.Enqueue(new Line("[ACT]", "[SET]", null, HandleModifier(textLine), null));
                        }
                        else if(textLine.StartsWith("[ANIM")){
                            textLine = textLine.Remove(0, textLine.IndexOf("(") + 1); //Removes "[ANIM("
                            textLine = textLine.Remove(textLine.Length - 2); //Removes the last ")]"
                            textLine = textLine.Trim();
                            exchangeLines.Enqueue(new Line("[ACT]", "[ANIM]", null, null, textLine));
                        }
                        else{
                            throw new NotImplementedException(); //TODO Custom exception, unknown action line in script
                        }
                    }
                    
                    else if(textLine.StartsWith("{")){//Curly Braces indicate Options! These are nessicarily the end of this exchange
                        List<Response> respsonses = new List<Response>();
                        while(true){
                            textLine = textLine.Remove(0, 4).Trim(); //Removes "{OPT"
                            
                            ResponseCondition condition = null;
                            
                            if(textLine.StartsWith("[")){ //Checks for conditions to allow the response (i.e. "{OPT[silverRep > 5]}")


                                //shoud split the key from the operator from the value, i.e. {"silverRep", ">", "5"}
                                string[] conditionData = textLine.Substring(1, textLine.IndexOf("]") - 1).Trim().Split(' ');
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

                            //The Substring call removes the "" from the text.
                            string responseText =  textLine.Substring(1, textLine.Length - 2);
                    
                            respsonses.Add(new Response(responseText, condition, nextExchangeIndex)); 

                            while(textQueue.Peek().Trim().StartsWith("//")){
                                textQueue.Dequeue();
                            }
                            if(textQueue.Peek().Trim().StartsWith("{")){ //If the next line is another option...
                                textLine = textQueue.Dequeue(); //We grab that line and restart the process
                                continue;
                            }
                            //Add Responses to a data structure
                            break;
                        }
                    }
                    
                    else{ //This should be a dialogue line, possibly with textEffects
                        string characterName = textLine.Substring(0, textLine.IndexOf(":"));
                        textLine = textLine.Remove(0, textLine.IndexOf(":") + 1).Trim();
                        var gotoIndex = -1; 
                        if(textLine.Contains("GOTO:")){
                            gotoIndex = ParseGotoStatement(textLine, exchangeDirectory, out var newLine);
                            textLine = newLine;
                        }
                        textLine = textLine.Substring(1, textLine.Length - 3);
                        string finalLine;
                        TextEffect[] effects = ParseLineTextEffects(textLine, out finalLine);
                        exchangeLines.Enqueue(new Line(finalLine, characterName, effects, null, null));
                    }
                }
                //Create an Exchange Object, add it to the list we give the ScreenPlay at the end
                exchanges.Add(new Exchange(exchangeLines));
            } 
        }
        return null;
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

    public static TextEffect[] ParseLineTextEffects(string lineIn, out string lineOut){
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

    public static int ParseGotoStatement(string lineIn, List<(string, int)> directory, out string lineOut){
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
}