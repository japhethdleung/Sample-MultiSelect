using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample_MultiSelect.Data
{
    public class Team
    {
        public Team()
        {
            this.Players = new List<Player>();
        }

        [Key]
        public Guid TeamId { get; set; }

        [Required]
        [Display(Name = "Team Name")]
        [StringLength(128, ErrorMessage = "Team Name can only be 128 characters in length.")]
        public string Name { get; set; }

        public virtual ICollection<Player> Players { get; set; }
    }
}
