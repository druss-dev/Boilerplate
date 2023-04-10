using Boilerplate.Models;
using Boilerplate.Repositories.Patient;
using Boilerplate.Utilities;
using Microsoft.Extensions.Logging;
using PatientApiService;
using PatientApi = PatientApiService.Models;

namespace Boilerplate.Services.Patient;

public class PatientService : IPatientService
{
    private readonly IPatientApiClient _patientApiClient;
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger, IPatientApiClient patientApiClient)
    {
        _patientRepository = patientRepository;
        _logger = logger;
        _patientApiClient = patientApiClient;
    }

    public async Task ProcessPatientsAsync(PatientFile patientFile)
    {
        _logger.LogInformation("Processing patient file");

        // fetch patient info from patient database
        var patientRecords = await GetPatientRecordsAsync(patientFile);
        if (patientRecords.Count == 0)
        {
            _logger.LogInformation("No patients received from patient database");
            return;
        }

        patientRecords = patientRecords.Take(1000).ToList();
        
        _logger.LogInformation("Processing {PatientRecordsCount} patient records from the patient database",  patientRecords.Count);
        var processedPatientCount = 0;
        var successfulPatientCount = 0;
        
        // iterate through each patient record, upsert to patient api, and produce a topic for each
        await Task.WhenAll(patientRecords
            .Select(patientRecord => Task.Run(() =>
                    CreatePatientAsync(patientRecord, patientFile, processedPatientCount, successfulPatientCount))));
        
        
        // foreach(var patientRecord in patientRecords)
        // {
        //     await CreatePatientAsync(patientRecord, patientFile, processedPatientCount, successfulPatientCount);
        // }

        _logger.LogInformation("Successfully processed {SuccessfulPatientCount} of {ProcessedPatientCount}", 
            successfulPatientCount, processedPatientCount);
    }

    private async Task CreatePatientAsync (PatientRecord patientRecord, PatientFile patientFile, int processedPatientCount, int successfulPatientCount)
    {
        try
        {
            // map patient record model to patient model
            var patient = MapPatientRecordToPatient(patientRecord);
                
            // call patient api service to upsert and and retrieve patient information
            patient = await UpsertPatientAsync(patientFile.ClientId!, patient);

            _logger.LogInformation("Successfully processed patient record {PatientRecord} with patient id {PatientId}", 
                processedPatientCount, patient?.Id);
            processedPatientCount++;
            successfulPatientCount++;
        }
        catch (Exception ex)
        {
            // log exception and continue processing
            _logger.LogError(ex, "Something went wrong creating context for the patient record {PatientRecord}", 
                processedPatientCount);
            processedPatientCount++;
        }
    }

    private async Task<List<PatientRecord>> GetPatientRecordsAsync(PatientFile patientFile)
    {
        // sets the start date as the first day of the month two months prior, and the end date as the last day of that month
        var twoMonthsPriorDate = DateTime.UtcNow.AddMonths(-3);

        patientFile.StartDate = new DateTime(twoMonthsPriorDate.Year, twoMonthsPriorDate.Month, 1);
        patientFile.EndDate = patientFile.StartDate.Value.AddMonths(1).AddTicks(-1);
        
        _logger.LogInformation("Retrieving patient records from {StartDate} to {EndDate}", patientFile.StartDate, patientFile.EndDate);
        switch (patientFile.SegmentationGrouping)
        {
            case Enumerations.ServiceLineSegmentation.Digestive:
                return await _patientRepository.GetDigestivePatientRecords(patientFile, StoredProcedure.Digestive);
                
            case Enumerations.ServiceLineSegmentation.Heart:
                return await _patientRepository.GetHeartPatientRecords(patientFile, StoredProcedure.Heart);
                
            case Enumerations.ServiceLineSegmentation.Lung:
                return await _patientRepository.GetLungPatientRecords(patientFile, StoredProcedure.Lung);
                
            case Enumerations.ServiceLineSegmentation.Orthopedic:
                // retrieve cardiac and orthopedic patients, filter out the any medical record numbers from orthopedic that exist in the cardiac
                var cardiacMedicalRecordNumberHash = await GetCardiacMedicalRecordNumbers(patientFile);
                var orthopedicPatients = await _patientRepository.GetOrthopedicPatientRecords(patientFile, StoredProcedure.Orthopedic);
                var filteredOrthopedicPatients = orthopedicPatients
                    .Where(x => !cardiacMedicalRecordNumberHash.Contains(x.MedicalRecordNumber))
                    .ToList();
                
                return filteredOrthopedicPatients;
        }
        
        _logger.LogError("No service line segmentation specified");
        throw new Exception($"No service line segmentation specified");
    }
    
    private async Task<PatientApi.Patient?> UpsertPatientAsync(string clientId, PatientApi.Patient patient)
    {
        _logger.LogInformation("Upserting patient record to the patient database");
        var patientGuid = await _patientApiClient.UpsertPatientAsync(clientId, patient);

        if (patientGuid is null)
        {
            _logger.LogError("Patient guid cannot be null");
            throw new Exception("Patient guid cannot be null");
        }

        patient.Id = patientGuid.Value;
        _logger.LogInformation("Successfully upserted patient {PatientId} to the patient database", patientGuid);
        
        return patient;
    }

    private async Task<HashSet<string?>> GetCardiacMedicalRecordNumbers(PatientFile patientFile)
    {
        _logger.LogInformation("Executing stored procedure {StoredProcedure}", StoredProcedure.Heart);
        var cardiacPatients =  await _patientRepository.GetHeartPatientRecords(patientFile, StoredProcedure.Heart);
        
        return cardiacPatients
            .Select(x => x.MedicalRecordNumber)
            .ToHashSet();
    }

    public PatientApi.Patient MapPatientRecordToPatient(PatientRecord patientRecord)
    {
        var patient = new PatientApi.Patient();

        var nameArray = Array.Empty<string>();
        if (!string.IsNullOrEmpty(patientRecord.Name))
        {
            var name = patientRecord.Name.Split(' ');
            nameArray = (string[])name.Clone();
        }

        switch (nameArray.Length)
        {
            case < 2:
                _logger.LogError("Patient name must contain first and last");
                throw new Exception("Patient name must contain first and last");
            case 3:
                patient.FirstName = nameArray[0];
                patient.MiddleName = nameArray[2];
                patient.LastName = nameArray[1];
                break;
            default:
                patient.FirstName = nameArray[0];
                patient.LastName = nameArray[1];
                break;
        }

        patient.DateOfBirth = patientRecord.DateOfBirth;
        patient.Sex = patientRecord.Sex;
        patient.MedicalRecordNumbers = new List<PatientApi.MedicalRecordNumber>
        {
            new PatientApi.MedicalRecordNumber
            {
                Value = patientRecord.MedicalRecordNumber
            }
        };
        
        var phoneNumbers = FormatPhoneNumbers(patientRecord);
        patient.PhoneNumbers = phoneNumbers;
        patient.EmailAddresses = new List<PatientApi.EmailAddress>
        {
            new PatientApi.EmailAddress
            {
                Email = patientRecord.EmailAddress,
                IsPrimary = true
            }
        };
        
        if (!string.IsNullOrEmpty(patientRecord.ZipCode))
        {
            patient.Addresses = new List<PatientApi.Address>
            {
                new PatientApi.Address
                {
                    Zip = patientRecord.ZipCode
                }
            };
        }

        return patient;
    }
    
    private List<PatientApi.PhoneNumber> FormatPhoneNumbers(PatientRecord patientRecord)
    {
        var phoneNumbers = new List<PatientApi.PhoneNumber>();
        // format home phone
        if (patientRecord.HomeAreaCode?.Length == 3 && patientRecord.HomePhoneNumber?.Length == 7)
        {
            var homePhone = string.Concat(patientRecord.HomeAreaCode, patientRecord.HomePhoneNumber);
            phoneNumbers.Add(new PatientApi.PhoneNumber { Phone = homePhone});
        }
        
        // format cell phone
        if (patientRecord.CellAreaCode?.Length == 3 && patientRecord.CellPhoneNumber?.Length == 7)
        {
            var cellPhone = string.Concat(patientRecord.CellAreaCode, patientRecord.CellPhoneNumber);
            phoneNumbers.Add(new PatientApi.PhoneNumber { Phone = cellPhone});
        }

        return phoneNumbers;
    }
}