using System;
using System.Collections.Generic;
using System.Text;

namespace Student_Performance
{
    public class UserData
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
    }
    public static class UserSession
    {
        public static UserData CurrentUser { get; private set; }

        public static void Start(int id, string username, string role)
        {
            CurrentUser = new UserData { Id = id, Username = username, Role = role };
        }

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
    public class StudentProfile
    {
        public string id { get; set; }
        public string fullName { get; set; }
        public string male { get; set; }
        public string birthDate { get; set; }
        public string contact { get; set; }
        public string group { get; set; }
        public string type { get; set; }
        public string startDate { get; set; }
        public string status { get; set; }
    }
}
