using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace PaintTool.Actions
{
    public class Redo
    {
        Stack<WriteableBitmap> redoStack;
        WriteableBitmap ImageFromRedoStack { get; set; }

        public Redo()
        {
            redoStack = new Stack<WriteableBitmap>();
        }

        public void RedoMethod()
        {
            if (redoStack.Count > 0)
            {                
                ImageFromRedoStack = redoStack.Pop();
            }
            else if (redoStack.Count == 0)
            {
                return;
            }
        }

        public void PutInRedoStack(WriteableBitmap instanceCopy)
        {            
            redoStack.Push(instanceCopy);
        }
    }
}
