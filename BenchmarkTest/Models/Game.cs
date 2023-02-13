using System;
using System.Collections.Generic;

namespace BenchmarkTest.Models;

public partial class Game
{
    public long Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? PartialScore { get; set; }

    public int? FinalScore { get; set; }

    public long? WinnerUserId { get; set; }

    public virtual ICollection<GameScore> GameScores { get; } = new List<GameScore>();

    public virtual ICollection<GameUser> GameUsers { get; } = new List<GameUser>();

    public virtual ICollection<Roll> Rolls { get; } = new List<Roll>();

    public virtual User? WinnerUser { get; set; }
}
