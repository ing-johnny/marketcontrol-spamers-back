using System.ComponentModel.DataAnnotations;

namespace marketControlSpamers.Models
{
    public class FederalEntity
    {
        [Key]
        public long idFederalEntity { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime CreationDate { get; set; }
        public long CreationUser { get; set; }
        public DateTime? EditDate { get; set; }
        public long EditUser { get; set; }
        public DateTime? DeleteDate { get; set; }
        public long DeleteUser { get; set; }
        public bool Deleted { get; set; }
    }
}
