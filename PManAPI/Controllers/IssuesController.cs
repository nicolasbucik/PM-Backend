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
                    a_list_i.Add(new DTOIssue
                    {
                        Name = i.Name,
                        Description = i.Description,
                        AssignedTo = await _userManager.GetEmailAsync(await _userManager.FindByIdAsync(i.AssignedTo)),
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
                list_i.Add(new DTOIssue
                {
                    Name = i.Name,
                    Description = i.Description,
                    AssignedTo = await _userManager.GetEmailAsync(await _userManager.FindByIdAsync(i.AssignedTo)),
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
        public async Task<IActionResult> GetbyId(int id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var issue = await _dbContext.Issues.Include(b => b.Project).ThenInclude(c => c.ProjectUsers).Where(d => d.id == id).FirstOrDefaultAsync();
            if (issue == null)
                return NotFound("No issues found");

            if (user.IsAdmin)
            {
                return Ok(new DTOIssue()
                {
                    Name = issue.Name,
                    Description = issue.Description,
                    AssignedTo = await _userManager.GetEmailAsync(await _userManager.FindByIdAsync(issue.AssignedTo)),
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
                return Ok(new DTOIssue()
                {
                    Name = issue.Name,
                    Description = issue.Description,
                    AssignedTo = await _userManager.GetEmailAsync(await _userManager.FindByIdAsync(issue.AssignedTo)),
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

            var IssueProjectUser = project.ProjectUsers.Where(c => c.UserId == user.Id).FirstOrDefault();

            if (user.IsAdmin ||( IssueProjectUser != null && IssueProjectUser.Role <= 2))
            {

            }
        }
    }
}
