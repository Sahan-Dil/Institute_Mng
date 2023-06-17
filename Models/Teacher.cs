using System;

namespace Institute_Mng.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNo { get; set; }
        public string EmailAddress { get; set; }
        public string AllocatedClasses { get; set; }
        public string AllocatedSubjects { get; set; }
    }
}
