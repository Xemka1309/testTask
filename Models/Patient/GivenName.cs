namespace TestTask.Models.Patient;

public class GivenName
{
    public Guid Id { get; private set; }
    public string Value { get; set; }
    public Guid PatientNameId { get; set; }
    public PatientName PatientName { get; set; }
    private GivenName()
    {

    }

    public GivenName(Guid id, string value, Guid patientNameId)
    {
        Value = value;
        PatientNameId = patientNameId;
        Id = id;
    }
}