namespace EntryProject.Domain;

/// <summary>
/// Five entry roles of the portal. Stored as int (see TriageResult.Role),
/// mapping to Czech display names is held by <see cref="RoleCatalog"/>.
/// </summary>
public enum Role
{
    Patient = 1,
    HealthcareWorker = 2,
    ItSpecialistOrVendor = 3,
    PolicyMakerOrMedia = 4,
    NonClinicalHospitalStaff = 5
}

public static class RoleCatalog
{
    public static readonly IReadOnlyDictionary<Role, string> Names = new Dictionary<Role, string>
    {
        [Role.Patient] = "Pacient",
        [Role.HealthcareWorker] = "Zdravotník",
        [Role.ItSpecialistOrVendor] = "IT odborník či dodavatel",
        [Role.PolicyMakerOrMedia] = "Tvůrce politik a média",
        [Role.NonClinicalHospitalStaff] = "Nezdravotnický personál nemocnice"
    };

    /// <summary>Role for which it makes sense to offer the Masterclass (Management) when the management flag is checked.</summary>
    public static bool IsManagementRelevant(Role role) => role == Role.NonClinicalHospitalStaff;

    public static string Name(Role role) => Names[role];

    public static IEnumerable<Role> All => Enum.GetValues<Role>();
}
