using System;
using System.Collections.Generic;
using System.Text;

namespace ExamCenterSystem.Models
{
    // 🧠 BASE CLASS (INHERITANCE)
    public class Person
    {
        public string Name { get; set; }
        public string RollNumber { get; set; }

        // 🎯 POLYMORPHISM (BASE METHOD)
        public virtual void ShowInfo()
        {
            System.Console.WriteLine($"Name: {Name}, Roll: {RollNumber}");
        }
    }
}
