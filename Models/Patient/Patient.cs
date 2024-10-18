using System.ComponentModel.DataAnnotations;
using ErrorOr;
using TestTask.Errors;

namespace TestTask.Models.Patient;

public class Patient
{
    [Key]
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
        PatientName? name, 
        DateTime? birthDateUtc,
        Gender? gender, 
        bool? active)
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

        if (name is not null)
        {
            Name.Family = name.Family ?? Name.Family;
            Name.Use = name.Use ?? Name.Use;
            if (name.Given is not null)
            {
                Name.Given = name.Given;
            }
        }

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
