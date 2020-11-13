using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Error
{
    public class ErrorModel
    {
        public string ExceptionId { get; set; }

        [Required(ErrorMessage = "The description is required if you want to submit more information")]
        [MaxLength(2000)]
        public string Description { get; set; }
    }
}
