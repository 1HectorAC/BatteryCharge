using System;
using System.Collections.Generic;

namespace BatteryCharge.Models;

public partial class BatteryTrackerUser
{
    public int Id { get; set; }

    public string AspNetUserId { get; set; } = null!;

    public virtual ICollection<Device> Devices { get; } = new List<Device>();
}
