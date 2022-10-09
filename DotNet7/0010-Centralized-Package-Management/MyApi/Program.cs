using System.Reflection;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();
app.MapGet("/ping", () => "pong");
app.MapPost("/build-name", (RequestDto req, IMapper mapper) => 
{
    // Map input to DTO for data processing "layer"
    var data = mapper.Map<DataDto>(req);

    // Process data
    var result = BuildFullName(data);

    // Build result DTO and return it
    return Results.Ok(new ResponseDto(result));
});

app.Run();

static string BuildFullName(DataDto person) => $"{person.GivenName} {person.SurName}";

record RequestDto(string FirstName, string LastName);
record DataDto(string GivenName, string SurName);
record ResponseDto(string FullName);
public class MapperProfile : Profile
{
	public MapperProfile()
	{
		CreateMap<RequestDto, DataDto>()
            .ForCtorParam(nameof(DataDto.GivenName), opt => opt.MapFrom(src => src.FirstName))
            .ForCtorParam(nameof(DataDto.SurName), opt => opt.MapFrom(src => src.LastName));
	}
}