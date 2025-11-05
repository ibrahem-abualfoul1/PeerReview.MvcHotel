using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PeerReview.MvcHotel.Models
{
    public class LoginRequest { public string? userName { get; set; } public string? password { get; set; } }
    public class LoginResponse { public string? token { get; set; } public string? userName { get; set; } public string? role { get; set; } public int? UserId { get; set; } }

    public class User
    {
        public int id { get; set; }
        public string? userName { get; set; }
        public string? fullName { get; set; }
        public string? email { get; set; }
        public bool isActive { get; set; }
        public int roleId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string RoleName { get; set; } = "";

        [JsonProperty("role")]
        private JToken? roleRaw // 👈 نقرأها ديناميكيًا من JSON
        {
            set
            {
                if (value == null) return;
                if (value.Type == JTokenType.String)
                {
                    // الحالة: "role": "Admin"
                    RoleName = value.ToString();
                }
                else if (value.Type == JTokenType.Object)
                {
                    // الحالة: "role": { "name": "Admin", ... }
                    RoleName = value["name"]?.ToString() ?? "";
                }
            }
        }
    }

    public class UserCreateDto { [Required] public string? userName { get; set; } public string? fullName { get; set; } public string? email { get; set; } public string? password { get; set; } public int roleId { get; set; } }

    public class UserUpdateDto { public string? fullName { get; set; } public string? email { get; set; } public bool isActive { get; set; } public int roleId { get; set; } }

    public class Role { public int id { get; set; } public string? name { get; set; } public bool canSeeAllUsers { get; set; } public bool canSeeSystemStats { get; set; } public bool canSeeAssignmentsAll { get; set; } public bool canSeeAnswersAll { get; set; } }


    public class QuestionDto
    {
        public int Id { get; set; }
        public string? TitleAr { get; set; }
        public string? DescriptionAr { get; set; }
        public string? TitleEn { get; set; }
        public string? DescriptionEn { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<QuestionItemDto>? Items { get; set; }
    }
    public class QuestionCreateDto
    {
        public string? TitleAr { get; set; }
        public string? DescriptionAr { get; set; }
        public string? TitleEn { get; set; }
        public string? DescriptionEn { get; set; }
        public int? CategoryId { get; set; }
        public List<QuestionItemCreateDto>? Items { get; set; }
    }
    public class QuestionItemCreateDto
    {
        public string? TextAr { get; set; }
        public string? TextEn { get; set; }
        public int Type { get; set; }
        public bool IsRequired { get; set; }
        public string? OptionsCsvAr { get; set; }
        public string? OptionsCsvEn { get; set; }
            public int? ParentItemId { get; set; }
            public string? ShowWhenValue { get; set; }
    }

    public class QuestionUpdateDto : QuestionCreateDto { }

    public class QuestionItemDto
    {
        public int Id { get; set; }
        public string? TextAr { get; set; }
        public string? TextEn { get; set; }
        public int Type { get; set; }
        public bool IsRequired { get; set; }
        public string? OptionsCsvAr { get; set; }
        public string? OptionsCsvEn { get; set; }
    }
    //public class QuestionItemCreateDto { public string? text { get; set; } public int type { get; set; } public bool isRequired { get; set; } public string? optionsCsv { get; set; } public int? parentItemId { get; set; } public string? showWhenValue { get; set; } }

    public class AssignRequest { public List<int>? questionIds { get; set; } public List<int>? userIds { get; set; } }
    public class AssignmentDto { public int id { get; set; } public int questionId { get; set; } public int userId { get; set; } public DateTime? assignedAt { get; set; } public bool isActive { get; set; } public QuestionDto question { get; set; } }

    public class Answer { public int id { get; set; } public int questionId { get; set; } public int? questionItemId { get; set; } public int userId { get; set; } public string? value { get; set; } public int? fileId { get; set; } public DateTime? submittedAt { get; set; } }
    public class AnswerCreateDto { public int questionId { get; set; } public int? questionItemId { get; set; } public string? value { get; set; } }
    public class AnswerUpdateDto { public string? value { get; set; } }

    // يُنصح أن يحتوي الـ Lookup على "Code" لأن الـ API تستخدمه في /api/Lookups/{code}
    public class Lookup
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        // مفتاح المسار في الـ API
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("NameAR")]
        public string? NameAr { get; set; }

        [JsonPropertyName("NameEn")]
        public string? NameEn { get; set; }

        [JsonPropertyName("TypeAr")]
        public string? TypeAr { get; set; }

        [JsonPropertyName("TypeEn")]
        public string? TypeEn { get; set; }

        [JsonPropertyName("subLookups")]
        public List<SubLookup>? SubLookups { get; set; }
    }

    public class SubLookup
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
    }

    // ===================== LOOKUPS DTOs (Requests) =====================

    // POST /api/Lookups
    // الأفضل دعم "code" هنا لأنه المعرّف الذي ستستخدمه لاحقًا في GET/PUT/DELETE /{code}
    public class LookupCreateDto
    {
        public string? Code { get; set; }

        public string? NameEn { get; set; }
        public string? NameAr { get; set; }

        public string? TypeEn { get; set; }
        public string? TypeAr { get; set; }
    }

    // PUT /api/Lookups/{code}  ← الـ code يُمرر في المسار، لذا لا نعيده في البودي
    public class LookupUpdateDto
    {
        public string? Code { get; set; }

        public string? NameEn { get; set; }
        public string? NameAr { get; set; }

        public string? TypeEn { get; set; }
        public string? TypeAr { get; set; }
    }

    // ===================== SUB LOOKUPS DTOs (Requests) =====================

    // POST /api/Lookups/{code}/sub
    // بما أن "code" موجود في المسار، في العادة يكفي إرسال الاسم فقط.
    // إن كان الـ API عندك يتطلب أيضًا lookupId، يمكنك إبقاء الخاصية Nullable.
    public class SubLookupCreateDto
    {
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }

    }

    // PUT /api/Lookups/sub/{id}
    // غالبًا يكفي التحديث بالاسم، ووجود lookupId اختياري (حسب منطقك).
    public class SubLookupUpdateDto
    {
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
    }

    public class DashboardDto { public Dictionary<string, object>? metrics { get; set; } }
    public class QuestionTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class AssignmentUpsertDto
    {
        public int? id { get; set; } // عند الإضافة = null

        [Required(ErrorMessage = "اختر المستخدم")]
        public int? userId { get; set; }

        [Required(ErrorMessage = "اختر السؤال")]
        public int? questionId { get; set; }

        public bool isActive { get; set; } = true;
    }
}
