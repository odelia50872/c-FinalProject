using GatherUp.core.interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GatherUp.core.DO
{
    public abstract class Person : IEntity
    {
        public Person() { }
        public Person(string name, string email, string phoneNumber)
        {
            Name = name;
            Email = email ?? throw new ArgumentNullException(nameof(email), "אימייל לא יכול להיות ריק");
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber), "טלפון לא יכול להיות ריק");
        }
        [XmlAttribute]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        public  string PhoneNumber { get; set; } = string.Empty;

    }

}
