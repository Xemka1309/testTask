using AutoMapper;
using TestTask.Models.Patient;
using TestTask.Contract.v1.Patient;

namespace TestTask.Mapping;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<Contract.v1.Patient.Gender, Models.Patient.Gender>();
        CreateMap<Models.Patient.Gender, Contract.v1.Patient.Gender>();
        
        CreateMap<Contract.v1.Patient.NameUse, Models.Patient.NameUse>();
        CreateMap<Models.Patient.NameUse, Contract.v1.Patient.NameUse>();

        CreateMap<PatientName, GetPatientNameResponse>()
            .ForMember(n => n.Given, opt => opt.MapFrom(src => src.Given == null ? null : src.Given.Select(g => g.Value)))
            .ForMember(n => n.Id, opt => opt.MapFrom(src => src.PatientId))
            .ForMember(n => n.Given, opt => opt.AllowNull())
            .ForMember(n => n.Use, opt => opt.AllowNull());
        
        CreateMap<CreatePatientName, PatientName>().ConvertUsing((src, _, ctx) => MapFromCreatePatientName(src, ctx));

        CreateMap<Patient, GetPatientResponse>()
            .ForMember(p => p.Active, opt => opt.AllowNull())
            .ForMember(p => p.Gender, opt => opt.AllowNull());

        CreateMap<CreatePatientRequest, Patient>().ConvertUsing((src, _, ctx) => MapFromCreatePatient(src, ctx));
        
    }

    // Assume src values is valid at this point
    public static Patient MapFromCreatePatient(CreatePatientRequest createPatient, ResolutionContext ctx)
    {   
        var patientName = ctx.Mapper.Map<PatientName>(createPatient.Name);
        var birthDateUtc = DateTime.Parse(
            createPatient.BirthDate!, 
            default, 
            System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal);
        
        var patient = Patient.Create(
            patientName,
            birthDateUtc,
            Guid.NewGuid(),
            createPatient.Gender is null ? null : ctx.Mapper.Map<Models.Patient.Gender>(createPatient.Gender),
            createPatient.Active
        );

        return patient.Value;
    }

    public static PatientName MapFromCreatePatientName(CreatePatientName createPatientName, ResolutionContext ctx)
    {
        var patientNameId = Guid.NewGuid();
        var givenName = createPatientName.Given?
            .Select(value => new GivenName(id: Guid.NewGuid(), value, patientNameId)).ToList();
        
        return new PatientName(
            patientNameId,
            createPatientName.Family, 
            givenName, 
            createPatientName.Use is null ? null : ctx.Mapper.Map<Models.Patient.NameUse>(createPatientName.Use));
    }
}