using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamCenterSystem.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string RollNumber { get; set; }
        public string Department { get; set; }
        public string RoomName { get; set; }
    }
}