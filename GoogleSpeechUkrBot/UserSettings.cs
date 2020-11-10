using Microsoft.EntityFrameworkCore;

namespace GoogleSpeechUkrBot
{
    class UserSettings : DbContext
    {
        private string connectionString;

        public DbSet<User> Users { get; set; }

        public UserSettings(string connectionString) : base()
        {
            this.connectionString = connectionString;

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(connectionString);

        public User GetUser(long id)
        {
            var user = Users.Find(id);

            if (user is null)
            {
                user = new User 
                { 
                    ID = id,
                    Voice = true,
                    Text = true
                };

                Users.Add(user);

                SaveChanges();

                user = GetUser(id);
            }

            return user;
        }

        public bool GetVoice(long id) => GetUser(id).Voice;

        public bool GetText(long id) => GetUser(id).Text;

        public void ManageVoice(long id, bool value)
        {
            var user = GetUser(id);

            user.Voice = value;

            SaveChanges();
        }

        public void ManageText(long id, bool value)
        {
            var user = GetUser(id);

            user.Text = value;

            SaveChanges();
        }
    }
}