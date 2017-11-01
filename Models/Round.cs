using System;
using System.Collections.Generic;

namespace svc_tournament.Models {
    public class Round {
        public Guid Id { get; set; }
        public String Question { get; set; }
        public List<Matchup> Matchups { get; set; }
    }
}