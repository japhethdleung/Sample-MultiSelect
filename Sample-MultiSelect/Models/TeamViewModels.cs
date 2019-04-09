using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sample_MultiSelect.Models
{
    public class CreateTeamViewModel
    {
        [Required]
        [Display(Name = "Team Name")]
        [StringLength(128, ErrorMessage = "Team Name can only be 128 characters in length.")]
        public string Name { get; set; }
    }

    public class EditTeamViewModel : CreateTeamViewModel
    {
        public Guid TeamId { get; set; }
    }
}