using E_store.Models;
using E_store.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace E_store.Services
{
    class AuthService
    {
        private const string ROLE_USER = "User";
        private const string ROLE_MANAGER = "Manager";
        private const string ROLE_ADMIN = "Admin";
        public static AuthService Instance { get; } = new AuthService();

        private readonly UserRepository userRepository = new UserRepository();
        private readonly RoleRepository roleRepository = new RoleRepository();

        public User CurrentUser { get; private set; }
        public List<Role> CurrentRoles { get; private set; } = new List<Role>();
        public List<Permission> CurrentPermissions { get; private set; } = new List<Permission>();

        public bool Login(string username, string password)
        {
            User user = userRepository.GetByUsername(username);

            if (user == null)
                return false;

            bool ispravno = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!ispravno)
                return false;

            CurrentUser = user;
            CurrentRoles = roleRepository.GetRolesForUser(user.Id);
            CurrentPermissions = roleRepository.GetPermissionsForUser(user.Id);

            return true;
        }

        //TODO 
        public (bool succesful, string message) Register(User newUser)
        {
            if (userRepository.GetByUsername(newUser.Username) != null)
                return (false, "Username already exists.");

            userRepository.AddUser(newUser);

            User savedUser = userRepository.GetByUsername(newUser.Username);

            Role defaultRola = roleRepository.GetRoleByName(ROLE_USER);

            if (defaultRola == null)
                return (false, "Default role 'User' doesn't exist in db.");

            roleRepository.AddRoleToUser(savedUser.Id, defaultRola.Id);

            bool successfulLogin = Login(newUser.Username, newUser.Password);

            return (successfulLogin, successfulLogin ? "Registration succussful." : "Registration succussful, but auto-login didn't.");
        }

        public (bool succesful, string message) AddManager(User newUser)
        {
            if (!HasRole(ROLE_ADMIN))
                return (false, "You don't have role for this action.");

            if (userRepository.GetByUsername(newUser.Username) != null)
                return (false, "Username already exists.");

            userRepository.AddUser(newUser);

            User savedUser = userRepository.GetByUsername(newUser.Username);

            Role managerRole = roleRepository.GetRoleByName(ROLE_MANAGER);

            if (managerRole == null)
                return (false, "role 'Manager' doesn't exist in db.");

            roleRepository.AddRoleToUser(savedUser.Id, managerRole.Id);

            return (true, "Manager added successfully.");
        }

        //TODO
        public bool HasRole(string roleName)
        {
            return CurrentRoles.Any(r => r.RoleName.Equals(roleName, System.StringComparison.OrdinalIgnoreCase));
        }

        public bool HasPermission(string permissionDescription)
        {
            return CurrentPermissions.Any(p => p.Description.Equals(permissionDescription, System.StringComparison.OrdinalIgnoreCase));
        }

        public void Logout()
        {
            CurrentUser = null;
            CurrentRoles.Clear();
            CurrentPermissions.Clear();
        }
        
    }
}
