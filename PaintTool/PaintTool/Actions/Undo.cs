using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace PaintTool.Actions
{
    public class Undo
    {
        public Stack<WriteableBitmap> UndoStack { get; set; }
        public WriteableBitmap ImageFromUndoStack { get; set; }        
        public Undo()
        {
            UndoStack = new Stack<WriteableBitmap>();
        }
        
        public void UndoMethod(WriteableBitmap instanceCopy)
        {
            if (UndoStack.Count > 0)            {
                //Redo.PutInRedoStack(instanceCopy);                
                ImageFromUndoStack = UndoStack.Pop();                
            }            
        }

        public void PutInUndoStack(WriteableBitmap instanceCopy)
        {            
            UndoStack.Push(instanceCopy);
        }
    }
}
