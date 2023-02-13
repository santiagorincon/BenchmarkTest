using BenchmarkTest.DTO;
using BenchmarkTest.Interfaces;
using BenchmarkTest.Models;

namespace BenchmarkTest.Repositories
{
    public class UserRepository : UserInterface
    {
        // Database context is injected using dependency injection
        private readonly BenchmarkTestContext _dbContext;

        public UserRepository(BenchmarkTestContext dbContext)
        {
            _dbContext = dbContext;
        }
        public UserDTO getUserById(long userId)
        {
            var user = _dbContext.Users.Where(u => u.Id == userId)
                .Select(u => new UserDTO(u))
                .FirstOrDefault();

            if (user == null)
            {
                throw new Exception(string.Format("User not found: {0}", userId));
            }

            return user;
        }

        public UserDTO getUserByNickname(string nickname)
        {
            var user = _dbContext.Users.Where(u => u.Nickname == nickname)
                .Select(u => new UserDTO(u))
                .FirstOrDefault();

            if (user == null)
            {
                throw new Exception(string.Format("Nickname not found: {0}", nickname));
            }

            return user;
        }

        public List<UserDTO> getUsers()
        {
            var users = _dbContext.Users
                .Select(u => new UserDTO(u))
                .ToList();

            return users;
        }

        public string getValidNickname(string name)
        {
            string nickname = name.Replace(" ", "");
            var oldUser = getUserByNickname(nickname);
            while(oldUser != null)
            {
                Random rnd = new Random();
                nickname = string.Format("{0}{1}", nickname, rnd.Next(0, 9));
                oldUser = getUserByNickname(nickname);
            }

            return nickname;
        }

        public UserDTO newUser(UserDTO newUser)
        {
            User user = new User();

            user.Nickname = newUser.Nickname;
            user.Name = newUser.Name;

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            UserDTO userResult = getUserById(user.Id);
            return userResult;
        }

        public UserDTO updateUser(UserDTO updatedUser)
        {
            if (updatedUser.Id is null)
            {
                throw new Exception("Id can't be null");
            }

            User user = _dbContext.Users.Find(updatedUser.Id);
            if(user is null)
            {
                throw new Exception(string.Format("User not found: {0}", updatedUser.Id));
            }

            user.Name = updatedUser.Name;
            user.Nickname = updatedUser.Nickname;

            _dbContext.SaveChanges();
            UserDTO userResult = new UserDTO(user);
            return userResult;
        }
    }
}
