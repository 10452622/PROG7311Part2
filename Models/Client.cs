using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PROG7311Part2.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ContactDetails { get; set; }

        [Required]
        public string Region { get; set; }

        [ValidateNever]
        public ICollection<Contract>? Contracts { get; set; }
    }
}
