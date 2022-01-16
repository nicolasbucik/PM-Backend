using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMan.AuthService.Services;
using PMan.DataService;
using PMan.Core.DTOModels;
using PMan.Core.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace PManAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IssuesController : ControllerBase
    {

        private UserManager<ApplicationUser> _userManager;
        private AppDbContext _dbContext;

        public IssuesController(UserManager<ApplicationUser> userManager, AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllIssues()
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var issues = await _dbContext.Issues.Include(b => b.Project).ToListAsync();
                if (issues == null || issues.Count() == 0)
                    return NotFound("No issues found");

            if (user.IsAdmin)
            {
                var a_list_i = new List<DTOIssue>();

                foreach(Issue i in issues)
                {
                    var target = await _userManager.FindByEmailAsync(i.AssignedTo);
                    a_list_i.Add(new DTOIssue
                    {
                        IssueId = i.id,
                        Name = i.Name,
                        Description = i.Description,
                        AssignedTo = target.Email,
                        StartTime = i.StartTime,
                        EndTime = i.EndTime,
                        Priority = i.Priority,
                        Type = i.Type,
                        IsFinished = i.IsFinished
                    });
                }
                return Ok(a_list_i);
            }

            var UserIssues = issues.Where(a => a.AssignedTo == user.Id);
            if (UserIssues == null || issues.Count == 0)
                return NotFound("No issues found");

            var list_i = new List<DTOIssue>();

            foreach (Issue i in issues)
            {
                var target = await _userManager.FindByEmailAsync(i.AssignedTo);
                list_i.Add(new DTOIssue
                {
                    IssueId = i.id,
                    Name = i.Name,
                    Description = i.Description,
                    AssignedTo = target.Email,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    Priority = i.Priority,
                    Type = i.Type,
                    IsFinished = i.IsFinished
                });
            }
            return Ok(list_i);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var issue = await _dbContext.Issues.Include(b => b.Project).ThenInclude(c => c.ProjectUsers).Where(d => d.id == id).FirstOrDefaultAsync();
            if (issue == null)
                return NotFound("No issues found");

            if (user.IsAdmin)
            {
                var target = await _userManager.FindByEmailAsync(issue.AssignedTo);
                return Ok(new DTOIssue()
                {
                    IssueId = issue.id,
                    Name = issue.Name,
                    Description = issue.Description,
                    AssignedTo = target.Email,
                    StartTime = issue.StartTime,
                    EndTime = issue.EndTime,
                    Priority = issue.Priority,
                    Type = issue.Type,
                    IsFinished = issue.IsFinished,
                    ProjectId = issue.ProjectId
                });
            }

            var IssueProjectUser = issue.Project.ProjectUsers.Where(c => c.UserId == user.Id).FirstOrDefault();

            if (user.Id == issue.AssignedTo || (IssueProjectUser != null && IssueProjectUser.Role <= 2 ))
            {
                var target = await _userManager.FindByEmailAsync(issue.AssignedTo);
                return Ok(new DTOIssue()
                {
                    IssueId = issue.id,
                    Name = issue.Name,
                    Description = issue.Description,
                    AssignedTo = target.Email,
                    StartTime = issue.StartTime,
                    EndTime = issue.EndTime,
                    Priority = issue.Priority,
                    Type = issue.Type,
                    IsFinished = issue.IsFinished,
                    ProjectId = issue.ProjectId
                });
            }
            
            return Forbid();
        }

        [HttpPost]
        public async Task<IActionResult> CreateIssue([FromBody] DTOIssue IncIssue)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var project = await _dbContext.Projects.Include(b => b.ProjectUsers).Where(a => a.Id == IncIssue.ProjectId).FirstOrDefaultAsync();
            if (project == null)
                return NotFound("Project not found");

            var TargetUser = await _userManager.FindByEmailAsync(IncIssue.AssignedTo);
            if (TargetUser == null)
                return NotFound("User not found");

            if (project.ProjectUsers.Where(c => c.UserId == TargetUser.Id).FirstOrDefault() == null)
                return NotFound("Target user is not in the project");

            var IssueProjectUser = project.ProjectUsers.Where(c => c.UserId == user.Id).FirstOrDefault();

            if (user.IsAdmin ||( IssueProjectUser != null && IssueProjectUser.Role <= 2))
            {
                var issue = new Issue()
                {
                    Project = project,
                    Name = IncIssue.Name,
                    Description = IncIssue.Description,
                    StartTime = IncIssue.StartTime,
                    EndTime = IncIssue.EndTime,
                    IsFinished = IncIssue.IsFinished,
                    AssignedTo = IncIssue.AssignedTo,
                    Priority = IncIssue.Priority,
                    Type = IncIssue.Type

                };
                _dbContext.Issues.Add(issue);
                await _dbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById),
                    new { id = issue.id },
                    issue);
            }
            
            if((IssueProjectUser != null && IssueProjectUser.Role == 3))
            {
                var issue = new Issue()
                {
                    Project = project,
                    Name = IncIssue.Name,
                    Description = IncIssue.Description,
                    StartTime = IncIssue.StartTime,
                    EndTime = IncIssue.EndTime,
                    IsFinished = IncIssue.IsFinished,
                    AssignedTo = user.Email,
                    Priority = IncIssue.Priority,
                    Type = IncIssue.Type

                };
                _dbContext.Issues.Add(issue);
                await _dbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById),
                    new { id = issue.id },
                    issue);
            }

            return Forbid();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIssue(int id, DTOUpdateIssue IncIssue)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var issue = await _dbContext.Issues.Include(a => a.Project).ThenInclude(c => c.ProjectUsers).Where(b => b.id == id).FirstOrDefaultAsync();
            if (issue == null)
                return NotFound("Issue not found");

            var IssueProjectUser = issue.Project.ProjectUsers.Where(c => c.UserId == user.Id).FirstOrDefault();

            var TargetUser = await _userManager.FindByEmailAsync(IncIssue.AssignedTo);
            if (TargetUser == null)
                return NotFound("User not found");

            if (issue.Project.ProjectUsers.Where(c => c.UserId == TargetUser.Id).FirstOrDefault() == null)
                return NotFound("Target user is not in the project");


            if (user.IsAdmin || (IssueProjectUser != null && IssueProjectUser.Role <= 2))
            {
                issue.Name = IncIssue.Name;
                issue.Description = IncIssue.Description;
                issue.StartTime = IncIssue.StartTime;
                issue.EndTime = IncIssue.EndTime;
                issue.IsFinished = IncIssue.IsFinished;
                issue.AssignedTo = IncIssue.AssignedTo;
                issue.Priority = IncIssue.Priority;
                issue.Type = IncIssue.Type;

                await _dbContext.SaveChangesAsync();

                return NoContent();
            };

            if(issue.AssignedTo == user.Email && IssueProjectUser !=null)
            {
                issue.Name = IncIssue.Name;
                issue.Description = IncIssue.Description;
                issue.StartTime = IncIssue.StartTime;
                issue.EndTime = IncIssue.EndTime;
                issue.IsFinished = IncIssue.IsFinished;
                issue.Priority = IncIssue.Priority;
                issue.Type = IncIssue.Type;

                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            return Forbid();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssue(int id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var issue = await _dbContext.Issues.Include(a => a.Project).ThenInclude(c => c.ProjectUsers).Where(b => b.id == id).FirstOrDefaultAsync();
            if (issue == null)
                return NotFound("Issue not found");

            var IssueProjectUser = issue.Project.ProjectUsers.Where(c => c.UserId == user.Id).FirstOrDefault();

            if (user.IsAdmin || (IssueProjectUser != null && IssueProjectUser.Role <= 2))
            {
                _dbContext.Remove(issue);
                await _dbContext.SaveChangesAsync();

                return Ok(
                new Issue()
                {
                    Name = issue.Name,
                    Description = issue.Description,
                    StartTime = issue.StartTime,
                    EndTime = issue.EndTime,
                    IsFinished = issue.IsFinished,
                    AssignedTo = issue.AssignedTo,
                    Priority = issue.Priority,
                    Type = issue.Type
                });
            }

            if (issue.AssignedTo == user.Email && IssueProjectUser != null)
            {
                _dbContext.Remove(issue);
                await _dbContext.SaveChangesAsync();
                return Ok(
                    new Issue()
                    {
                        Name = issue.Name,
                        Description = issue.Description,
                        StartTime = issue.StartTime,
                        EndTime = issue.EndTime,
                        IsFinished = issue.IsFinished,
                        AssignedTo = issue.AssignedTo,
                        Priority = issue.Priority,
                        Type = issue.Type
                    });
            }
            return Forbid();
        }
    }
}
