using System.ComponentModel.DataAnnotations;
using ErrorOr;
using TestTask.Errors;

namespace TestTask.Models.Patient;

public class Patient
{
    public Guid Id { get; private set;}
    public PatientName Name { get; private set;}
    public Gender? Gender { get; private set; }
    public DateTime BirthDate { get; private set; }
    public bool? Active { get; private set; }
    private Patient(Guid id, PatientName name, DateTime birthDate, Gender? gender, bool? active)
    {
        Name = name;
        BirthDate = birthDate;
        Id = id;
        Gender = gender;
        Active = active;
    }

    private Patient(){}

    public static ErrorOr<Patient> Create(
        PatientName name, 
        DateTime birthDateUtc,
        Guid id,
        Gender? gender, 
        bool? active)
    {
        if (birthDateUtc > DateTime.UtcNow)
        {
            return PatientErrors.InvalidBirthDate();
        }

        return new Patient(id, name, birthDateUtc, gender, active);
    }

    public ErrorOr<Updated> Update( 
        DateTime? birthDateUtc,
        Gender? gender, 
        bool? active,
        string? family, 
        IEnumerable<string>? given, 
        NameUse? nameUse)
    {
        if (birthDateUtc is not null)
        {
            if (birthDateUtc.Value > DateTime.UtcNow)
            {
                return PatientErrors.InvalidBirthDate();
            }
            BirthDate = birthDateUtc.Value;
        }
        
        Gender = gender ?? Gender;
        Active = active ?? Active;

        Name.Update(family, given, nameUse);

        return Result.Updated;

    }

    public ErrorOr<Updated> SetBirthDate(DateTime birthDateUtc)
    {
        if (birthDateUtc > DateTime.UtcNow)
        {
            return PatientErrors.InvalidBirthDate();
        }

        BirthDate = birthDateUtc;

        return Result.Updated;
    }
}
