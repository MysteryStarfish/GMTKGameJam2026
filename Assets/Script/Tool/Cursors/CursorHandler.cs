using UnityEngine;

namespace Script.Tool.Cursors
{
    public class CursorHandler
    {
        private Texture2D handCursor;
        private Vector2 hotSpot = Vector2.zero;
        
        public CursorHandler(Texture2D  handCursor, Vector2 hotSpot)
        {
            this.handCursor = handCursor;
            this.hotSpot = hotSpot;
            ResetCursor();
        }
        
        public CursorHandler(Texture2D handCursor)
            : this(handCursor, new Vector2(handCursor.width / 2f, handCursor.height / 2f))
        {
            ;
        }

        public void SetNewCursor(Texture2D texture)
        {
            Vector2 customHotSpot = new Vector2(texture.width / 2f, texture.height / 2f);
            Cursor.SetCursor(texture, customHotSpot, CursorMode.Auto);
        }
        
        public void SetNewCursor(Texture2D texture, Vector2 hotSpotPosition)
        {
            Vector2 customHotSpot = new Vector2(texture.width / 2f, texture.height / 2f);
            Cursor.SetCursor(texture, hotSpotPosition, CursorMode.Auto);
        }

        public void ResetCursor()
        {
            Cursor.SetCursor(handCursor, hotSpot, CursorMode.Auto);
        }

        public void ResetToDefault()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}