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

            var root = BracketFactory.Convert<AnswerData>(answers);
            
            var matchups = CreateMatchups(root, question);

            await _dbContext.Questions.AddAsync(question);
            await _dbContext.MatchUps.AddRangeAsync(matchups);
            await _dbContext.Answers.AddRangeAsync(answers);
            await _dbContext.SaveChangesAsync();
            
            return Ok(new { Question = question.Id } );
        }

        private IEnumerable<MatchupData> CreateMatchups(BracketFactory.Node<AnswerData> root, QuestionData question) {
            var workspace = new Queue<BracketFactory.Node<AnswerData>>();
            workspace.Enqueue(root);

            var results = new List<MatchupData>();
            
            while(workspace.Count > 0) {
                var node = workspace.Dequeue();
                if(node.Content == null) {
                    if(node.Left != null) workspace.Enqueue(node.Left);
                    if(node.Right != null) workspace.Enqueue(node.Right);

                    var matchup = new MatchupData {
                        Id = Guid.NewGuid(),
                        Answer1Id = node?.Left?.Content?.Id ?? Guid.Empty,
                        Answer2Id = node?.Right?.Content?.Id ?? Guid.Empty,
                        QuestionId = question.Id
                    };
                    results.Add(matchup);
                }
            }
            
            results.Reverse();

            for(int i = 0, n = results.Count; i < n; ++i) {
                results[i].StartDate = DateTime.UtcNow.AddDays(i);
                results[i].EndDate = DateTime.UtcNow.AddDays(i + 1);
            }

            return results;
        } 

        
    }
}