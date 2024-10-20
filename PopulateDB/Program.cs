using System.Net.Http.Json;

var httpClient = new HttpClient();
var rand = new Random(13131313);
var familyNames = FamilyNames.GetNames();

for (var i = 0; i < 100; i++)
{
    var patient = CreatePatient();
    using var response = await httpClient.PostAsJsonAsync("http://localhost:8000/api/v1/patients", patient);
    Console.WriteLine($"Response status code recevied: {response.StatusCode}");
}

CreatePatientRequest CreatePatient()
{
    return new CreatePatientRequest()
    {
        Active = true,
        Gender = (Gender)rand.NextInt64(0, 3),
        BirthDate = DateTime.Parse("1900-01-01")
            .AddYears((int)rand.NextInt64(0, 100))
            .AddDays(rand.NextInt64(0, 25))
            .AddMonths((int)rand.NextInt64(0, 10))
            .AddHours((int)rand.NextInt64(0, 22))
            .AddMinutes((int)rand.NextInt64(0, 58))
            .AddSeconds((int)rand.NextInt64(0, 58))
            .ToString(),
        Name = new CreatePatientName()
        {
            Family = familyNames[(int)rand.NextInt64(1, 50)],
            Given = new List<string>(){ "foo", "bar"},
            Use = (NameUse)rand.NextInt64(0, 6)
        }
    };
}