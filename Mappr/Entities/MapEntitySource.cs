using Mappr.Controls;

namespace Mappr.Entities
{
    public class MapEntitySource
    {
        private List<MapEntity> entities = new List<MapEntity>();

        public IEnumerable<IDrawable> GetDrawables() => entities;

        public IEnumerable<MapEntity> GetEntities() => entities;


        public void Add(MapEntity entity) => entities.Add(entity);
    }







}
