using Libs.Data;
using Libs.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Repositories
{
    public interface IAccountRepository : IRepository<User>
    {
        //Task<bool> AddAsync(User user);
        Task<IdentityResult> Create(User user, string PasswordHash);
    }
    public class AccountRepository : RepositoryBase<User>, IAccountRepository
    {
        private readonly UserManager<User> _userManager;

        public AccountRepository(ApplicationDbContext dbContext, UserManager<User> _userManager) : base(dbContext)
        {
            this._userManager= _userManager;
        }

        //public virtual async Task<bool> AddAsync(User user)
        //{
        //    await dbset.AddAsync(user);
        //    return true;
        //}

        public  async Task<IdentityResult> Create(User user,string PasswordHash)
        {
            return await _userManager.CreateAsync(user, PasswordHash);
        }
    }
}
