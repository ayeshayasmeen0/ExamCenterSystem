using System;
using System.Collections.Generic;
using System.Text;

namespace ExamCenterSystem.Models
{
    public class Person
    {
        public string Name { get; set; }
        public string RollNumber { get; set; }

        public virtual void ShowInfo()
        {
            Console.WriteLine($"Name: {Name}, Roll: {RollNumber}");
        }
    }
}