using System;
using System.Collections.Generic;

namespace BenchmarkTest.Models;

public partial class Roll
{
    public long Id { get; set; }

    public long GameId { get; set; }

    public int Frame { get; set; }

    public int Counter { get; set; }

    public int PinsKnocked { get; set; }

    public bool IsStrike { get; set; }

    public bool IsSpare { get; set; }

    public long UserId { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
