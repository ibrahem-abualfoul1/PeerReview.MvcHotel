
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
namespace PeerReview.MvcHotel.Models
{
    public class LoginRequest { public string? userName { get; set; } public string? password { get; set; } }
    public class LoginResponse { public string? token { get; set; } public string? userName { get; set; } public string? role { get; set; } }

    public class User { public int id { get; set; } public string? userName { get; set; } public string? fullName { get; set; } public string? email { get; set; } public bool isActive { get; set; } public int roleId { get; set; } }
    public class UserCreateDto { [Required] public string? userName { get; set; } public string? fullName { get; set; } public string? email { get; set; } public string? password { get; set; } public int roleId { get; set; } }
    public class UserUpdateDto { public string? fullName { get; set; } public string? email { get; set; } public bool isActive { get; set; } public int roleId { get; set; } }

    public class Role { public int id { get; set; } public string? name { get; set; } public bool canSeeAllUsers { get; set; } public bool canSeeSystemStats { get; set; } public bool canSeeAssignmentsAll { get; set; } public bool canSeeAnswersAll { get; set; } }

    public class QuestionDto { public int id { get; set; } public string? title { get; set; } public string? description { get; set; } public int? categoryId { get; set; } public List<QuestionItemDto>? items { get; set; } }
    public class QuestionCreateDto { public string? title { get; set; } public string? description { get; set; } public int? categoryId { get; set; } public List<QuestionItemCreateDto>? items { get; set; } }
    public class QuestionUpdateDto : QuestionCreateDto { }
    public class QuestionItemDto { public int id { get; set; } public string? text { get; set; } public int type { get; set; } public bool isRequired { get; set; } public string? optionsCsv { get; set; } public int? parentItemId { get; set; } public string? showWhenValue { get; set; } }
    public class QuestionItemCreateDto { public string? text { get; set; } public int type { get; set; } public bool isRequired { get; set; } public string? optionsCsv { get; set; } public int? parentItemId { get; set; } public string? showWhenValue { get; set; } }

    public class AssignRequest { public List<int>? questionIds { get; set; } public List<int>? userIds { get; set; } }
    public class AssignmentDto { public int id { get; set; } public int questionId { get; set; } public int userId { get; set; } public DateTime? assignedAt { get; set; } public bool isActive { get; set; } }

    public class Answer { public int id { get; set; } public int questionId { get; set; } public int? questionItemId { get; set; } public int userId { get; set; } public string? value { get; set; } public int? fileId { get; set; } public DateTime? submittedAt { get; set; } }
    public class AnswerCreateDto { public int questionId { get; set; } public int? questionItemId { get; set; } public string? value { get; set; } }
    public class AnswerUpdateDto { public string? value { get; set; } }

    public class Lookup { public int id { get; set; } public string? name { get; set; } public string? type { get; set; } public List<SubLookup>? subLookups { get; set; } }
    public class SubLookup { public int id { get; set; } public int lookupId { get; set; } public string? name { get; set; } }
    public class LookupCreateDto { public string? name { get; set; } public string? type { get; set; } }
    public class LookupUpdateDto { public string? name { get; set; } public string? type { get; set; } }
    public class SubLookupCreateDto { public int lookupId { get; set; } public string? name { get; set; } }
    public class SubLookupUpdateDto { public int lookupId { get; set; } public string? name { get; set; } }

    public class DashboardDto { public Dictionary<string, object>? metrics { get; set; } }
    public class QuestionTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
