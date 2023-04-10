using System.Data;
using System.Diagnostics;
using Boilerplate.Models;
using Boilerplate.Utilities;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Boilerplate.Repositories.Patient;

public class PatientRepository : IPatientRepository
{
    private readonly IPatientSqlConnectionFactory _sqlConnectionFactory;
    private readonly ILogger<PatientRepository> _logger;

    public PatientRepository(IPatientSqlConnectionFactory sqlConnectionFactory, ILogger<PatientRepository> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _logger = logger;
    }
    
    public async Task<List<PatientRecord>> GetHeartPatientRecords(PatientFile patientFile, string storedProcedure)
    {
        for (int i = 0; i < 10; i++)
        {
            _logger.LogInformation("Executing stored procedure {StoredProcedure}", storedProcedure);
            var parameters = new DynamicParameters();
            parameters.Add("@DateOfBirth55", DateTime.UtcNow.AddYears(-55), DbType.DateTime2, ParameterDirection.Input); 
            parameters.Add("@DateOfBirth80", DateTime.UtcNow.AddYears(-80), DbType.DateTime2, ParameterDirection.Input);
            parameters.Add("@EndDate", patientFile.EndDate!.Value.DateTime, DbType.DateTime2, ParameterDirection.Input);
            parameters.Add("@StartDate", patientFile.StartDate!.Value.DateTime, DbType.DateTime2, ParameterDirection.Input);
            parameters.Add("@StringBlank", string.Empty, DbType.String, ParameterDirection.Input, 512);
                
            await using var sqlConnection = await _sqlConnectionFactory.CreateAsync(patientFile.ClientId!);
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            
            var results = await sqlConnection.QueryAsync<PatientRecord>(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure);
            
            stopWatch.Stop();
            _logger.LogInformation("It took {ElapsedTimeSeconds} seconds to run the {StoredProcedure} stored procedure for {PatientRecordCount} records", 
                stopWatch.ElapsedMilliseconds / 1000, storedProcedure, results.ToList().Count);
        }

        return new List<PatientRecord>();
        //return results.ToList();
    }
    
    public async Task<List<PatientRecord>> GetDigestivePatientRecords(PatientFile patientFile, string storedProcedure)
    {
        _logger.LogInformation("Executing stored procedure {StoredProcedure}", storedProcedure);
        
        var parameters = new DynamicParameters();
        parameters.Add("@DateOfBirth35", DateTime.UtcNow.AddYears(-35), DbType.DateTime2);
        parameters.Add("@DateOfBirth40", DateTime.UtcNow.AddYears(-40), DbType.DateTime2);
        parameters.Add("@DateOfBirth80", DateTime.UtcNow.AddYears(-80), DbType.DateTime2);
        parameters.Add("@EndDate", patientFile.EndDate, DbType.DateTime2);
        parameters.Add("@StartDate", patientFile.StartDate, DbType.DateTime2);
        parameters.Add("@StringBlank", string.Empty);
        parameters.Add("@StringM", "M");
            
        await using var sqlConnection = await _sqlConnectionFactory.CreateAsync(patientFile.ClientId!);

        var stopWatch = new Stopwatch();
        stopWatch.Start();
        
        var results = await sqlConnection.QueryAsync<PatientRecord>(storedProcedure, parameters,
            commandType: CommandType.StoredProcedure);
        
        stopWatch.Stop();
        _logger.LogInformation("It took {ElapsedTimeSeconds} seconds to run the {StoredProcedure} stored procedure", 
            stopWatch.ElapsedMilliseconds / 1000, storedProcedure);
        
        return results.ToList();
    }

    public async Task<List<PatientRecord>> GetLungPatientRecords(PatientFile patientFile, string storedProcedure)
    {
        _logger.LogInformation("Executing stored procedure {StoredProcedure}", storedProcedure);
        
        var parameters = new DynamicParameters();
        parameters.Add("@DateOfBirth55", DateTime.UtcNow.AddYears(-55), DbType.DateTime2); 
        parameters.Add("@DateOfBirth80", DateTime.UtcNow.AddYears(-80), DbType.DateTime2);
        parameters.Add("@EndDate", patientFile.EndDate, DbType.DateTime2);
        parameters.Add("@StartDate", patientFile.StartDate, DbType.DateTime2);
        parameters.Add("@StringBlank", string.Empty);
            
        await using var sqlConnection = await _sqlConnectionFactory.CreateAsync(patientFile.ClientId!);
        
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        
        var results = await sqlConnection.QueryAsync<PatientRecord>(storedProcedure, parameters,
            commandType: CommandType.StoredProcedure);
        
        stopWatch.Stop();
        _logger.LogInformation("It took {ElapsedTimeSeconds} seconds to run the {StoredProcedure} stored procedure", 
            stopWatch.ElapsedMilliseconds / 1000, storedProcedure);
        return results.ToList();
    }
    
    public async Task<List<PatientRecord>> GetOrthopedicPatientRecords(PatientFile patientFile, string storedProcedure)
    {
        _logger.LogInformation("Executing stored procedure {StoredProcedure}", storedProcedure);
        
        var parameters = new DynamicParameters();
        parameters.Add("@DateOfBirth35", DateTime.UtcNow.AddYears(-35), DbType.DateTime2);
        parameters.Add("@DateOfBirth80", DateTime.UtcNow.AddYears(-80), DbType.DateTime2);
        parameters.Add("@EndDate", patientFile.EndDate, DbType.DateTime2);
        parameters.Add("@StartDate", patientFile.StartDate, DbType.DateTime2);
        parameters.Add("@StringBlank", string.Empty);
            
        await using var sqlConnection = await _sqlConnectionFactory.CreateAsync(patientFile.ClientId!);
        
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        
        var results = await sqlConnection.QueryAsync<PatientRecord>(storedProcedure, parameters,
            commandType: CommandType.StoredProcedure);
        
        stopWatch.Stop();
        _logger.LogInformation("It took {ElapsedTimeSeconds} seconds to run the {StoredProcedure} stored procedure", 
            stopWatch.ElapsedMilliseconds / 1000, storedProcedure);

        return results.ToList();
    }
}