using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace PaintTool.Actions
{
    public class Redo
    {
        Stack<WriteableBitmap> redoStack;

        public Redo()
        {
            redoStack = new Stack<WriteableBitmap>();
        }

        public void Redomethod()
        {
            if (redoStack.Count > 0)
            {
                wb = redoStack.Pop();
                PaintField.Source = wb;
            }
            else if (undoStack.Count == 0)
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
