using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using svc_tournament.Data;
using svc_tournament.Models;

namespace svc_tournament.Controllers {
    [Route("api/[controller]")]
    public class RoundController : Controller {
        private readonly TournamentContext _dbContext;

        public RoundController(TournamentContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpGet("current/{questionId}")]
        public IActionResult GetCurrentRound(Guid questionId) {
            var questionData = _dbContext.Questions.Single(q => q.Id == questionId);
            var roundData = _dbContext.Rounds.Where(r => r.QuestionId == questionId).Single(r => r.StartDate <= DateTime.UtcNow && r.EndDate >= DateTime.UtcNow);
            var matchupData = _dbContext.MatchUps.Where(m => m.RoundId == roundData.Id);

            var answerIds = new List<Guid>();
            Parallel.ForEach(matchupData, m => {
                answerIds.Add(m.Answer1Id);
                answerIds.Add(m.Answer2Id);
            });

            var answersData = _dbContext.Answers.Where(a => answerIds.Contains(a.Id));

            var round = new Round {
                Id = roundData.Id,
                Matchups = matchupData.Select(m => new Matchup {
                    Id = m.Id,
                    Answers = answersData.Where(a => a.Id == m.Answer1Id || a.Id == m.Answer2Id).Select( a => new Answer {
                        Id = a.Id,
                        Content = a.Content
                    }).ToList()
                }).ToList()
            };

            return Ok(round);
        }

        [HttpGet]
        public IActionResult GetDetails() {
            return null;
        }

    }
}