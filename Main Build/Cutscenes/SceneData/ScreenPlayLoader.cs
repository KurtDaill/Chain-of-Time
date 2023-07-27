using System;
using System.IO;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using static CutsceneUtils;

public static class ScreenPlayLoader{
        
    public static void LoadScript(string filePath){
        XmlDocument scriptXML = new XmlDocument();
        scriptXML.PreserveWhitespace = false;
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;

        XmlReader reader = XmlReader.Create(ProjectSettings.GlobalizePath(filePath), settings);

        scriptXML.Load(reader);

        XmlNode screenPlay = scriptXML.FirstChild.NextSibling.NextSibling;
        XmlNode currentBlock = screenPlay.FirstChild;
        List<CutsceneBlock> blocks;
        while(true){ //Reads Blocks
            string name = currentBlock.Attributes.GetNamedItem("name").Value;
            //Get all of the blocks actions into a list
            XmlNode currentAction = currentBlock.FirstChild;
            List<CutsceneAction> actions = new List<CutsceneAction>();
            while(true){
                switch(currentAction.Name){
                    case "line":
                        string speaker = currentAction.FirstChild.InnerText;
                        string text = currentAction.FirstChild.NextSibling.InnerText;

                        List<CutsceneTextEffect> effects = new List<CutsceneTextEffect>();
                        //Index is used track where in the bigger line we are. We pass this data to the Text Effects so they know where to start/stop
                        int index = -1;

                        //The only children Line nodes should have are text effects and text fragments, so this iterates through those.
                        foreach(XmlNode node in currentAction.FirstChild.NextSibling.ChildNodes){ 
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
                        actions.Add(new CutsceneLine(speaker, text, effects.ToArray()));
                        break;

                    case "beat":
                        actions.Add(new CutsceneBeat(Convert.ToDouble(currentAction.InnerText)));
                        break;

                    case "setCharacterAnimation":
                        string character = currentAction.FirstChild.InnerText;
                        string animation = currentAction.FirstChild.NextSibling.InnerText;
                        bool concurency = Convert.ToBoolean(currentAction.Attributes.GetNamedItem("concurent").Value);
                        actions.Add(new CutsceneCharacterAnimation(character, animation, concurency));
                        break;

                    case "setFlag":
                        string flagName = currentAction.Attributes.GetNamedItem("name").Value;
                        bool flagSet = Convert.ToBoolean(currentAction.FirstChild.InnerText);
                        actions.Add(new CutsceneSetStoryFlag(flagName, flagSet));
                        break;
                }
                if(currentAction.NextSibling == null) throw new NotImplementedException(); //We should have an end block, and only reach here if we dont. TODO: Custom Exception.
                currentAction = currentAction.NextSibling;
            }
            if(currentBlock.NextSibling == null) break;
        }   
    }
}