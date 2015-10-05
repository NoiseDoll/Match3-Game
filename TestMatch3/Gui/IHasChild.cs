namespace Match3.Gui
{
    interface IHasChild
    {
        void AddElement(int id, GuiElement element);
        GuiElement GetElement(int id);
    }
}