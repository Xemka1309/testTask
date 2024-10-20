using ErrorOr;
using TestTask.Models;
using TestTask.Models.Patient;

namespace TestTask.Repos;

public interface IPatientRepo
{
    ErrorOr<Patient> GetPatient(Guid id);

    ErrorOr<IEnumerable<Patient>> GetPatientsByBirthDate(IEnumerable<FilterQuerry> predicates);

    ErrorOr<Guid> CreatePatient(Patient patient);

    ErrorOr<Patient> UpdatePatient(
        Guid id, 
        string? family,
        NameUse? nameUse,
        IEnumerable<string>? givenNames, 
        DateTime? birthDateUtc,
        Gender? gender, 
        bool? active);

    ErrorOr<Deleted> DeletePatient(Guid id);
}