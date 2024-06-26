﻿using Libs.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Room> Room { get; set; }
        public DbSet<UserInRoom> UserInRoom { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<RefeshToken> RefeshToken { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
