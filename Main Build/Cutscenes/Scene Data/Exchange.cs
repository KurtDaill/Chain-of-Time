using System;
using System.Collections.Generic;

public class Exchange{
    Queue<Line> lines;

    public Exchange(Queue<Line> lines){
        this.lines = lines;
    }

    public Line GetNextLine(){
        return lines.Dequeue();
    }

    public Line PeekNextLine(){
        return lines.Peek();
    }
}