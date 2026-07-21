using EntryProject.Domain;
using Xunit;

namespace EntryProject.Tests;

public class ScoringServiceTests
{
    // --- Habit score boundary values -------------------------------------

    [Theory]
    [InlineData(2, 0)]
    [InlineData(3, 13)]   // odd sum -> exercises AwayFromZero midpoint rounding (12.5 -> 13)
    [InlineData(4, 25)]
    [InlineData(6, 50)]
    [InlineData(8, 75)]
    [InlineData(10, 100)]
    public void HabitScore_MapsCorrectly(int sum, int expected)
        => Assert.Equal(expected, ScoringService.HabitScore(sum));

    [Theory]
    [InlineData(1)]
    [InlineData(11)]
    public void HabitScore_OutOfRange_Throws(int sum)
        => Assert.Throws<ArgumentOutOfRangeException>(() => ScoringService.HabitScore(sum));

    // --- Level transitions -------------------------------------------------

    [Theory]
    [InlineData(0, Level.Basic)]
    [InlineData(39, Level.Basic)]
    [InlineData(40, Level.Advanced)]
    [InlineData(74, Level.Advanced)]
    [InlineData(75, Level.Specialist)]
    [InlineData(100, Level.Specialist)]
    public void LevelFromScore_BoundariesAreCorrect(int score, Level expected)
        => Assert.Equal(expected, ScoringService.LevelFromScore(score));

    // --- Full evaluation -----------------------------------------------------

    [Fact]
    public void Evaluate_AllAnswers5_Gives100AndSpecialist()
    {
        var answers = AllAnswers(5);
        var result = ScoringService.Evaluate(answers);

        Assert.Equal(100, result.SpScore);
        Assert.Equal(Level.Specialist, result.Level);
        Assert.All(result.HabitScores.Values, s => Assert.Equal(100, s));
    }

    [Fact]
    public void Evaluate_AllAnswers1_Gives0AndBasic()
    {
        var answers = AllAnswers(1);
        var result = ScoringService.Evaluate(answers);

        Assert.Equal(0, result.SpScore);
        Assert.Equal(Level.Basic, result.Level);
    }

    [Fact]
    public void Evaluate_MixedAnswers_ComputesEachHabitIndependently()
    {
        var answers = new Dictionary<int, int>
        {
            [1] = 5, [2] = 5,   // EarlyAdoption      sum=10 -> 100
            [3] = 4, [4] = 4,   // Experimentation    sum=8  -> 75
            [5] = 3, [6] = 3,   // Iteration          sum=6  -> 50
            [7] = 2, [8] = 2,   // OutputVerification sum=4  -> 25
            [9] = 1, [10] = 2,  // SavedWorkflows     sum=3  -> 13
            [11] = 1, [12] = 1, // Sharing            sum=2  -> 0
        };

        var result = ScoringService.Evaluate(answers);

        Assert.Equal(100, result.HabitScores[Habit.EarlyAdoption]);
        Assert.Equal(75, result.HabitScores[Habit.Experimentation]);
        Assert.Equal(50, result.HabitScores[Habit.Iteration]);
        Assert.Equal(25, result.HabitScores[Habit.OutputVerification]);
        Assert.Equal(13, result.HabitScores[Habit.SavedWorkflows]);
        Assert.Equal(0, result.HabitScores[Habit.Sharing]);
        Assert.Equal(44, result.SpScore); // average of 100,75,50,25,13,0 = 43.83.. -> 44
        Assert.Equal(Level.Advanced, result.Level);
    }

    [Fact]
    public void Evaluate_MissingAnswer_Throws()
    {
        var answers = AllAnswers(3);
        answers.Remove(1);
        Assert.Throws<ArgumentException>(() => ScoringService.Evaluate(answers));
    }

    [Fact]
    public void Evaluate_AnswerOutOfRange_Throws()
    {
        var answers = AllAnswers(3);
        answers[1] = 6; // valid range is 1-5
        Assert.Throws<ArgumentOutOfRangeException>(() => ScoringService.Evaluate(answers));
    }

    [Fact]
    public void Evaluate_NullAnswers_Throws()
        => Assert.Throws<ArgumentNullException>(() => ScoringService.Evaluate(null!));

    // --- Masterclass logic ---------------------------------------------------

    [Fact]
    public void Masterclass_OfferedOnlyToManagementOfNonClinicalStaff()
    {
        Assert.True(ScoringService.OfferMasterclass(Role.NonClinicalHospitalStaff, true));
        Assert.False(ScoringService.OfferMasterclass(Role.NonClinicalHospitalStaff, false));
        Assert.False(ScoringService.OfferMasterclass(Role.HealthcareWorker, true));
    }

    private static Dictionary<int, int> AllAnswers(int value)
        => QuestionCatalog.All.ToDictionary(q => q.Id, _ => value);
}
