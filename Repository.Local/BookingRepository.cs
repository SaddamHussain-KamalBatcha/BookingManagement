using Repository.Local.RepositoryContract;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Repository.Local.Models;
using System.Collections.Generic;

namespace Repository.Local
{
    public class BookingRepository : IBookingRepository
    {
        private readonly MeetingDBContext _dbContext;
        private readonly IConfiguration _configuration;

        private readonly string AcropolisCapacity;
        private readonly string TajMahalCapacity;
        private readonly string SagradaFamiliaCapacity;


        public BookingRepository(MeetingDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;

            AcropolisCapacity = _configuration.GetValue<string>("Settings:Acropolis");
            TajMahalCapacity = _configuration.GetValue<string>("Settings:TajMahal");
            SagradaFamiliaCapacity = _configuration.GetValue<string>("Settings:SagradaFamilia");
        }

        public Task<bool> BookMeeting(DateTime date, TimeSpan startTime, TimeSpan endTime, int attendees)
        {
            var optimalMeetingRoom = FindOptimalMeetingRoom(attendees);

            var result = _dbContext.Meeting.Any(x => x.Date == date &&
                                                     x.StartTime <= startTime || x.EndTime >= startTime
                                                     && x.StartTime <= endTime || x.EndTime >= endTime && x.RoomName == optimalMeetingRoom);

            var isRoomBooked =
                 _dbContext.Meeting.Any(x => x.Date == date && x.StartTime == startTime
                                                           && x.EndTime == endTime
                                                           && x.RoomName == optimalMeetingRoom);
            if (!isRoomBooked)
            {
                var isOverLapping = IsOverLapping(date, startTime, endTime, optimalMeetingRoom);
                if (!isOverLapping)
                    return BookMeeting(date, startTime, endTime, optimalMeetingRoom);
            }

            if (attendees <= Convert.ToInt16(SagradaFamiliaCapacity))
            {
                var isSagardaAvailable =
                    Any(date, startTime, endTime, Room.Sagarda);
                if (!isSagardaAvailable)
                {
                    var isOverLapping = IsOverLapping(date, startTime, endTime, Room.Sagarda);

                    if (!isOverLapping)
                        return BookMeeting(date, startTime, endTime, Room.Sagarda);
                }
            }
            if (attendees <= Convert.ToInt16(TajMahalCapacity))
            {
                var isTajMahalAvailable =
                    Any(date, startTime, endTime, Room.TajMahal);
                if (!isTajMahalAvailable)
                {
                    var isOverLapping = IsOverLapping(date, startTime, endTime, Room.TajMahal);

                    if (!isOverLapping)
                        return BookMeeting(date, startTime, endTime, Room.TajMahal);
                }
            }

            var isAcropolisAvailable =
                Any(date, startTime, endTime, Room.Acropolis);
            if (!isAcropolisAvailable)
            {
                var isOverLapping = IsOverLapping(date, startTime, endTime, Room.Acropolis);

                if (!isOverLapping)
                    return BookMeeting(date, startTime, endTime, Room.Acropolis);
            }

            return Task.FromResult(false);
        }

        public Task<List<string>> ViewAvailableMeetingRooms(DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            List<string> availableRooms = new List<string>();
            var isSagardaAvailable = Any(date, startTime, endTime, Room.Sagarda);
            var isTajMahalAvailable = Any(date, startTime, endTime, Room.TajMahal);
            var isAcropolisAvailable = Any(date, startTime, endTime, Room.Acropolis);

            if (!isSagardaAvailable)
            {
                var isOverLapping = IsOverLapping(date, startTime, endTime, Room.Sagarda);
                if (!isOverLapping)
                    availableRooms.Add(Room.Sagarda);
            }
            if (!isTajMahalAvailable)
            {
                var isOverLapping = IsOverLapping(date, startTime, endTime, Room.TajMahal);
                if (!isOverLapping)
                    availableRooms.Add(Room.TajMahal);
            }
            if (!isAcropolisAvailable)
            {
                var isOverLapping = IsOverLapping(date, startTime, endTime, Room.Acropolis);
                if (!isOverLapping)
                    availableRooms.Add(Room.Acropolis);
            }

            return Task.FromResult(availableRooms);
        }

        private bool IsOverLapping(DateTime date, TimeSpan startTime, TimeSpan endTime, string roomName)
        {
            var isOverLapping = _dbContext.Meeting.Any(x => x.Date == date
                                                            && (x.StartTime <= startTime && x.EndTime >= startTime)
                                                            && (x.StartTime <= endTime && x.EndTime >= endTime)
                                                            && x.RoomName == roomName);
            return isOverLapping;
        }

        private Task<bool> BookMeeting(DateTime date, TimeSpan startTime, TimeSpan endTime, string optimalMeetingRoom)
        {
            var meetingEntity = new MeetingEntity()
            {
                RoomName = optimalMeetingRoom,
                Date = date,
                StartTime = startTime,
                EndTime = endTime
            };

            _dbContext.Meeting.Add(meetingEntity);
            _dbContext.SaveChanges();

            return Task.FromResult(true);
        }

        private bool Any(DateTime date, TimeSpan startTime, TimeSpan endTime, string roomName)
        {
            return _dbContext.Meeting.Any(x => x.Date == date && x.StartTime == startTime
                                                              && x.EndTime == endTime
                                                              && x.RoomName == roomName);
        }

        private string FindOptimalMeetingRoom(int attendees)
        {
            string optimalMeetingRoom;

            if (attendees <= Convert.ToInt16(SagradaFamiliaCapacity))
            {
                optimalMeetingRoom = Room.Sagarda;
            }
            else if (attendees > Convert.ToInt16(SagradaFamiliaCapacity) && attendees <= Convert.ToInt16(TajMahalCapacity))
            {
                optimalMeetingRoom = Room.TajMahal;
            }
            else
            {
                optimalMeetingRoom = Room.Acropolis;
            }

            return optimalMeetingRoom;
        }
    }
}
