using System.ComponentModel.DataAnnotations;

namespace marketControlSpamers.Models
{
    public class Neighborhood
    {
        [Key]
        public long idNeighborhood { get; set; }
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        [Required]
        public long idTownship { get; set; }
        public int? PostalCode { get; set; }
        public bool Active { get; set; }
        public DateTime CreationDate { get; set; }
        public long CreationUser { get; set; }
        public DateTime? EditDate { get; set; }
        public long EditUser { get; set; }
        public DateTime? DeleteDate { get; set; }
        public long DeleteUser { get; set; }
        public bool Deleted { get; set; }

        public Township Township { get; set; }
    }
}
