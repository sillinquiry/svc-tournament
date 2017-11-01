using Microsoft.EntityFrameworkCore;

namespace svc_tournament.Data {
    public class TournamentContext : DbContext {
        public TournamentContext() {

        }
        
        public TournamentContext(DbContextOptions<TournamentContext> options) : base(options) {
        }

        public DbSet<QuestionData> Questions { get; set; }
        public DbSet<RoundData> Rounds { get; set; }
        public DbSet<MatchupData> MatchUps { get; set; }
        public DbSet<AnswerData> Answers { get; set; }
    }
}