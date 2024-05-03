using Mappr.Extentions;
using System.Numerics;

namespace Mappr.Controls
{
    public class ContextMenuManager<T>
    {
        private readonly Control parentControl;
        private readonly ContextMenuStrip contextMenu;
        private T? data;
        public ContextMenuManager(Control parentControl)
        {
            this.parentControl = parentControl;
            contextMenu = new ContextMenuStrip();

        }

        public virtual void AddMenuItem(string menuPath, Action<T?> onClick)
        {
            string[] split = menuPath.Split('/');

            ToolStripMenuItem item = null;


            if (contextMenu.Items[split[0]] is ToolStripMenuItem tsi)
                item = tsi;
            else
            {
                item = new ToolStripMenuItem(split[0]);
                item.Name = split[0];
                contextMenu.Items.Add(item);
            }

            for (int i = 1; i < split.Length; i++)
            {
                string name = split[i];

                if (item.DropDownItems[name] is ToolStripMenuItem tsii)
                    item = tsii;
                else
                {
                    ToolStripMenuItem newItem = new ToolStripMenuItem(name);
                    newItem.Name = name;
                    item.DropDownItems.Add(newItem);
                    item = newItem;
                }
            }



            if (onClick != null)
                item.Click += (a, b) => onClick.Invoke(data);
        }


        public virtual void ShowMenu(Vector2 pos, T? data)
        {
            this.data = data;
            contextMenu.Show(parentControl, pos.ToPoint());
        }
    }

}
