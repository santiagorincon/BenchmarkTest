using System;
using System.Collections.Generic;

namespace BenchmarkTest.Models;

public partial class GameScore
{
    public long Id { get; set; }

    public long GameId { get; set; }

    public long UserId { get; set; }

    public int Frame { get; set; }

    public int Score { get; set; }

    public bool IsClosed { get; set; }

    public int PendingRolls { get; set; }

    public bool IsStrike { get; set; }

    public bool IsSpare { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
