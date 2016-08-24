using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Calendar
{
    //public class Matrix
    //{
    //    private readonly char[,] _matrix;

    //    public Matrix(char[,] matrix)
    //    {
    //        _matrix = matrix;
    //    }

    //    public char GetItem(int i, int j)
    //    {
    //        return _matrix[i, j];
    //    }
    //}

    //public class Week
    //{
    //    public Week(int length)
    //    {
    //        Length = length;
    //    }

    //    public int Length { get; private set; }     
    //}

    //public interface IDay
    //{
    //}

    //public class NotDay : IDay
    //{
    //    public NotDay(int numberInWeek)
    //    {
    //        NumberInWeek = numberInWeek;
    //    }

    //    public int NumberInWeek { get; private set; }
    //}

    //public class Day : IDay
    //{
    //    public Day(int numberInMonth, int numberInWeek)
    //    {
    //        NumberInMonth = numberInMonth;
    //        NumberInWeek = numberInWeek;
    //    }

    //    public int NumberInMonth { get; private set; }
    //    public int NumberInWeek { get; private set; }
    //}

    //class CalendarRenderer
    //{
    //    public string Render(int year, int month)
    //    {
    //        var dateTime = new DateTime(year, month, 1);
    //        var firstDay = dateTime.DayOfWeek;

    //        Fill(year, month, new Week(7));
    //    }

    //    private void Fill(int year, int month, Week week)
    //    {
    //        var dateTime = new DateTime(year, month, 1);
    //        var firstDay = (int)dateTime.DayOfWeek;
    //        var daysInMonth = DateTime.DaysInMonth(year, month);
    //        var weekStart = week.Length - firstDay;

    //        var weeksAmount = (daysInMonth - weekStart)/week.Length;

    //        new Day[week.Length].Select((x, i) => GetValue(firstDay + i, i));
    //    }

    //    private IDay GetValue(int firstDay, int i)
    //    {
    //        if (firstDay < i)
    //            return new NotDay(i);

    //        return new Day(firstDay + i, i);
    //    }
    //}
}
