using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamCenterSystem.Models
{
    public class Result
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string RollNumber { get; set; }
        public string SubjectName { get; set; }
        public int Marks { get; set; }
        public string Grade { get; set; }
    }
}