using System;

namespace svc_tournament.Data {
    public class MatchupData {
        public Guid Id { get; set; }
        public Guid Answer1Id { get; set; }
        public Guid Answer2Id { get; set; }
        public Guid QuestionId { get; set; }
        public Guid NextMatchupId { get; set; }
        public Guid RoundId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}