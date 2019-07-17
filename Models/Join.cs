using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MovieTime.Models
{
    public class Join
    {
        [Key]
        public int JoinId {get;set;}
        public int UserId {get;set;}
        public User Joiner {get;set;}
        public int MovieId {get;set;}
        public Movie JoinedMovie {get;set;}
    }
}


