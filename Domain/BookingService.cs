using Contract;
using Microsoft.Extensions.Configuration;
using Repository.Local.RepositoryContract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Domain
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IConfiguration _configuration;

        public string AcropolisCapacity;
        public string TajMahalCapacity;
        public string SagradaFamiliaCapacity;

        public BookingService(IBookingRepository bookingRepository, IConfiguration configuration)
        {
            _bookingRepository = bookingRepository;
            _configuration = configuration;

            AcropolisCapacity = _configuration.GetSection("Settings.Acropolis").Value;
            TajMahalCapacity = _configuration.GetSection("Settings.TajMahal").Value;
            SagradaFamiliaCapacity = _configuration.GetSection("Settings.SagradaFamilia").Value;
        }

        public Task BookMeeting(DateTime date, TimeSpan startTime, TimeSpan endTime, int attendees)
        {
            if (attendees < 2 || attendees > 20)
            {
                throw new DataException(ErrorMessage.MeetingRoomLimit);
            }


            HandleValidationRules(date, startTime, endTime);
            var result = _bookingRepository.BookMeeting(date, startTime, endTime, attendees);

            if (result.Result == false)
            {
                throw new DataException(ErrorMessage.NoMeetingRoomAvailable);
            }

            return result;
        }

        private static void HandleValidationRules(DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var bufferTime = new List<string>()
            {
                "09:00", "09:05", "09:10", "09:15", "13:15", "13:20", "13:25", "13:30", "13:35", "13:40", "13:45",
                "18:45", "18:50", "18:55", "19:00"
            };

            var stime = $@"{startTime:hh\:mm}";
            var etime = $@"{endTime:hh\:mm}";

            if (date < DateTime.Today)
            {
                throw new DataException(ErrorMessage.PastDate);
            }

            if (startTime > new TimeSpan(23, 45, 00)
                || endTime > new TimeSpan(23, 45, 00)
                || startTime.Minutes % 5 != 0
                || endTime.Minutes % 5 != 0)

            {
                throw new DataException(ErrorMessage.NotAvailableAtThisTime);
            }

            if (bufferTime.Contains(stime)
                || bufferTime.Contains(etime))
            {
                throw new DataException(ErrorMessage.MaintenanceHour);
            }

            if (endTime <= startTime)
            {
                if (stime != "12:00" || etime != "01:00")
                {
                    throw new DataException(ErrorMessage.NotAvailableAtThisTime);
                }
            }
        }

        public Task<List<string>> ViewAvailableRooms(DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            HandleValidationRules(date, startTime, endTime);

            return _bookingRepository.ViewAvailableMeetingRooms(date, startTime, endTime);
        }
    }
}
