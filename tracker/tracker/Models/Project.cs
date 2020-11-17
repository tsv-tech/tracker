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
    /* Задает "формат" данных (data), который будет использоваться в приложении, в данном случае единицей данных будет "Проект" 
     с полями, которые хранят конкретную информацию */

    [Table("Projects")]
    public class Project : ObservableObject
    {
        //Конструкторы "проекта" для различных ситуаций 
        //  (первое создание объекта / передача на страницу для изменений / инициализаия списка при запуске приложения / отправка на сервер ...)

        public Project(Project p)
        {
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
        }

        /* команды представляют функцию, к которой можно присоединить button*/

        #region PROPERTIES

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        /* Каждое поле имеет 2 переменные (variable): "name" хранит значение, "Name" реализует чтение/запись этого значения 
         при этом дополнительно "оповещает" о том, что значение изменилось*/

        string name;
        public string Name
        {
            get { return name; }
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

        TimeSpan time = new TimeSpan(0, 0, 0);
        public TimeSpan Time
        {
            get { return time; }
            set
            {
                if (value == time)
                    return;
                time = value;
                OnPropertyChanged(nameof(GetTime));
            }
        }

        public DateTime SessionStartTime { get; set; }

        TimeSpan sessionTime = new TimeSpan(0, 0, 0);
        public TimeSpan SessionTime
        {
            get { return sessionTime; }
            set
            {
                if (value == sessionTime)
                    return;
                sessionTime = value;
                OnPropertyChanged(nameof(GetTime));
            }
        }

        /* Специальная функция для передачи на View времени в удобном для пользователя формате */
        [Ignore]
        public string GetTime
        {
            //get Time + SessionTime while active and just Time if not
            get
            {
                if (isRunning)
                    return string.Format("{0}:{1:mm}:{1:ss}",
                         (int)(Time + SessionTime).TotalHours,
                         Time + SessionTime);
                else
                    return string.Format("{0}:{1:mm}:{1:ss}",
                     (int)Time.TotalHours,
                     Time);
            }
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

        /* Специальная функция для передачи на View даты в удобном для пользователя формате */
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

        /* Специальная функция для передачи на View цены в удобном для пользователя формате */
        [Ignore]
        public string GetTotalPrice
        {
            get
            {
                double total = Time.TotalHours * payment;
                return @"Total: " + total.ToString("F") + " " + Currency;
            }
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                SetProperty(ref isBusy, value);
            }
        }
        /*
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
        */
        #endregion END_PROPERTIES

        #region TIMERS & SESSIONS

        bool isRunning = false;
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                SetProperty(ref isRunning, value);
                OnPropertyChanged(nameof(GetTime));
                OnPropertyChanged(nameof(GetState));
                OnPropertyChanged(nameof(GetColor));
            }
        }


        [Ignore]
        public string GetState { get { return !isRunning ? "Start" : "Stop"; } }
        [Ignore]
        public string GetColor { get { return !isRunning ? "White" : "LightGreen"; } }

        #endregion TIMERS
    }
}
