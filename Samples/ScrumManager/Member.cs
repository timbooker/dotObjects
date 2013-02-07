using System;
using System.Collections.Generic;

namespace dotObjects.Samples.ScrumManager
{
    [Serializable]
    public class Member
    {
        private string name;
        private string email;
        private string username;
        private string password;

        public Member(string name, string email, string username, string password, string passwordConfirmation)
        {
            Name = name;
            Email = email;
            Username = username;
            Password = password;
            if (!password.Equals(passwordConfirmation))
                throw new ArgumentException("The password and confirmation doesn't match.");
        }

        public void ChangePassword(string oldPassword, string newPassword, string newPasswordConfirmation)
        {
            if (!Password.Equals(oldPassword))
                throw new ArgumentException("The old password is invalid!");
            if(!newPassword.Equals(newPasswordConfirmation))
                throw new ArgumentException("The new password and confirmation doesn't match.");
            Password = newPassword;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Username
        {
            get { return username; }
            internal set { username = value; }
        }

        public string Password
        {
            get { return password; }
            internal set { password = value; }
        }

        public override string ToString()
        {
            return Name + " ( " + Username + " )";
        }
    }
}