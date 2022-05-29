using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contract
{
    public interface IBookingService
    {
        Task BookMeeting(DateTime date, TimeSpan startTime, TimeSpan endTime, int attendees);

        Task<List<string>> ViewAvailableRooms(DateTime date, TimeSpan startTime, TimeSpan endTime);
    }
}
