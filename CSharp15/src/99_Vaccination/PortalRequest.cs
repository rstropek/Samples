using System.Text.Json.Serialization;

namespace Vaccination;

// The city's booking portal drops one JSON file per submission. We don't own
// that format, and it has no type field: a submission is either for one person
//   { "email": "...", "person":  { ... } }
// or for a family
//   { "email": "...", "members": [ { ... }, { ... } ] }
// The structural classifier picks the case by its property names.
public sealed record PortalPerson(string Name, DateOnly BirthDate, string InsuranceNumber, string[] Vaccines, bool RiskFactors = false)
{
    public Person ToPerson() => new(InsuranceNumber, Name, BirthDate, RiskFactors);
}

public sealed record SingleRequest(string Email, PortalPerson Person);
public sealed record FamilyRequest(string Email, PortalPerson[] Members);

[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public union PortalRequest(SingleRequest, FamilyRequest)
{
    // The rest of the app doesn't care which shape the portal sent.
    public string Email => this switch
    {
        SingleRequest s => s.Email,
        FamilyRequest f => f.Email,
    };

    public PortalPerson[] Members => this switch
    {
        SingleRequest s => [s.Person],
        FamilyRequest f => f.Members,
    };
}
