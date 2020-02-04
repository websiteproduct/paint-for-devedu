using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace PaintTool.Actions
{
    public class Undo
    {
        public static Stack<WriteableBitmap> UndoStack { get; set; }
        public WriteableBitmap ImageFromUndoStack { get; set; }        
        public Undo()
        {
            UndoStack = new Stack<WriteableBitmap>();
        }
        
        public void UndoMethod()
        {
            if (UndoStack.Count > 0)            
            {
                Redo.RedoStack.Push(NewImage.GetInstanceCopy());                
                ImageFromUndoStack = UndoStack.Pop();                
            }            
        }

        public void PutInUndoStack(WriteableBitmap instanceCopy)
        {            
            UndoStack.Push(instanceCopy);
        }

        public void ClearStack()
        {
            UndoStack.Clear();
        }
    }
}
