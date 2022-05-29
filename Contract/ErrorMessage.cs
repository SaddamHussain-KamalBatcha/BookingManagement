using System;
using System.Collections.Generic;
using System.Text;

namespace Contract
{
    public static  class ErrorMessage
    {
        public static string MeetingRoomLimit = "Bookings can be only made for 2 or up to a maximum of 20 people";

        public static string NoMeetingRoomAvailable = "No meeting rooms available at given time";

        public static string NotAvailableAtThisTime = "Meeting cannot be booked at given time";

        public static string MaintenanceHour = "Maintenance hours - Meeting cannot be booked at this time";

        public static string BadTimeFormat = "Bad time format";

        public static string SuceesMessage = "meeting successfully booked";

        public static string NoRoomsAvailable = "No Rooms available at given time";

        public static string PastDate = "Cannot book for past date-time";
    }
}
