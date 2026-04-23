using System;
using System.Collections.Generic;
using System.Text;

namespace ExamCenterSystem.Models
{
    public class Student : Person
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public string Semester { get; set; }

        public override void ShowInfo()
        {
            Console.WriteLine($"ID: {Id} | Name: {Name} | Roll: {RollNumber} | Dept: {Department} | Sem: {Semester}");
        }
    }
}