using BenchmarkTest.DTO;

namespace BenchmarkTest.Interfaces
{
    public interface UserInterface
    {
        UserDTO getUserById(long userId);
        UserDTO getUserByNickname(string nickname);
        List<UserDTO> getUsers();
        UserDTO newUser(UserDTO newUser);
        UserDTO updateUser(UserDTO updatedUser);
        string getValidNickname(string name);
    }
}
