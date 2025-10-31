namespace PeerReview.MvcHotel.Models
{

    public enum QuestionType
    {
        Text = 1,
        YesNo = 2,
        SingleChoice = 3,
        MultiChoice = 4,
        Number = 5,
        Date = 6,
        File = 7
    }

    public class QuestionItem
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public int Type { get; set; }
        public bool IsRequired { get; set; }
        public string? OptionsCsv { get; set; }
    }

    public class QuestionGroup
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int CategoryId { get; set; }
        public List<QuestionItem> Items { get; set; } = new();
    }

    public class SurveyViewModel
    {
        public string HeaderTitle { get; set; } = "استطلاع الفنادق";
        public string HeaderDescription { get; set; } = "مساعدتك في تحسين خدماتنا";
        public List<QuestionGroup> Groups { get; set; } = new();
    }
}
