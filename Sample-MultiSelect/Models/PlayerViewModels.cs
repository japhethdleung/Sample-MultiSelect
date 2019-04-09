using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sample_MultiSelect.Models
{
    public class CreatePlayerViewModel
    {
        [Required]
        [Display(Name = "Player Name")]
        [StringLength(128, ErrorMessage = "Players name can only be 128 characters in length.")]
        public string Name { get; set; }

        public List<string> TeamIds { get; set; }

        [Display(Name = "Teams")]
        public MultiSelectList Teams { get; set; }
    }

    // Model inherits the same properties from CreatePlayerViewModel, with addition of Id
    public class EditPlayerViewModel : CreatePlayerViewModel
    {
        // Note that this is a string - not a Guid. We can convert this to a Guid in the controller
        public string PlayerId { get; set; }
    }
}