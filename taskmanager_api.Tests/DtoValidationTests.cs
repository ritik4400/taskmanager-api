using System.ComponentModel.DataAnnotations;
using taskmanager_api.DTOs;
using taskmanager_api.Entities;
using Xunit;
using static taskmanager_api.Entities.TaskItem;

namespace taskmanager_api.Tests;

public class DtoValidationTests
{
    [Theory]
    [InlineData("Finish API", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    public void CreateTaskDto_Title_IsValidatedCorrectly(string title, bool expectedValid)
    {
        var dto = new CreateTaskDto { Title = title };

        Assert.Equal(expectedValid, IsValid(dto));
    }

    [Theory]
    [InlineData("ritik@test.com", "12345678", true)]
    [InlineData("not-an-email", "12345678", false)]
    [InlineData("ritik@test.com", "1234567", false)]   // 7 chars: too short
    public void RegisterDto_EmailAndPassword_AreValidatedCorrectly(string email, string password, bool expectedValid)
    {
        var dto = new RegisterDto { FullName = "Ritik", Email = email, Password = password };

        Assert.Equal(expectedValid, IsValid(dto));
    }

    [Fact]
    public void CreateTaskDto_UndefinedStatusNumber_IsInvalid()
    {
        var dto = new CreateTaskDto { Title = "x", Status = (TaskItemStatus)99 };

        Assert.False(IsValid(dto));
    }

    // Runs the same attribute checks that ASP.NET runs automatically for [ApiController]
    private static bool IsValid(object dto)
    {
        var context = new ValidationContext(dto);
        return Validator.TryValidateObject(dto, context, new List<ValidationResult>(), validateAllProperties: true);
    }
}