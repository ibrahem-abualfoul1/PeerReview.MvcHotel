
using System.Collections.Generic;

namespace PeerReview.MvcHotel.Models
{
    public class AnswerSetPost
    {
        public int QuestionId { get; set; }
        // key: QuestionItemId, value: string value
        public Dictionary<int, string?> Values { get; set; } = new();
    }
}
