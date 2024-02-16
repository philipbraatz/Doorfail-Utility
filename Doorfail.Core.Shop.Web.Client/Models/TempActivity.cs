namespace Doorfail.Core.Shop.Web.Client.Models;

public class TempActivity
{
    public string Name { get; set; }
    public string Message { get; set; }
    public string Time { get; set; }
    public string Type { get; set; }
    public DateTime ActivityTime { get; set; }

    public TempActivity() { }

    public TempActivity(string Name, string Message, string Time, string Type, DateTime ActivityTime)
    {
        this.Name = Name;
        this.Message = Message;
        this.Time = Time;
        this.Type = Type;
        this.ActivityTime = ActivityTime;
    }
    public List<TempActivity> GetActivityData()
    {
        List<TempActivity> data = new List<TempActivity>
            {
                new TempActivity("Added New Doctor", "Dr.Johnson James, Cardiologist", "5 mins ago", "doctor", new DateTime(2020, 2, 1, 9, 0, 0)),
                new TempActivity("Added New Appointment", "Laura for General Checkup on 7th March, 2020 @ 8.30 AM with Dr.Molli Cobb, Cardiologist", "5 mins ago", "appointment", new DateTime(2020, 2, 1, 11, 0, 0)),
                new TempActivity("Added New Patient", "James Richard for Fever and cold", "5 mins ago", "patient", new DateTime(2020, 2, 1, 10, 0, 0)),
                new TempActivity("Added New Appointment", "Joseph for consultation on 7th Feb, 2020 @ 11.10 AM with Dr.Molli Cobb", "5 mins ago", "appointment", new DateTime(2020, 2, 1, 11, 0, 0))
            };
        return data;
    }
}