using System;
using System.Collections.Generic;
public class ScreenPlay{
    Dictionary<string, CutsceneBlock> blocks;

    public ScreenPlay(List<CutsceneBlock> blocks){
        this.blocks = new Dictionary<string, CutsceneBlock>();
        foreach(CutsceneBlock block in blocks){
            this.blocks.Add(block.GetName(), block);
        }
        if(!this.blocks.ContainsKey("start")) throw new ArgumentException("Blocks must contain a block labeled 'Start'");
    }

    public CutsceneBlock Start(){
        if(blocks.TryGetValue("start", out CutsceneBlock startBlock)){
            return startBlock;
        }else{
            throw new ArgumentException(); //TODO custom exception
        }
    }

    public bool TryGetBlock(string name, out CutsceneBlock newBlock){
        if(blocks.ContainsKey(name)){
            newBlock = blocks.GetValueOrDefault(name);
            return true;
        }else{
            newBlock = null;
            return false;
        }
    }
}