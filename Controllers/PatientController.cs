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

/// <summary>
/// Patients webapi controller
/// </summary>
[ApiController]
[Route("/api/v1/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly ILogger<PatientsController> _logger;
    private readonly IMapper _mapper;
    private readonly IPatientRepo _patientRepo;


    public PatientsController(ILogger<PatientsController> logger, IPatientRepo patientRepo, IMapper mapper)
    {
        _logger = logger;
        _patientRepo = patientRepo;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets a patient by id
    /// </summary>
    /// <returns>Patient for provided id</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /e4f75b1c-4aa9-422f-ac0e-f4bf3efdc4dc
    ///
    /// </remarks>
    /// <response code="200">Returns the patient</response>
    /// <response code="400">If the id is invalid</response>
    /// <response code="404">If patient with provided id is not exists</response>
    [HttpGet("{id}", Name = nameof(GetPatient))]
    [ProducesResponseType(typeof(GetPatientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<GetPatientResponse> GetPatient([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            return ErrorResponse(PatientErrors.PatientIdInvalid(id));
        }

        var result = _patientRepo.GetPatient(id);

        return result.IsError ? ErrorResponse(result.FirstError) : Ok(_mapper.Map<GetPatientResponse>(result.Value));
    }

    [HttpGet("", Name = nameof(GetPatients))]
    public ActionResult<IEnumerable<GetPatientResponse>> GetPatients([FromQuery(Name = "birthDate")] string[] birthDateQuerries)
    {
        if (birthDateQuerries is null || !birthDateQuerries.Any())
        {
            return ErrorResponse(PatientErrors.BirthDateIsRequired());
        }

        if (birthDateQuerries.Any(querry => !querry.IsValidFilterDateTimeString()))
        {
            return ErrorResponse(PatientErrors.BirthDateFilterStringInvalid());
        }

        var result = _patientRepo.GetPatientsByBirthDate(birthDateQuerries.Select(value => value.ToFilterQuerry()));

        return result.IsError
            ? ErrorResponse(result.FirstError)
            : Ok(result.Value.Select(patient => _mapper.Map<GetPatientResponse>(patient)).ToArray());
    }

    /// <summary>
    /// Create new patient
    /// </summary>
    /// <returns>newly created patient</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /
    ///     {
    ///         "name": {
    ///             "use": "Usual",
    ///             "family": "ivanov",
    ///             "given": [
    ///                 "ivan", 
    ///                 "ivanovich"
    ///             ]
    ///          },
    ///         "gender": "Male",
    ///         "birthDate": "2000-02-03",
    ///         "active": true
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns newly created patient</response>
    /// <response code="400">If request model is invalid</response>
    [HttpPost(Name = nameof(CreatePatient))]
    [ProducesResponseType(typeof(GetPatientResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
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

        return result.IsError
            ? ErrorResponse(result.FirstError)
            : CreatedAtRoute(nameof(GetPatient), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id}", Name = nameof(UpdatePatient))]
    public ActionResult<GetPatientResponse> UpdatePatient([FromBody] UpdatePatientRequest updatePatientRequest, [FromRoute] Guid id)
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

        var result = _patientRepo.UpdatePatient(
            id,
            updatePatientRequest.Name?.Family,
            updatePatientRequest.Name?.Use is null ? null : _mapper.Map<TestTask.Models.Patient.NameUse>(updatePatientRequest.Name?.Use),
            updatePatientRequest.Name?.Given,
            birthDate,
            updatePatientRequest.Gender is null ? null : _mapper.Map<TestTask.Models.Patient.Gender?>(updatePatientRequest.Gender),
            updatePatientRequest.Active);

        return result.IsError ? ErrorResponse(result.FirstError) : Ok(_mapper.Map<GetPatientResponse>(result.Value));
    }

    /// <summary>
    /// Deletes a patient by id
    /// </summary>
    /// <returns>Id of the deleted patient</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /e4f75b1c-4aa9-422f-ac0e-f4bf3efdc4dc
    ///
    /// </remarks>
    /// <response code="200">Returns Id of the deleted patient</response>
    /// <response code="400">If the id is invalid</response>
    /// <response code="404">If patient with provided id is not exists</response>
    [HttpDelete("{id}", Name = nameof(DeletePatient))]
    [ProducesResponseType(typeof(GetPatientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
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
