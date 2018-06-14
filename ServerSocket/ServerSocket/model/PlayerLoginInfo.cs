using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSocket
{
    class PlayerLoginInfo
    {
        internal int year { get; set; }
        internal int month { get; set; }
        internal int day { get; set; }
        internal int hour { get; set; }
        internal int minute { get; set; }
        internal int second { get; set; }
        internal int miliSecond { get; set; }


        internal PlayerLoginInfo(int hour, int minute, int second, int miliSecond, int day, int month, int year)
        {
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.miliSecond = miliSecond;
            this.day = day;
            this.month = month;
            this.year = year;
        }

        internal bool Compare(PlayerLoginInfo otherPlayerLoginInfo)
        {
            bool higherPriority = true;
            if (year == otherPlayerLoginInfo.year)
            {
                if (month == otherPlayerLoginInfo.month)
                {
                    if (day == otherPlayerLoginInfo.day)
                    {
                        if (hour == otherPlayerLoginInfo.hour)
                        {
                            if (minute == otherPlayerLoginInfo.minute)
                            {
                                if (second == otherPlayerLoginInfo.second)
                                {
                                    if (miliSecond > otherPlayerLoginInfo.miliSecond) higherPriority = false;
                                }
                                else if (second > otherPlayerLoginInfo.second) higherPriority = false;
                            }
                        }
                        else if (hour > otherPlayerLoginInfo.hour) higherPriority = false;
                    }
                    else if (day > otherPlayerLoginInfo.day) higherPriority = false;
                }
                else if (month > otherPlayerLoginInfo.month) higherPriority = false;
            }
            else if (year > otherPlayerLoginInfo.year) higherPriority = false;
            return higherPriority;
        }

    }
}
