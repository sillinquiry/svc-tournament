using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using svc_tournament.Data;
using svc_tournament.Models;

namespace svc_tournament.Controllers {
    [Route("api/[controller]")]
    public class MatchUpController : Controller {
        private readonly TournamentContext _dbContext;

        public MatchUpController(TournamentContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpGet("current/{questionId}")]
        public IActionResult GetCurrent(Guid questionId) {
            var questionData = _dbContext.Questions.Single(q => q.Id == questionId);
            var matchupData = _dbContext.MatchUps.Single(m => m.QuestionId == questionData.Id && m.StartDate <= DateTime.UtcNow && m.EndDate > DateTime.UtcNow);

            var answerIds = new List<Guid>{
                matchupData.Answer1Id,
                matchupData.Answer2Id
            };

            var answersData = _dbContext.Answers.Where(a => answerIds.Contains(a.Id));

            var matchup = new Matchup {
                Id = matchupData.Id,
                Answers = answersData.Where( a => a.Id == matchupData.Answer1Id || a.Id == matchupData.Answer2Id ).Select( a => new Answer {
                    Id = a.Id,
                    Content = a.Content
                }).ToList()
            };

            return Ok(matchup);
        }

        [HttpGet]
        public IActionResult GetDetails() {
            return null;
        }

    }
}