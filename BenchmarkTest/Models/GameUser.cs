using System;
using System.Collections.Generic;

namespace BenchmarkTest.Models;

public partial class GameUser
{
    public long Id { get; set; }

    public long GameId { get; set; }

    public long UserId { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
