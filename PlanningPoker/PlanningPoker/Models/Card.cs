using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PlanningPoker.Models
{
    public class Card
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}