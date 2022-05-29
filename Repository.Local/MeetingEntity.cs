using Microsoft.EntityFrameworkCore;
using System;

namespace Repository.Local
{
    public class MeetingEntity
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class MeetingDBContext : DbContext
    {
        public MeetingDBContext(DbContextOptions<MeetingDBContext> options)
            : base(options)
        { }
        public DbSet<MeetingEntity> Meeting { get; set; }
    }
}
