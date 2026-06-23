using GatherUp.core.interfaces;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace GatherUp.core.DO
{
    public class Participant : IEntity
    {
        public Participant() { }

        private int _id;
        [XmlAttribute]
        public int Id { get => _id; set { if (_id == 0) _id = value; } }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public MailingPreference MailingPreferences { get; set; }
        public UserRole Role { get; set; } = UserRole.Participant;
    }
}
