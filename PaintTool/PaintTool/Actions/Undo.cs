using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace PaintTool.Actions
{
    public class Undo
    {
        Stack<WriteableBitmap> undoStack;
        WriteableBitmap ImageFromUndoStack { get; set; }
        Redo newRedo = new Redo();
        public Undo()
        {
            undoStack = new Stack<WriteableBitmap>();
        }

        public Stack<WriteableBitmap> UndoStack
        {
            get
            {
                return undoStack;
            }
        }

        public void Undomethod(WriteableBitmap instanceCopy)
        {
            if (undoStack.Count > 0)
            {
                newRedo.PutInRedoStack(instanceCopy);                
                ImageFromUndoStack = undoStack.Pop();
            }
            else if (undoStack.Count == 0)
            {
                return;
            }
        }

        public void PutInUndoStack(WriteableBitmap instanceCopy)
        {            
            undoStack.Push(instanceCopy);
        }
    }
}
