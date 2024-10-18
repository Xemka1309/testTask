using ErrorOr;
using TestTask.Models.Patient;

namespace TestTask.Errors;

public static class PatientErrors
{
    private static Dictionary<string, object> CreatePatientMetaData(Patient patient)
    {
        var meta = new Dictionary<string, object>
        {
            { "family", patient.Name.Family },
            { "patientId", patient.Id },
            { "traceId", Guid.NewGuid() }
        };

        return meta;
    }

    private static Dictionary<string, object> CreatePatientIdMetaData(Guid id)
    {
        var meta = new Dictionary<string, object>
        {
            { "patientId", id },
            { "traceId", Guid.NewGuid() }
        };

        return meta;
    }

    public static Error GenericCreatePatientError(Patient patient) 
        => Error.Failure("PatientErrors.GenericCreatePatientError", "Unable to add patient entry into database", CreatePatientMetaData(patient));
    
    public static Error GenericUpdatePatientError(Patient patient) 
        => Error.Failure("PatientErrors.GenericUpdatePatientError", "Unable to update patient entry in database", CreatePatientMetaData(patient));

    public static Error GenericDeletePatientError(Guid id) 
        => Error.Failure("PatientErrors.GenericDeletePatientError", "Unable to delete patient entry in database", CreatePatientIdMetaData(id));
    
    public static Error PatientWithIdAlreadyExists(Patient patient) 
        => Error.Conflict("PatientErrors.PatientWithIdAlreadyExists", "Unable to add patient entry into database: patient with provided id already exists", CreatePatientMetaData(patient));

    public static Error PatientIdInvalid(Guid id) 
        => Error.Validation("PatientErrors.InvalidId", "Patient id is invalid", CreatePatientIdMetaData(id));    

    public static Error PatientDoesNotExist(Guid id) 
        => Error.NotFound("PatientErrors.PatientDoesNotExist", "Patient with provided id does not exist", CreatePatientIdMetaData(id));    

    public static Error InvalidBirthDate() 
        => Error.Validation("PatientErrors.InvalidBirthDate", "Birth Date is invalid");

    public static Error FamilyNameIsRequired() 
        => Error.Validation("PatientErrors.FamilyNameIsRequired", "Family name is required");

    public static Error BirthDateIsRequired() 
        => Error.Validation("PatientErrors.FamilyNameIsRequired", "Birth date is required");

    public static Error SearchPredicatsEmpty() 
        => Error.Validation("PatientErrors.SearchPredicatsEmpty", "Search predicats is empty");
}