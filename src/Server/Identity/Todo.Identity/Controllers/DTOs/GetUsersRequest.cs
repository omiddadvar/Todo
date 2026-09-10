using System.ComponentModel.DataAnnotations;

namespace Todo.Identity.Controllers.DTOs;

public record GetUsersRequest
{
    public string? SearchTerm { get; set; }

    public bool? IsActive { get; set; }

    public string? PhoneSearch { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 200)]
    public int PageSize { get; set; } = 10;

    public string? SortBy { get; set; } = "CreatedAt";

    public bool SortDescending { get; set; } = true;
}
