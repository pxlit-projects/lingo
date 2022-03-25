using Lingo.Common;
using Lingo.Domain;
using Microsoft.AspNetCore.Identity;

namespace Lingo.Infrastructure.Storage
{
    //DO NOT TOUCH THIS FILE!!
    /// <summary>
    /// Makes sure that the database contains a quiz master user
    /// </summary>
    internal class DatabaseSeeder
    {
        private const string QuizMasterEmail = "quizmaster@pxl.be";
        private const string QuizMasterPassword = "quizmaster";

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public DatabaseSeeder(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Seed()
        {
            //Ensure quizmaster role exists
            IdentityRole<Guid> adminRole = await _roleManager.FindByNameAsync(AppConstants.QuizmasterRoleName);
            if (adminRole == null)
            {
                adminRole = new IdentityRole<Guid>(AppConstants.QuizmasterRoleName);
                await _roleManager.CreateAsync(adminRole);
            }

            //Ensure QuizMaster user exists
            User quizMasterUser = await _userManager.FindByEmailAsync(QuizMasterEmail);
            if(quizMasterUser == null)
            {
                quizMasterUser = new User 
                { 
                    NickName = "Quiz master",
                    UserName = QuizMasterEmail,
                    Email = QuizMasterEmail
                };

                await _userManager.CreateAsync(quizMasterUser, QuizMasterPassword);
                await _userManager.AddToRoleAsync(quizMasterUser, AppConstants.QuizmasterRoleName);
            }
        }
    }
}
