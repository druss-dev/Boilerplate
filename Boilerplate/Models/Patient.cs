using System.Text.Json.Serialization;
using Boilerplate.Utilities;

namespace Boilerplate.Models;

public class PatientRecord
{
    public string? PatientNumber { get; set; }
    
    public string? Name { get; set; }

    public DateTime DateOfBirth { get; set; }
    
    public string? Sex { get; set; }

    public string? MedicalRecordNumber { get; set; }

    public string? PrimaryLanguage { get; set; }
    
    public string? HomeAreaCode { get; set; }

    public string? HomePhoneNumber { get; set; }
    
    public string? CellAreaCode { get; set; }

    public string? CellPhoneNumber { get; set; }

    public string? EmailAddress { get; set; }
    
    public string? ZipCode { get; set; }
}

public class PatientFile
{
    public string? ClientId { get; set; }

    public int? MarketId { get; set; }

    public int? ImportId { get; set; }

    public Enumerations.ServiceLineSegmentation SegmentationGrouping { get; set; }

    public string? FileIdentifier { get; set; }
    
    public DateTimeOffset? StartDate { get; set; }
    
    public DateTimeOffset? EndDate { get; set; }
}