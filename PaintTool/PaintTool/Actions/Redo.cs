using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace PaintTool.Actions
{
    public class Redo
    {
        public static Stack<WriteableBitmap> RedoStack { get; set; }
        public WriteableBitmap ImageFromRedoStack { get; set; }

        public Redo()
        {
            RedoStack = new Stack<WriteableBitmap>();
        }

        public void RedoMethod()
        {
            if (RedoStack.Count > 0)
            {                
                ImageFromRedoStack = RedoStack.Pop();
            }           
        }
        public void PutInRedoStack(WriteableBitmap instanceCopy)
        {            
            RedoStack.Push(instanceCopy);
        }
        public void ClearStack()
        {
            RedoStack.Clear();
        }
    }
}
