using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Domain.IdentityModels
{
    public class RevokedToken
    {
        [Key]
        [Required]
        public string? JwtTokenDigest { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RevokedAt { get; set; } = DateTime.Now;
    }
}
