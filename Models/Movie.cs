using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MovieTime.Models
{
    public class Movie
    {
        [Key]
        public int MovieId {get;set;}
        [Required]
        public string Title {get;set;}
        [Required]
        public int Year {get;set;}
        [Required]
        public string Address {get;set;}
        [Required]
        public DateTime StartTime {get;set;}
        [Required]
        public int Duration {get;set;}
        public int PlannerId {get;set;}
        public User Planner {get;set;}
        public List<Join> AttendingUsers {get;set;}

        public void Display()
        {
            Console.WriteLine($"{Title} {Year} {StartTime}");
        }
    }
}