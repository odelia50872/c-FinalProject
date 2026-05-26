using GatherUp.core.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Infrastructure.Memory
{
    public class MemoryRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly List<T> _data = new List<T>();
        public void Add(T entity)
        {
            _data.Add(entity);
        }

        public T GetById(int id)
        {
            return _data.FirstOrDefault(e => e.Id == id)!;
        }

        public IEnumerable<T> GetAll()
        {
            return _data;
        }

        public void Update(T entity)
        {
            var existing = GetById(entity.Id);
            if (existing != null)
            {
                var index = _data.IndexOf(existing);
                _data[index] = entity;
            }
        }

        public void Delete(int id)
        {
            var existing = GetById(id);
            if (existing != null)
            {
                _data.Remove(existing);

            }
        }
    }
}


