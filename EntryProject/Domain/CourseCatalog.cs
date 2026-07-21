namespace EntryProject.Domain;

/// <summary>
/// Prototype content catalog for the aivezdravotnictví portal (plausible content in the spirit of IPVZ).
/// Courses are split by level; some steps are tailored to the role.
/// </summary>
public static class CourseCatalog
{
    /// <summary>Offer for a given level (format of the recommended steps).</summary>
    public static readonly IReadOnlyDictionary<Level, IReadOnlyList<string>> CoursesByLevel =
        new Dictionary<Level, IReadOnlyList<string>>
        {
            [Level.Basic] = new List<string>
            {
                "Volný e-learning „AI gramotnost ve zdravotnictví – základy“ (bez registrace)",
                "Mikrolekce „Co AI umí a neumí v klinické praxi“",
                "Checklist „Bezpečná práce s citlivými daty a AI“"
            },
            [Level.Advanced] = new List<string>
            {
                "Webinář IPVZ „Praktické nasazení AI ve zdravotnické praxi“",
                "Workshop „AI čtvrthodinka“ – pravidelné krátké tréninky promptování",
                "Kurz „Ověřování a kritické čtení výstupů AI“"
            },
            [Level.Specialist] = new List<string>
            {
                "Specializované školení „Pokročilé workflow s AI pro vaši roli“",
                "Pilotní workshop s reálnými AI nástroji a mentorem",
                "Komunita praxe – sdílení a spoluvytváření knihovny postupů"
            }
        };

    /// <summary>Role-specific accent added to the recommendation (1 sentence).</summary>
    public static readonly IReadOnlyDictionary<Role, string> RoleTip = new Dictionary<Role, string>
    {
        [Role.Patient] =
            "Zaměřte se na srozumitelné a bezpečné využití AI pro orientaci ve vlastním zdraví (nenahrazuje lékaře).",
        [Role.HealthcareWorker] =
            "Vybírejte moduly cílené na klinickou dokumentaci, edukaci pacientů a ověřování výstupů.",
        [Role.ItSpecialistOrVendor] =
            "Doplňte témata integrace, bezpečnosti dat a governance AI ve zdravotnických systémech.",
        [Role.PolicyMakerOrMedia] =
            "Preferujte moduly o dopadech, etice a odpovědné komunikaci AI ve zdravotnictví.",
        [Role.NonClinicalHospitalStaff] =
            "Volte moduly pro provozní a administrativní procesy nemocnice."
    };

    /// <summary>Masterclass for management (offered based on role, not score).</summary>
    public const string Masterclass =
        "Masterclass (Management) – strategické vedení adopce AI v organizaci, řízení rizik a změny.";

    public static Level? NextLevel(Level level) => level switch
    {
        Level.Basic => Level.Advanced,
        Level.Advanced => Level.Specialist,
        _ => null
    };
}
