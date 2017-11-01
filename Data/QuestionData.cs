using System;

namespace svc_tournament.Data {
    public class QuestionData {
        public Guid Id { get; set; }
        public String Content { get; set; }
        public Guid AskedBy { get; set; }
        public DateTime AskedAt { get; set; }
        public DateTime ClosedAt { get; set; }
        public String ClosedReason { get; set; }
    }
}