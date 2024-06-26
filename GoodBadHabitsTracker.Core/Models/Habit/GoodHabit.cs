﻿using GoodBadHabitsTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GoodBadHabitsTracker.Core.Models.Habit
{
    public class GoodHabit : Habit
    {
        public GoodHabit()
        {
            IsGood = true;
        }
        public User User { get; init; } = default!;
        public Guid UserId { get; set; }
        public bool IsTimeBased { get; init; }
        public int Quantity { get; set; }
        public Frequencies Frequency { get; set; }
        public RepeatModes RepeatMode { get; set; }
        public List<string> RepeatDaysOfWeek { get; init; } = new();
        public List<int> RepeatDaysOfMonth { get; init; } = new();
        public int RepeatInterval { get; set; }
        public List<TimeOnly> ReminderTimes { get; init; } = new();
        public Group? Group { get; set; }
        public Guid? GroupId { get; set; }
    }
}
