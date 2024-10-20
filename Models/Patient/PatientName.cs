namespace TestTask.Models.Patient;

public class PatientName
{
    public Guid Id { get; private set;}

    public NameUse? Use { get; set;}

    public string Family { get; set; }

    public Guid PatientId { get; set; }

    public Patient Patient { get; set; }

    public IEnumerable<GivenName>? Given { get; set; }

    public PatientName(Guid id, string family, IEnumerable<GivenName>? given, NameUse? nameUse)
    {
        Family = family;
        Id = id;
        Given = given;
        Use = nameUse;
    }

    public void Update(string? family, IEnumerable<string>? given, NameUse? nameUse)
    {
        Family = family ?? Family;
        Use = nameUse ?? Use;

        if (given is not null)
        {
            Given = given.Select(n => new GivenName(Guid.NewGuid(), n, Id)).ToList();
        }
    }

    private PatientName(){}
}
