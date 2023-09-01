namespace Mappr.Controls
{
    public interface IMapObjectsSource
    {
        IEnumerable<IMapObject> GetAll();
    }

}

