using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lib_utilities;
using Microsoft.AspNetCore.Mvc;
using svc_tournament.Data;
using svc_tournament.Logic;
using svc_tournament.Models;

namespace svc_tournament.Controllers {
    [Route("api/[controller]")]
    public class QuestionController : Controller {
        private readonly TournamentContext _dbContext;
        public QuestionController(TournamentContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody]Ask ask) {

            var question = new QuestionData {
                Id = Guid.NewGuid(),
                Content = ask.Question,
                AskedAt = DateTime.UtcNow
            };

            var answers = ask.Answers.Shuffle().Select(a => new AnswerData {
                Id = Guid.NewGuid(),
                Content = a,
                QuestionId = question.Id,
            }).ToList();

            int numberOfRounds = (int)Math.Ceiling(Math.Log(answers.Count, 2));
            var rounds = new List<RoundData>();
            for(uint i = 0; i < numberOfRounds; ++i) {
                uint offset = (uint)(numberOfRounds - i);

                rounds.Add(new RoundData{
                    Id = Guid.NewGuid(),
                    Round = offset,
                    QuestionId = question.Id,
                    StartDate = DateTime.UtcNow.AddDays(7 * i),
                    EndDate = DateTime.UtcNow.AddDays(7 * (i+1))
                });
            }
            rounds.Reverse();

            var root = BracketFactory.Convert<AnswerData>(answers);
            
            var matchups = CreateMatchups(root, rounds);

            await _dbContext.Questions.AddAsync(question);
            await _dbContext.Rounds.AddRangeAsync(rounds);
            await _dbContext.MatchUps.AddRangeAsync(matchups);
            await _dbContext.Answers.AddRangeAsync(answers);
            await _dbContext.SaveChangesAsync();
            
            return Ok(new { Question = question.Id } );
        }

        private IEnumerable<MatchupData> CreateMatchups(BracketFactory.Node<AnswerData> root, List<RoundData> rounds) {
            var workspace = new Queue<BracketFactory.Node<AnswerData>>();
            workspace.Enqueue(root);

            var results = new List<MatchupData>();
            var roundIndex = 0;
            var roundCount = 0;
            

            while(workspace.Count > 0) {
                var node = workspace.Dequeue();
                if(node.Content == null) {
                    if(node.Left != null) workspace.Enqueue(node.Left);
                    if(node.Right != null) workspace.Enqueue(node.Right);

                    var matchup = new MatchupData {
                        Id = Guid.NewGuid(),
                        RoundId = rounds[roundIndex].Id,
                        Answer1Id = node?.Left?.Content?.Id ?? Guid.Empty,
                        Answer2Id = node?.Right?.Content?.Id ?? Guid.Empty
                    };
                    ++roundCount;

                    results.Add(matchup);

                    if(Math.Pow(2,roundIndex) <= roundCount) {
                        roundCount = 0;
                        ++roundIndex;
                    }
                }
            }

            return results;
        } 

        
    }
}