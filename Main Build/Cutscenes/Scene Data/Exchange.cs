using System;
using System.Collections.Generic;

public class Exchange{
    Queue<Line> lines;

    public Exchange(Queue<Line> lines){
        this.lines = lines;
    }

    public Line GetNextLine(){
        if(lines.Count == 0) return null;
        return lines.Dequeue();
    }

    public Line PeekNextLine(){
        return lines.Peek();
    }
}