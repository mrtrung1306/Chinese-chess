using Libs.Entity;
using Libs.Repositories;
using Libs.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Services
{
    
    public class UserService
    {
        private ApplicationDbContext applicationDbContext;
        private IAccountRepository accountRepository;
        private readonly UserManager<User> _userManager;
        //private IUnitOfWork unitOfWork;
        public UserService(ApplicationDbContext applicationDbContext, UserManager<User> _userManager)
        {
            this.applicationDbContext = applicationDbContext;
            this.accountRepository = new AccountRepository(applicationDbContext, _userManager);
            this._userManager = _userManager;
        }
        public async Task CompleteAsync()
        {
            await applicationDbContext.SaveChangesAsync();
        }
        //public virtual async Task<bool> AddAsync(User user)
        //{
        //    await accountRepository.AddAsync(user);
        //    return true;
        //}
        public async Task<IdentityResult> Create(User user,string PasswordHash)
        {
            return await accountRepository.Create(user, PasswordHash);
        }
    }
}
