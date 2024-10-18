using System.Net;
using AutoMapper;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using TestTask.Contract.v1.Patient;
using TestTask.Errors;
using TestTask.Extensions;
using TestTask.Models.Patient;
using TestTask.Repos;

namespace testTask.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class PatientController : ControllerBase
{
    private readonly ILogger<PatientController> _logger;
    private readonly IMapper _mapper;
    private readonly IPatientRepo _patientRepo;

    public PatientController(ILogger<PatientController> logger, IPatientRepo patientRepo, IMapper mapper)
    {
        _logger = logger;
        _patientRepo = patientRepo;
        _mapper = mapper;
    }

    [HttpGet("{id}", Name = nameof(GetPatient))]
    public ActionResult<GetPatientResponse> GetPatient([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            return ErrorResponse(PatientErrors.PatientIdInvalid(id));
        }

        var result = _patientRepo.GetPatient(id);

        return result.IsError ? ErrorResponse(result.FirstError) : Ok(_mapper.Map<GetPatientResponse>(result.Value));
    }

    [HttpPost(Name = nameof(CreatePatient))]
    public ActionResult<Guid> CreatePatient([FromBody] CreatePatientRequest createPatientRequest)
    {
        if (string.IsNullOrEmpty(createPatientRequest.Name?.Family))
        {
            return ErrorResponse(PatientErrors.FamilyNameIsRequired());
        }

        if (string.IsNullOrEmpty(createPatientRequest.BirthDate))
        {
            return ErrorResponse(PatientErrors.BirthDateIsRequired());
        }

        if (!createPatientRequest.BirthDate.IsValidDateTimeString())
        {
            return ErrorResponse(PatientErrors.InvalidBirthDate());
        }

        var patient = _mapper.Map<Patient>(createPatientRequest);

        var result = _patientRepo.CreatePatient(patient);

        return result.IsError ? ErrorResponse(result.FirstError) : CreatedAtRoute(nameof(GetPatient), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id}", Name = nameof(UpdatePatient))]
    public ActionResult<Guid> UpdatePatient([FromBody] UpdatePatientRequest updatePatientRequest, [FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            return ErrorResponse(PatientErrors.PatientIdInvalid(id));
        }

        DateTime? birthDate = null;
        if (updatePatientRequest.BirthDate is not null)
        {
            if (!updatePatientRequest.BirthDate.IsValidDateTimeString())
            {
                return ErrorResponse(PatientErrors.InvalidBirthDate());
            }
            birthDate = updatePatientRequest.BirthDate.ToDateTime();
        }

        var result = _patientRepo.UpdatePatient(id, 
            _mapper.Map<PatientName>(updatePatientRequest.Name), 
            birthDate, 
            _mapper.Map<TestTask.Models.Patient.Gender>(updatePatientRequest.Gender), 
            updatePatientRequest.Active);

        return result.IsError ? ErrorResponse(result.FirstError) : Ok(id);
    }

    [HttpDelete("{id}", Name = nameof(DeletePatient))]
    public ActionResult<Guid> DeletePatient([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            return ErrorResponse(PatientErrors.PatientIdInvalid(id));
        }

        var result = _patientRepo.DeletePatient(id);
        
        return result.IsError ? ErrorResponse(result.FirstError) : Ok(id);
    }

    private ActionResult ErrorResponse(Error error)
    {
        if (error.Type == ErrorType.Validation)
        {
            return BadRequest(error);
        }

        if (error.Type == ErrorType.NotFound)
        {
            return NotFound(error);
        }

        return StatusCode((int)HttpStatusCode.InternalServerError, error);
    }
}
