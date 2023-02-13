using BenchmarkTest.DTO;

namespace BenchmarkTest.Interfaces
{
    public interface RollInterface
    {
        RollDTO newRoll(RollRequestDTO rollRequest);
    }
}
