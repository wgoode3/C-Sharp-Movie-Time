using System;
using Microsoft.EntityFrameworkCore;


namespace MovieTime.Models
{
    public class DBContext: DbContext
    {
        public DBContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<Movie> Movies {get;set;}
        public DbSet<Join> Joins {get;set;}
    }
}