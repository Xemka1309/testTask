using System.ComponentModel.DataAnnotations;

namespace TestTask.Contract.v1.Patient;

public class CreatePatientRequest
{
    public CreatePatientName Name { get; set;}
    public Gender? Gender { get; set; }
    public string BirthDate { get; set; }
    public bool? Active { get; set; }
}

public class CreatePatientName
{
    public NameUse? Use { get; set;}
    public string Family { get; set; }
    public IEnumerable<string>? Given { get; set; }
}