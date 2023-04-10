using Boilerplate.Models;

namespace Boilerplate.Repositories.Patient;

public interface IPatientRepository
{
    Task<List<PatientRecord>> GetDigestivePatientRecords(PatientFile patientFileImport, string storedProcedure);
    Task<List<PatientRecord>> GetHeartPatientRecords(PatientFile patientFileImport, string storedProcedure);
    Task<List<PatientRecord>> GetLungPatientRecords(PatientFile patientFileImport, string storedProcedure);
    Task<List<PatientRecord>> GetOrthopedicPatientRecords(PatientFile patientFileImport, string storedProcedure);
}