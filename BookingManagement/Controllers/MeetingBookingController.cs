using System;
using System.Collections.Generic;
using System.Data;
using Contract;
using Microsoft.AspNetCore.Mvc;

namespace Api.Http.Controllers
{
    [ApiController]
    [Route("api")]
    public class MeetingBookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public MeetingBookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        [Route("book/date/{date}/start-time/{startTime}/end-time/{endTime}/attendees/{attendees}")]
        public ActionResult Book(DateTime date, string startTime, string endTime, int attendees)
        {
            try
            {
                if (!TimeSpan.TryParse(startTime, out var startTimeSpan) ||
                    !TimeSpan.TryParse(endTime, out var endTimeSpan))
                {
                    return BadRequest(ErrorMessage.BadTimeFormat);
                }

                var result = _bookingService.BookMeeting(date, startTimeSpan, endTimeSpan, attendees);

                return Ok("meeting successfully booked");
            }

            catch (DataException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("view/date/{date}/start-time/{startTime}/end-time/{endTime}")]
        public ActionResult<List<string>> View(DateTime date, string startTime, string endTime)
        {
            try
            {
                if (!TimeSpan.TryParse(startTime, out var startTimeSpan) ||
                    !TimeSpan.TryParse(endTime, out var endTimeSpan))
                {
                    return BadRequest(ErrorMessage.BadTimeFormat);
                }

                var result = _bookingService.ViewAvailableRooms(date, startTimeSpan, endTimeSpan).Result;
                
                if (result.Count == 0)
                {
                    return Ok(ErrorMessage.NoRoomsAvailable);
                }

                return Ok(result);
            }

            catch (DataException)
            {
                return BadRequest(ErrorMessage.NoRoomsAvailable);
            }
        }
    }
}
