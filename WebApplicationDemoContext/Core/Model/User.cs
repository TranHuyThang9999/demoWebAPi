using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationDemoContext.core.Model;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)] 
    public int Id { get; set; }
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateOnly Created { get; set; }
    public DateTime Updated { get; set; }
}