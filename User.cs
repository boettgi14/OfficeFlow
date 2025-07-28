using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeFlow
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool AdminStatus { get; set; }

        public User(int id, string username, string passwordHash, bool adminStatus)
        {
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
            AdminStatus = adminStatus;
        }

        public override string ToString()
        {
            return $"{Id} {Username} {PasswordHash} {(AdminStatus ? "(Admin)" : "")}";
        }
    }
}
