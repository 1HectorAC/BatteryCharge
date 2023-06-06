namespace BatteryCharge.Models.ViewModels
{
    public class RechargeDeviceDetails
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime LastRechargeDate { get; set; }

        public int RechargeCycle { get; set; }

        public int DaysSinceLastRecharge { get; set; }


    }
}
