using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Windows.Input;
using MvvmHelpers;
using SQLite;
using Xamarin.Forms;

namespace tracker.Models
{
    [Table("Projects")]
    public class Project : ObservableObject
    {
        
        public Project(string Name, string Author, string CustomId, string Payment, string Comment)
        {
            StartCommand = new Command(StartTimer);
            StopCommand = new Command(StopTimer);

            this.Name = Name;
            this.Author = Author;
            this.CustomId = CustomId;
            this.Payment = Double.Parse(Payment);
            this.Comment = Comment;
            DateCreated = DateTime.Now;
        }

        public Project(Project p)
        {
            StartCommand = new Command(StartTimer);
            StopCommand = new Command(StopTimer);

            this.Id = p.Id;
            this.Name = p.Name;
            this.Author = p.Author;
            this.CustomId = p.CustomId;
            this.Payment = p.Payment;
            this.Comment = p.Comment;
            this.Time = p.Time;
            this.DateCreated = p.DateCreated;
            this.Currency = p.Currency;
        }
        public Project()
        {
            StartCommand = new Command(StartTimer);
            StopCommand = new Command(StopTimer);
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        #region PROPERTIES

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        string name;
        public string Name
        {
            get {return name;}
            set
            {
                SetProperty(ref name, value);
            }
        }

        string author;
        public string Author
        {
            get { return author; }
            set
            {
                SetProperty(ref author, value);
            }
        }

        TimeSpan time = new TimeSpan(0,0,0);
        public TimeSpan Time
        {
            get { return time; }
            set
            {
                //SetProperty(ref time, value);
                if (value == time)
                    return;
                time = value;
                OnPropertyChanged(nameof(GetTime));
            }
        }

        [Ignore]
        public string GetTime
        {
            get { return "Timer is " + (IsRunning? " ON ":"OFF ") +  Time.ToString(@"hh\:mm\:ss"); }
        }

        string customId;
        [Unique]
        public string CustomId
        {
            get { return customId; }
            set
            {
                SetProperty(ref customId, value);
            }
        }

        string comment;
        public string Comment
        {
            get { return comment; }
            set
            {
                SetProperty(ref comment, value);
            }
        }

        DateTime dateCreated = DateTime.Now;
        public DateTime DateCreated
        {
            get { return dateCreated; }
            set
            {
                //SetProperty(ref dateCreated, value);
                if (value == dateCreated)
                    return;
                dateCreated = value;
                OnPropertyChanged(nameof(GetDate));
            }
        }
        [Ignore]
        public string GetDate
        {
            get { return DateCreated.ToString("dd.MM.yyyy"); }
        }

        double payment;
        public double Payment
        {
            get { return payment; }
            set
            {
                SetProperty(ref payment, value);
                OnPropertyChanged(nameof(GetTotalPrice));
            }
        }

        string currency;
        public string Currency
        {
            get { return currency; }
            set
            {
                SetProperty(ref currency, value);
                OnPropertyChanged(nameof(GetTotalPrice));
            }
        }

        [Ignore]
        public string GetTotalPrice
        {
            get { double total = Time.TotalHours * payment; 
                return @"Total: " + total + " " + Currency; }
        }
        [Ignore]
        public ObservableRangeCollection<Session> Sessions
        {
            get
            {
                var sessions = new ObservableRangeCollection<Session>();
                IEnumerable<Session> allSessions = App.DBSessions.GetItems();
                var range = allSessions.Where(s => s.ProjectId == this.Id);
                sessions.AddRange(range);
                return sessions;
            }
        }

        #endregion END_PROPERTIES

        #region TIMERS & SESSIONS

        private DateTime SessionDateStarted;
        bool isRunning = false;
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                SetProperty(ref isRunning, value);
                OnPropertyChanged(nameof(GetTime));
            }
        }
        public void StartTimer()
        {
            if (IsRunning)
                return;
            SessionDateStarted = DateTime.Now;
            IsRunning = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Time += new TimeSpan(0, 0, 1);
                //Time = Time.Add(new TimeSpan(0,0,1));
                return IsRunning;
            });
        }

        public void StopTimer()
        {
            if (!IsRunning)
                return;

            IsRunning = false;
            App.DBSessions.SaveItem(new Session { ProjectId = this.Id,
                Date = SessionDateStarted.ToString(), 
                Duration = (SessionDateStarted - DateTime.Now).ToString(@"hh\:mm\:ss")
            });

            App.DBProjects.SaveItem(this);
            //Sessions.Add(DateTime.Now.ToString() + "     Lasted: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss")); 
        }

        #endregion TIMERS
    }
}
