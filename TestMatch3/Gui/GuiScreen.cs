using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Match3.Gui
{
    abstract class GuiScreen : IHasChild
    {
        public string type;
        protected Match3Game game;
        protected Dictionary<int, GuiElement> childElements;

        protected GuiScreen(Match3Game game)
        {
            this.game = game;
            childElements = new Dictionary<int, GuiElement>();
        }

        internal virtual void LoadContent() { }
        internal virtual void UnloadContent() { }
        internal virtual void Update(GameTime gameTime) { }
        internal virtual void Draw() { }

        public void AddElement(int id, GuiElement element)
        {
            childElements.Add(id, element);
        }

        public GuiElement GetElement(int id)
        {
            return childElements[id];
        }
    }
}
