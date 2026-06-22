using GatherUp.core.interfaces;
using GatherUp.Infrastructure.XML;
using System.Collections.Generic;
using System.Linq;

namespace GatherUp.Infrastructure.Repositories
{
    public class XmlRepository<T> : IRepository<T> where T : class, IEntity, new()
    {
        private readonly string _filePath;

        public XmlRepository(string folderPath)
        {
            _filePath = Path.Combine(folderPath, $"{typeof(T).Name}.xml");
        }

        private List<T> LoadAll() =>
            File.Exists(_filePath) ? XMLSerializer.LoadFromXML<List<T>>(_filePath) : new List<T>();

        private void SaveAll(List<T> data) =>
            XMLSerializer.SaveToXML(_filePath, data);

        public virtual void Add(T entity)
        {
            var list = LoadAll();
            list.Add(entity);
            SaveAll(list);
        }

        public T GetById(int id) => LoadAll().FirstOrDefault(e => e.Id == id)!;

        public IEnumerable<T> GetAll() => LoadAll();

        public virtual void Update(T entity)
        {
            var list = LoadAll();
            var index = list.FindIndex(e => e.Id == entity.Id);
            if (index >= 0)
            {
                list[index] = entity;
                SaveAll(list);
            }
        }

        public virtual void Delete(int id)
        {
            var list = LoadAll();
            var item = list.FirstOrDefault(e => e.Id == id);
            if (item == null)
                throw new GatherUp.core.Exceptions.EntityNotFoundException(typeof(T).Name, id);
            list.Remove(item);
            SaveAll(list);
        }
    }
}
