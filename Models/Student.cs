using System;
using System.Collections.Generic;
using System.Text;

namespace ExamCenterSystem.Models
{
    // 🧠 INHERITANCE HERE
    public class Student : Person
    {
        public string Department { get; set; }
        public int Semester { get; set; }

        // 🎯 POLYMORPHISM (METHOD OVERRIDE)
        public override void ShowInfo()
        {
            System.Console.WriteLine(
                $"Name: {Name}, Roll: {RollNumber}, Dept: {Department}, Sem: {Semester}"
            );
        }
    }
}