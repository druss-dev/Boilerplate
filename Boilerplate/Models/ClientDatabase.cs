using System.ComponentModel.DataAnnotations.Schema;

namespace Boilerplate.Models;

public class ClientDatabase
{
    public int Id { get; set; }
    
    [Column("CUID")]
    public string? Cuid { get; set; }
    public string? DatabaseName { get; set; }
}