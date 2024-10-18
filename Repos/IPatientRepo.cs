using ErrorOr;
using TestTask.Models.Patient;

namespace TestTask.Repos;

public interface IPatientRepo
{
    ErrorOr<Patient> GetPatient(Guid id);

    ErrorOr<IEnumerable<Patient>> GetPatientsByBirthDate(IEnumerable<Predicate<DateTime>> predicates);

    ErrorOr<Guid> CreatePatient(Patient patient);

    ErrorOr<Updated> UpdatePatient(
        Guid id, 
        PatientName? name, 
        DateTime? birthDateUtc,
        Gender? gender, 
        bool? active);

    ErrorOr<Deleted> DeletePatient(Guid id);
}