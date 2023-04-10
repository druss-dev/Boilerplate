using Boilerplate.Models;

namespace Boilerplate.Services.Patient;

public interface IPatientService
{
    Task ProcessPatientsAsync(PatientFile patientFile);
}