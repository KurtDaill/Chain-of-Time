using System;
using System.IO;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public static class ScreenPlayLoader{
        
    public static void LoadScript(string filePath){
        XmlDocument scriptXML = new XmlDocument();
        scriptXML.PreserveWhitespace = false;
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;

        XmlReader reader = XmlReader.Create(ProjectSettings.GlobalizePath(filePath), settings);

        scriptXML.Load(reader);
        XmlNode root = scriptXML.FirstChild;
        XmlNode screenPlay = scriptXML.FirstChild.NextSibling.NextSibling;
    }
}