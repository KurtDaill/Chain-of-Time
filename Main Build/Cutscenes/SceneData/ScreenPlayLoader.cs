using System;
using System.IO;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public static class ScreenPlayLoader{
        
    public static void LoadScript(string filePath){
        XmlDocument scriptXML = new XmlDocument();
        scriptXML.PreserveWhitespace = true;
        XmlReaderSettings settings = new XmlReaderSettings();
        scriptXML.Load(ProjectSettings.GlobalizePath(filePath));
        XmlNode root = scriptXML.FirstChild;
    }
}