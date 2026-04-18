using System;
using System.Collections.Generic;
using System.Text;

namespace ExamCenterSystem.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RollNumber { get; set; }
        public string Department { get; set; }
        public int Semester { get; set; }
    }
}