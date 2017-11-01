using System;

namespace svc_tournament.Data {
    public class AnswerData {
        public Guid Id { get; set; }
        public String Content { get; set; }
        public Guid QuestionId { get; set; }
    }
}