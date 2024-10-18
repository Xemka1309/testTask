using System.Text;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using TestTask.Errors;
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
            .Include(p => p.Name.Given)
            .FirstOrDefault(p => p.Id == id);
        if (patient is null)
        {
            var errorResult = PatientErrors.PatientDoesNotExist(id);
            LogError(errorResult);
            return errorResult;
        }

        return patient;
    }

    public ErrorOr<IEnumerable<Patient>> GetPatientsByBirthDate(IEnumerable<Predicate<DateTime>> predicates)
    {
        if (predicates is null || !predicates.Any())
        {
            var errorResult = PatientErrors.SearchPredicatsEmpty();
            LogError(errorResult);
            return errorResult;
        }

        var result = _dbContext.Patients.Where(patient => predicates.All(rule => rule.Invoke(patient.BirthDate)));

        return result.ToList();
    }

    public ErrorOr<Updated> UpdatePatient(
        Guid id, 
        PatientName? name, 
        DateTime? birthDateUtc,
        Gender? gender, 
        bool? active)
    {
        var patient = _dbContext.Patients
            .Include(p => p.Name)
            .Include(p => p.Name.Given)
            .FirstOrDefault(p => p.Id == id);

        if (patient is null)
        {
            var errorResult = PatientErrors.PatientDoesNotExist(id);
            LogError(errorResult);
            return errorResult;
        }

        var updateResult = patient.Update(
            name,
            birthDateUtc,
            gender,
            active
        );

        if (updateResult.IsError)
        {
            return updateResult.FirstError;
        }

        try
        {
            _dbContext.SaveChanges();
            return Result.Updated;
        }
        catch (Exception ex)
        {
            var errorResult = PatientErrors.GenericUpdatePatientError(patient);
            LogError(errorResult, ex);

            return errorResult;
        }
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
