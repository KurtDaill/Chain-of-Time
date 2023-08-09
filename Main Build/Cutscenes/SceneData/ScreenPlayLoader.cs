using System;
using System.IO;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using static CutsceneUtils;

public static class ScreenPlayLoader{
        
    public static ScreenPlay LoadScript(string filePath){
        XmlDocument scriptXML = new XmlDocument();
        scriptXML.PreserveWhitespace = false;
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;

        XmlReader reader = XmlReader.Create(ProjectSettings.GlobalizePath(filePath), settings);

        scriptXML.Load(reader);

        XmlNode screenPlay = scriptXML.FirstChild.NextSibling.NextSibling;
        XmlNode currentBlock = screenPlay.FirstChild;
        List<CutsceneBlock> blocks = new List<CutsceneBlock>();
        while(true){ //Reads Blocks
            string name = currentBlock.Attributes.GetNamedItem("name").Value;
            //Get all of the blocks actions into a list
            XmlNode currentAction = currentBlock.FirstChild;
            List<CutsceneAction> actions = new List<CutsceneAction>();
            bool endBlock = false;
            while(endBlock == false){
                switch(currentAction.Name){
                    case "line":
                        actions.Add(GenerateLine(currentAction));
                        break;

                    case "beat":
                        actions.Add(new CutsceneBeat(Convert.ToDouble(currentAction.InnerText)));
                        break;

                    case "setCharacterAnimation":
                        //If this function returns a line object...
                        if(GenerateCharacterAnimationOrLineWithAnimation(currentAction, out CutsceneAction returnedAction)){
                            currentAction = currentAction.NextSibling; //we just inputted the next line into the block, so we need to manually skip past it here
                        }
                        actions.Add(returnedAction);
                        break;

                    case "setFlag":
                        string flagName = currentAction.Attributes.GetNamedItem("name").Value;
                        bool flagSet = Convert.ToBoolean(currentAction.FirstChild.InnerText);
                        actions.Add(new CutsceneSetStoryFlag(flagName, flagSet));
                        break;
                    
                    case "modValue" :
                        string modName = currentAction.Attributes.GetNamedItem("name").Value;
                        int valueMod = Convert.ToInt32(currentAction.FirstChild.InnerText);
                        actions.Add(new CutsceneModStoryValue(modName, valueMod));
                        break;
                    
                    case "setValue" :
                        string setName = currentAction.Attributes.GetNamedItem("name").Value;
                        int valueSet = Convert.ToInt32(currentAction.FirstChild.InnerText);
                        actions.Add(new CutsceneSetStoryValue(setName, valueSet));
                        break;
                    case "cameraMove" :
                        string targetShot = currentAction.InnerText;
                        string transitionType = currentAction.Attributes.GetNamedItem("transition").Value;
                        double transitionLength;
                        if(currentAction.Attributes.GetNamedItem("transitionLengthInSeconds") == null) transitionLength = 1; //TODO firgure out how to properly iteract with XML default values
                        else transitionLength = Convert.ToDouble(currentAction.Attributes.GetNamedItem("transitionLengthInSeconds").Value);
                        actions.Add(new CutsceneCameraMove(targetShot, transitionType, transitionLength));
                        break;
                    case "characterMove" :
                        string characterName = currentAction.Attributes.GetNamedItem("character").Value;
                        string blockingMarkerName = currentAction.InnerText;
                        actions.Add(new CutsceneCharacterMove(characterName, blockingMarkerName));
                        break;
                    case "envAnimation" :
                        string environmentAnimationName = currentAction.InnerText;
                        actions.Add(new CutsceneEnvironmentAnimation(environmentAnimationName));
                        break;
                    case "smashCut" :
                        XmlNode smashCutElementHead = currentAction.FirstChild;
                        List<CutsceneCharacterMove> characterTeleports = new List<CutsceneCharacterMove>();
                        List<CutsceneCharacterAnimation> animationChanges = new List<CutsceneCharacterAnimation>();
                        List<string> showMeNodes = new List<string>();
                        List<string> hideMeNodes = new List<string>();
                        CutsceneCameraMove cameraMove = null;
                        while(smashCutElementHead != null){
                            switch(smashCutElementHead.Name){
                                case "characterAnimationChange" : 
                                    if(GenerateCharacterAnimationOrLineWithAnimation(smashCutElementHead, out CutsceneAction generatedAnimation)){
                                        throw new ArgumentException("Cannot place animations concurrent with lines in a smash cut!");
                                    }
                                    animationChanges.Add((CutsceneCharacterAnimation)generatedAnimation);
                                    break;
                                case "characterTeleport" :
                                    string smashCutMoveCharacterName = smashCutElementHead.Attributes.GetNamedItem("character").Value;
                                    string smashCutMoveBlockingMarkerName = smashCutElementHead.InnerText;
                                    characterTeleports.Add(new CutsceneCharacterMove(smashCutMoveCharacterName, smashCutMoveBlockingMarkerName));
                                    break;
                                case "showNode" :
                                    showMeNodes.Add(smashCutElementHead.InnerText); break;
                                case "hideNode" :
                                    hideMeNodes.Add(smashCutElementHead.InnerText); break;
                                case "cameraCut" :
                                    cameraMove = new CutsceneCameraMove(smashCutElementHead.InnerText, "cut", 0); break;
                            }
                            smashCutElementHead = smashCutElementHead.NextSibling;
                        }
                        actions.Add(new CutsceneSmashCut(cameraMove, characterTeleports, animationChanges, showMeNodes, hideMeNodes));
                        break;
                    case "END":
                        List<CutsceneDialogueResponse> responses = new List<CutsceneDialogueResponse>();
                        foreach(XmlNode node in currentAction.ChildNodes){ //Iterates through all of the dialgoue options
                            bool finished = false;
                            switch(node.Name){
                                case "GOTO": //If we have a GOTO block, that takes precedence over any opts in the same block, so we bypass them using finsihed
                                    responses = null;
                                    actions.Add(new CutsceneEndBlock(new CutsceneGoToBlock(node.FirstChild.InnerText)));
                                    finished = true;
                                    break;
                                case "opt": //This is a text effect, we pull out which kind it is from the XML name and save it.
                                    responses.Add(new CutsceneDialogueResponse(node.FirstChild.InnerText, node.Attributes.GetNamedItem("GOTO").Value));
                                    break;
                                default:
                                    throw new NotImplementedException(); //TODO custom exception for broken end blocks
                            }
                            if(finished) break;
                        }
                        if(responses != null && responses.Count() > 0) actions.Add(new CutsceneEndBlock(responses.ToArray()));
                        endBlock = true;
                        break;
                }
                if(endBlock) break;
                if(currentAction.NextSibling == null) throw new NotImplementedException(); //We should have an end block, and only reach here if we dont. TODO: Custom Exception.
                currentAction = currentAction.NextSibling;
            }
            blocks.Add(new CutsceneBlock(currentBlock.Attributes.GetNamedItem("name").Value, actions.ToArray()));
            if(currentBlock.NextSibling == null) break;
        }
        return new ScreenPlay(blocks);
    }

    private static CutsceneLine GenerateLine(XmlNode lineNode, CutsceneCharacterAnimation anim = null){
        if(lineNode.Name != "line") throw new ArgumentException(); 
        string speaker = lineNode.FirstChild.InnerText;
        string text = lineNode.FirstChild.NextSibling.InnerText;
        List<CutsceneTextEffect> effects = new List<CutsceneTextEffect>();
        //Index is used track where in the bigger line we are. We pass this data to the Text Effects so they know where to start/stop
        int index = -1;

        //The only children Line nodes should have are text effects and text fragments, so this iterates through those.
        foreach(XmlNode node in lineNode.FirstChild.NextSibling.ChildNodes){ 
            switch(node.Name){ //If we find a text fragement, we just note it's length for indexing.
                case "#text":
                    index += node.InnerText.Length;
                    break;
                default: //This is a text effect, we pull out which kind it is from the XML name and save it.
                    int start = index;
                    int end = index + node.InnerText.Length;
                    effects.Add(new CutsceneTextEffect(start, end, node.Name));
                    break;
            }
        }
        bool hasAnim = (anim != null);
        return new CutsceneLine(speaker, text, effects.ToArray(), hasAnim, anim);
    }

    //returns whether or not it returned a line
    private static bool GenerateCharacterAnimationOrLineWithAnimation(XmlNode animationNode, out CutsceneAction resultingAction){
        string character = animationNode.FirstChild.InnerText;
        string animation = animationNode.FirstChild.NextSibling.InnerText;
        bool concurency = false;
        if(animationNode.Attributes.GetNamedItem("concurent") != null)concurency = Convert.ToBoolean(animationNode.Attributes.GetNamedItem("concurent").Value);
        bool idleLoop = false;
        if(animationNode.Attributes.GetNamedItem("idleLoop") != null)idleLoop = Convert.ToBoolean(animationNode.Attributes.GetNamedItem("idleLoop").Value);
        if(concurency){
            if(animationNode.NextSibling.Name != "line") throw new ArgumentException();
            resultingAction =  GenerateLine(animationNode.NextSibling, new CutsceneCharacterAnimation(character, animation, concurency, idleLoop));
            return true;
        }else{
            resultingAction = new CutsceneCharacterAnimation(character, animation, concurency, idleLoop);
            return false;
        }
    }
}