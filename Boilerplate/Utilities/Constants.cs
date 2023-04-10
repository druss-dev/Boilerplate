namespace Boilerplate.Utilities;

public static class ServiceType
{
    public static readonly string PatientService = "Boilerplate.Services.Patient.PatientService";
    public static readonly string ThingService = "Boilerplate.Services.Thing.ThingService";
}

public static class StoredProcedure
{
    public static readonly string Digestive = "[Campaigns].[proc_GetServiceLineGastroenterology]";
    public static readonly string Heart = "[Campaigns].[proc_GetServiceLineHeart]";
    public static readonly string Lung = "[Campaigns].[proc_GetServiceLineLung]";
    public static readonly string Orthopedic = "[Campaigns].[proc_GetServiceLineOrthopedics]";
}