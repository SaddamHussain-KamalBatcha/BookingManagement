using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Local.RepositoryContract
{
    public interface IBookingRepository
    {
        Task<bool> BookMeeting(DateTime date, TimeSpan startTime, TimeSpan endTime, int attendees);

        Task<List<string>> ViewAvailableMeetingRooms(DateTime date, TimeSpan startTime, TimeSpan endTime);

    }
}
