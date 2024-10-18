namespace TestTask.Contract.v1.Patient;

public class UpdatePatientRequest
{
    public UpdatePatientName? Name { get; set;}
    public Gender? Gender { get; set; }    
    public string? BirthDate { get; set; }
    public bool? Active { get; set; }
}


public class UpdatePatientName
{
    public NameUse? Use { get; set;}
    public string? Family { get; set; }
    public IEnumerable<string>? Given { get; set; }
}