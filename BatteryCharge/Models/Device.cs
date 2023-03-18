using System;
using System.Collections.Generic;

namespace BatteryCharge.Models;

public partial class Device
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public string Name { get; set; } = null!;

    public int BatteryReplacmentCount { get; set; }

    public DateTime DateBought { get; set; }

    public int? BatteryCapacity { get; set; }

    public int? BatteryVoltage { get; set; }

    public DateTime? LastRechargeDate { get; set; }

    public int? RechargeCycle { get; set; }

    public virtual BatteryTrackerUser? Owner { get; set; }
}
