using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample_MultiSelect.Data
{
    public class Player
    {
        public Player()
        {
            this.Teams = new List<Team>();
        }

        [Key]
        public Guid PlayerId { get; set; }

        [Required]
        [Display(Name = "Player Name")]
        [StringLength(128, ErrorMessage = "Player's name can only be 128 characters in length.")]
        public string Name { get; set; }

        public virtual ICollection<Team> Teams { get; set; }
    }
}
