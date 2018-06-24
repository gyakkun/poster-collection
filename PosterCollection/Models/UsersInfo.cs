namespace PosterCollection.Models
{
    class UsersInfo
    {
        private int id;
        private string username;
        private string password;
        private int role;
        private string email;
        private string phone;

        public UsersInfo(int id = 0, string name = "", string pass = "", string e = "", string p = "", int r = 1)
        {
            this.id = id;
            this.username = name;
            this.password = pass;
            this.role = r;
            this.email = e;
            this.phone = p;
        }

        public int Id
        {
            get { return id; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public int Role
        {
            get { return role; }
            set { role = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }
    }
}
