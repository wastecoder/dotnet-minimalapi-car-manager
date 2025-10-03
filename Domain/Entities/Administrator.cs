using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarManager.Domain.Entities;

public class Administrator
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(50)]
    public string Password { get; set; }

    [Required]
    [StringLength(20)]
    public string Role { get; set; }
}
