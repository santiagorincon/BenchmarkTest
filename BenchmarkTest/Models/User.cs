using System;
using System.Collections.Generic;

namespace BenchmarkTest.Models;

public partial class User
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Nickname { get; set; }

    public virtual ICollection<GameScore> GameScores { get; } = new List<GameScore>();

    public virtual ICollection<GameUser> GameUsers { get; } = new List<GameUser>();

    public virtual ICollection<Game> Games { get; } = new List<Game>();

    public virtual ICollection<Roll> Rolls { get; } = new List<Roll>();
}
