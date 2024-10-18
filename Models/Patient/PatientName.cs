using System.ComponentModel.DataAnnotations;

namespace TestTask.Models.Patient;

public class PatientName
{
    [Key]
    public Guid Id { get; private set;}

    public NameUse? Use { get; set;}

    public string Family { get; set; }

    public Guid PatientId { get; set; }

    public Patient Patient { get; set; }

    public IEnumerable<GivenName>? Given { get; set; }

    public PatientName(string family, IEnumerable<GivenName>? given, NameUse? nameUse)
    {
        Family = family;
        Id = new Guid();
        Given = given;
        Use = nameUse;
    }

    private PatientName(){}
}
