using GatherUp.core.interfaces;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace GatherUp.core.DO
{
    public abstract class Person : IEntity
    {
        public Person() { }

        [XmlAttribute]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; } = string.Empty;

        // Password = ID number, used for authentication
        public string Password { get; set; } = string.Empty;
    }
}
