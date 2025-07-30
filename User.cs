using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeFlow
{
    /// <summary>
    /// Represents a user with an ID, username, password hash, and administrative status.
    /// </summary>
    /// <remarks>This class provides properties to access and modify user details, including their unique
    /// identifier, username, hashed password, and whether they have administrative privileges. Instances of this class
    /// can be used to manage user information in applications that require user authentication and role-based access
    /// control.</remarks>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the username associated with the user.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the hashed representation of the user's password.
        /// </summary>
        public string PasswordHash { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the administrative status is enabled.
        /// </summary>
        public bool AdminStatus { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with the specified user details.
        /// </summary>
        /// <param name="id">The unique identifier for the user.</param>
        /// <param name="username">The username associated with the user. Cannot be null or empty.</param>
        /// <param name="passwordHash">The hashed password for the user. Cannot be null or empty.</param>
        /// <param name="adminStatus">A value indicating whether the user has administrative privileges.  <see langword="true"/> if the user is an
        /// administrator; otherwise, <see langword="false"/>.</param>
        public User(int id, string username, string passwordHash, bool adminStatus)
        {
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
            AdminStatus = adminStatus;
        }

        /// <summary>
        /// Returns a string representation of the user, including the ID, username, and admin status.
        /// </summary>
        /// <returns>A string containing the user's ID, username, and a designation indicating admin status if applicable.</returns>
        public override string ToString()
        {
            return $"{Id} {Username} {PasswordHash} {(AdminStatus ? "(Admin)" : "")}";
        }
    }
}
