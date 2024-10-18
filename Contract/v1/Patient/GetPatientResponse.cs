namespace TestTask.Contract.v1.Patient;

public class GetPatientResponse
{
    public GetPatientNameResponse Name { get; set;}
    public Gender? Gender { get; set; }
    public DateTime BirthDate { get; private set; }
    public bool? Active { get; set; }
}

public class GetPatientNameResponse
{
    public NameUse? Use { get; set;}
    public string Family { get; set; }
    public Guid Id { get; set; }
    public IEnumerable<string>? Given { get; set; }
}