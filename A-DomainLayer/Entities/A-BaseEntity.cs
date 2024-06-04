using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A_DomainLayer.Entities;

public class BaseEntity
{
    
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("_created_at")]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    
    [Required]
    [Column("_updated_at")]
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;

}