using System.Linq.Expressions;
using System.Text;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using TestTask.Errors;
using TestTask.Models;
using TestTask.Models.Patient;
using TestTask.Repos;

namespace TestTask.Persistence.Repos;

public class PatientRepo : IPatientRepo
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PatientRepo> _logger;

    public PatientRepo(ApplicationDbContext dbContext, ILogger<PatientRepo> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public ErrorOr<Guid> CreatePatient(Patient patient)
    {
        if (_dbContext.Patients.Any(p => p.Id == patient.Id))
        {
            var errorResult = PatientErrors.PatientWithIdAlreadyExists(patient);
            LogError(errorResult);
            return errorResult;
        }

        try
        {
            _dbContext.Patients.Add(patient);
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            var errorResult = PatientErrors.GenericCreatePatientError(patient);
            LogError(errorResult, ex);

            return errorResult;
        }

        return patient.Id;
    }

    public ErrorOr<Deleted> DeletePatient(Guid id)
    {
        var patient = _dbContext.Patients.FirstOrDefault(p => p.Id == id);
        if (patient is null)
        {
            var errorResult = PatientErrors.PatientDoesNotExist(id);
            LogError(errorResult);
            return errorResult;
        }

        try
        {
            _dbContext.Patients.Remove(patient);
            _dbContext.SaveChanges();
            return Result.Deleted;
        }
        catch (Exception ex)
        {
            var errorResult = PatientErrors.GenericDeletePatientError(id);
            LogError(errorResult, ex);

            return errorResult;
        }
    }

    public ErrorOr<Patient> GetPatient(Guid id)
    {
        var patient = _dbContext.Patients
            .Include(p => p.Name)
                .ThenInclude(p => p.Given)
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == id);
        
        if (patient is null)
        {
            var errorResult = PatientErrors.PatientDoesNotExist(id);
            LogError(errorResult);
            return errorResult;
        }

        return patient;
    }

    public ErrorOr<IEnumerable<Patient>> GetPatientsByBirthDate(IEnumerable<FilterQuerry> filterQuerries)
    {
        if (filterQuerries is null || !filterQuerries.Any())
        {
            var errorResult = PatientErrors.BirthDateFilterStringInvalid();
            LogError(errorResult);
            return errorResult;
        }

        var firstFilter = ConstructExpression(filterQuerries.First().Prefix, filterQuerries.First().DateTime);
        IQueryable<Patient> result = _dbContext.Patients
            .Where(firstFilter)
            .Include(p => p.Name)
                .ThenInclude(n => n.Given)
            .AsNoTracking();
        
        foreach (var querry in  filterQuerries.Skip(1)){
            var nextFilter = ConstructExpression(querry.Prefix, querry.DateTime);
            result = result
                .Where(nextFilter)
                .AsNoTracking();
        }
        
        return result.ToList(); 
    }

    public ErrorOr<Patient> UpdatePatient(
        Guid id, 
        string? family,
        NameUse? nameUse,
        IEnumerable<string>? givenNames, 
        DateTime? birthDateUtc,
        Gender? gender, 
        bool? active)
    {
        var patient = _dbContext.Patients
            .Include(p => p.Name)
                .ThenInclude(p => p.Given)
            .FirstOrDefault(p => p.Id == id);

        if (patient is null)
        {
            var errorResult = PatientErrors.PatientDoesNotExist(id);
            LogError(errorResult);
            return errorResult;
        }

        var prevGivenNames = patient.Name.Given;
        var updateResult = patient.Update(
            birthDateUtc,
            gender,
            active,
            family,
            givenNames,
            nameUse
        );

        if (updateResult.IsError)
        {
            return updateResult.FirstError;
        }

        try
        {
            _dbContext.SaveChanges();
            
            return GetPatient(id);
        }
        catch (Exception ex)
        {
            var errorResult = PatientErrors.GenericUpdatePatientError(patient);
            LogError(errorResult, ex);

            return errorResult;
        }
    }

    private static Expression<Func<Patient, bool>> ConstructExpression(string prefix, DateTime filterDateTime)
    {
        Expression<Func<Patient, bool>> binaryExpression = prefix switch
        {
            "eq" => (patient) => patient.BirthDate == filterDateTime,
            "ne" => (patient) => patient.BirthDate != filterDateTime,
            "gt" => (patient) => patient.BirthDate > filterDateTime,
            "lt" => (patient) => patient.BirthDate < filterDateTime,
            "ge" => (patient) => patient.BirthDate >= filterDateTime,
            "le" => (patient) => patient.BirthDate <= filterDateTime,
            "sa" => (patient) => patient.BirthDate > filterDateTime,
            "eb" => (patient) => patient.BirthDate < filterDateTime,
            "ap" => (patient) => patient.BirthDate <= filterDateTime.AddDays(7) || patient.BirthDate >= filterDateTime.AddDays(-7),
            _ => (patient) => patient.BirthDate == filterDateTime,
        };

        return binaryExpression;
    }

    private void LogError(Error errorResult, Exception? ex = null)
    {
        var additionalData = new StringBuilder();
        if (errorResult.Metadata is not null)
        {
            additionalData.Append("Metadata: ");
            foreach (var pair in errorResult.Metadata)
            {
                additionalData.Append($"{pair.Key}:{pair.Value} ");
            }
        }
        if (ex is not null)
        {
            additionalData.Append($"Exeption:{ex.Message}");
        }
        _logger.LogError($"{errorResult.Code}: {errorResult.Description} {additionalData}");
    }
}
