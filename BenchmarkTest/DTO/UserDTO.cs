using BenchmarkTest.Models;

namespace BenchmarkTest.DTO
{
    public class UserDTO
    {
        public UserDTO()
        {

        }

        // This method is to convert a data base object into a DTO
        public UserDTO(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Nickname = user.Nickname;
        }

        public long? Id { get; set; }
        public string Name { get; set; }
        public string? Nickname { get; set; }
    }
}
