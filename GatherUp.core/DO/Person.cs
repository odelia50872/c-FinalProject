using GatherUp.core.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.core.DO
{
    public abstract class Person : IEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; init; }
        public required string PhoneNumber { get; init; }
        int IEntity.Id
        {
            get => Id;
            set => throw new NotSupportedException("Id is init-only for Person.");
        }
    }

}
