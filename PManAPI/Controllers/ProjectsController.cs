using Microsoft.AspNetCore.Authorization;
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
    public class ProjectsController : ControllerBase
    {

        private UserManager<ApplicationUser> _userManager;
        private AppDbContext _dbContext;

        public ProjectsController(UserManager<ApplicationUser> userManager, AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;

        }

        // GET: api/<ProjectsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user.IsAdmin)
            {
                return Ok(_dbContext.Projects.ToListAsync());
            }
            else 
            {

                var query = await _dbContext.Projects.Include(p => p.ProjectUsers).ToListAsync();
                var projects = new List<Project>();
                foreach(Project p in query)
                {
                   foreach(ProjectUser c in p.ProjectUsers)
                   {
                        if(c.UserId == user.Id)
                        {
                            projects.Add(new Project{Id = p.Id, Name = p.Name, Description = p.Description });
                        }
                   }

                }
                return Ok(projects);
            }
        }

        // GET api/<ProjectsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var project = await _dbContext.Projects.Include(b => b.ProjectUsers).Where(a => a.Id == id).FirstOrDefaultAsync();
            if (project == null)
                return NotFound();

            if (user.IsAdmin)
                return Ok(new Project { Id = project.Id, Name = project.Name, Description = project.Description });


            foreach (ProjectUser u in project.ProjectUsers)
            {
                if (u.UserId == user.Id)
                    return Ok(new Project { Id = project.Id, Name = project.Name, Description = project.Description});
            }

            return Forbid();

        }
        //Get all tickets within project
        [HttpGet]
        [Route("/api/projects/{id}/issues")]
        public async Task<IActionResult> GetProjectIssues(int id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var issues = await _dbContext.Issues.Where(b => b.Project.Id == id).ToListAsync();
            if (issues == null || issues.Count <= 0)
                return NotFound();

            if (user.IsAdmin)
                return Ok(issues);

            var UserIssues = issues.Where(b => b.AssignedTo == user.Id).ToList();
            if (UserIssues == null || UserIssues.Count <= 0)
                return NotFound();

            return Ok(UserIssues);
        }

        [HttpGet]
        [Route("/api/projects/{id}/users")]
        public async Task<IActionResult> GetProjectUsers(int id)
        {

            var _project = await _dbContext.Projects.Include(b => b.ProjectUsers).ThenInclude(c => c.User).Where(a => a.Id == id).FirstOrDefaultAsync();
            if (_project == null)
                return NotFound("Project not found");

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (user.IsAdmin || _project.ProjectUsers.Where(a => a.UserId == user.Id && a.Role <= 2).FirstOrDefault() != null)
            {
                var allUsers = new List<DTOUserProject>();
                foreach(ProjectUser u in _project.ProjectUsers)
                {
                    allUsers.Add(new DTOUserProject { UserMail = u.User.Email, role = u.Role });
                }
                return Ok(allUsers);
            }

            return Forbid();
        }

        // POST api/<ProjectsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DTOProject project)
        {

            ApplicationUser user =  await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var _project = new Project
            {
                Name = project.Name,
                Description = project.Description,
            };

            var _projectUser = new ProjectUser
            {
                Project = _project,
                User = user,
                Role = 1
            };

            _project.ProjectUsers = new List<ProjectUser>() { _projectUser };

            _dbContext.Add(_project);
            await _dbContext.SaveChangesAsync();


            return CreatedAtAction(nameof(GetById),
                new { id = _project.Id },
                _project
            );
        }


        //Add user to project
        [HttpPost]
        [Route("/api/projects/{id}/users")]
        public async Task<IActionResult> AddUserToProject(int id, [FromBody] DTOUserProject addUserProject)
        {

            var _project = await _dbContext.Projects.Include(b => b.ProjectUsers).Where(a => a.Id == id).FirstOrDefaultAsync();
            if (_project == null)
                return NotFound("Project not found");

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            ApplicationUser TargetUser = await _userManager.FindByEmailAsync(addUserProject.UserMail);

            if(TargetUser == null)
                return NotFound("User not found");


            var _ProjectUser = _project.ProjectUsers?.Where(a => a.UserId == user.Id).FirstOrDefault();
            if (_ProjectUser == null && !user.IsAdmin)
                return Forbid();

            if (user.IsAdmin || (_ProjectUser.Role <= 2 && addUserProject.role > _ProjectUser.Role))
            {
                if(_project.ProjectUsers?.Where(a => a.UserId == TargetUser.Id).FirstOrDefault() != null)
                    return BadRequest("User is already assigned in this project");

                var newUser = new ProjectUser
                {
                    Project = _project,
                    User = TargetUser,
                    Role = addUserProject.role
                };

                _project.ProjectUsers.Add(newUser);

                await _dbContext.SaveChangesAsync();

                return Ok("User " + TargetUser.Email + " was successfully added to project");
            }

            return Forbid();
        }


        //Update user in project
        [HttpPut]
        [Route("/api/projects/{id}/users")]
        public async Task<IActionResult> UpdateUserInProject(int id, [FromBody] DTOUserProject addUserProject)
        {

            var _project = await _dbContext.Projects.Include(b => b.ProjectUsers).Where(a => a.Id == id).FirstOrDefaultAsync();
            if (_project == null)
                return NotFound("Project not found");

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            ApplicationUser TargetUser = await _userManager.FindByEmailAsync(addUserProject.UserMail);

            if (TargetUser == null)
                return NotFound("User not found");


            var _ProjectUser = _project.ProjectUsers?.Where(a => a.UserId == user.Id).FirstOrDefault();
            if (_ProjectUser == null && !user.IsAdmin)
                Forbid();

            if (user.IsAdmin || (_ProjectUser?.Role <= 2 && addUserProject.role > _ProjectUser.Role))
            {
                if (_project.ProjectUsers?.Where(a => a.UserId == TargetUser.Id).FirstOrDefault() == null)
                    return BadRequest("User is not in your project");

                var newUser = new ProjectUser
                {
                    Project = _project,
                    User = TargetUser,
                    Role = addUserProject.role
                };

                _project.ProjectUsers.Remove(_project.ProjectUsers.Where(a => a.UserId == TargetUser.Id).First());
                _project.ProjectUsers.Add(newUser);

                await _dbContext.SaveChangesAsync();

                return NoContent();
            }

            return Forbid();
        }



        // PUT api/<ProjectsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] DTOProject project)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var _project = await _dbContext.Projects.Include(b => b.ProjectUsers).Where(a => a.Id == id).FirstOrDefaultAsync();
            if (_project == null) return NotFound();

            var _ProjectUser = _project.ProjectUsers?.Where(a => a.UserId == user.Id && a.Role == 1).FirstOrDefault();


            if (user.IsAdmin || _ProjectUser != null)
            {
                _project.Name = project.Name;
                _project.Description = project.Description;
                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch
                {
                    if (_dbContext.Projects.Find(id) == null)
                        return NotFound();
                    throw;
                }
                return NoContent();
                }

            return Forbid();
        }


        //Delete user in project
        [HttpDelete]
        [Route("/api/projects/{id}/users")]
        public async Task<IActionResult> DeleteUserInProject(int id, DTODeleteUserProject deleteUserProject)
        {


            var _project = await _dbContext.Projects.Include(b => b.ProjectUsers).Where(a => a.Id == id).FirstOrDefaultAsync();
            if (_project == null)
                return NotFound("Project not found");

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            ApplicationUser TargetUser = await _userManager.FindByEmailAsync(deleteUserProject.UserMail);

            if (TargetUser == null)
                return NotFound("User not found");


            var _ProjectUser = _project.ProjectUsers?.Where(a => a.UserId == user.Id).FirstOrDefault();
            if (_ProjectUser == null && !user.IsAdmin)
                return Forbid();
            var _TargetProjectUser = _project.ProjectUsers?.Where(a => a.UserId == TargetUser.Id).FirstOrDefault();

            if (user.IsAdmin || _ProjectUser.Role <= 2)
            {
                if (_TargetProjectUser == null)
                    return BadRequest("User is not in your project.");
                if (_TargetProjectUser.Role == 1)
                    return BadRequest("You cannot delete a creator from the project.");
                if (_ProjectUser.Role >= _TargetProjectUser.Role || user.IsAdmin)
                    return Forbid();

                _project.ProjectUsers.Remove(_TargetProjectUser);

                await _dbContext.SaveChangesAsync();

                return Ok("User " + TargetUser.Email + " was successfully updated in project");
            }

            return Forbid();
        }
        // DELETE api/<ProjectsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var project = await _dbContext.Projects.Include(b => b.ProjectUsers).Where(a => a.Id == id).FirstAsync();
            if (project == null) return NotFound();

            if(user.IsAdmin)
            {
                _dbContext.Projects.Remove(project);
                await _dbContext.SaveChangesAsync();
                return Ok(project);
            }

            foreach (ProjectUser u in project.ProjectUsers)
            {
                if (u.UserId == user.Id && u.Role == 1)
                {
                    _dbContext.Projects.Remove(project);
                    await _dbContext.SaveChangesAsync();
                    return Ok(project);
                }
            }

            return Forbid();
        }
    }
}
