using System.Numerics;

namespace Mappr.Misc
{
    public class ContextMenuManager
    {
        ContextMenuManager<object> menuManager;
        public ContextMenuManager(Control parentControl)
        {
            menuManager = new ContextMenuManager<object>(parentControl);
        }

        public void AddMenuItem(string menuPath, Action onClick)
        {
            menuManager.AddMenuItem(menuPath, (o) => { onClick.Invoke(); });
        }

        public void ShowMenu(Vector2 pos)
        {
            menuManager.ShowMenu(pos, null);
        }
    }

}
