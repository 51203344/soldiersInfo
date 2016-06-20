using SoldiersInfo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoldiersInfo.Controllers
{
    public class Searching
    {
        static public IQueryable<Soldier> Search(IQueryable<Soldier> soldiers, string searchString, string search_option)
        {
            searchString = searchString.Trim(); // xử lý từ cần tìm
            switch (search_option)
            {
                case "name":
                    String[] string_filter = filter_space_for_name(searchString); // xử lý tên
                    soldiers = search_by_name(soldiers, string_filter); // tìm theo tên
                    break;
                case "servingDate":
                    int[] time_0 = filter_slash_for_date(searchString); // xử lý ngày
                    soldiers = search_by_day(soldiers, 0, time_0); // tìm theo ngày
                    break;
                case "pointDate":
                    int[] time_1 = filter_slash_for_date(searchString); // xử lý ngày
                    soldiers = search_by_day(soldiers, 1, time_1); // tìm theo ngày
                    break;
                case "company":
                    soldiers = search_by_company(soldiers, searchString); // tìm theo phòng
                    break;
                default:
                case "unknow":
                    IQueryable<Soldier> soldiers_in_company = search_by_company(soldiers, searchString);
                    IQueryable<Soldier> soldiers_in_name = search_by_name(soldiers.Except(soldiers_in_company), filter_space_for_name(searchString));
                    IQueryable<Soldier> soldiers_in_servingDate = null;
                    IQueryable<Soldier> soldiers_in_pointDate = null;
                    soldiers_in_servingDate = search_by_day(soldiers, 0, filter_slash_for_date(searchString));
                    soldiers_in_pointDate = search_by_day(soldiers, 1, filter_slash_for_date(searchString));
                    IQueryable<Soldier> result = null;
                    result = union_IQeryable(result, soldiers_in_name); // thêm vào kết quả từ tìm kiếm tên
                    result = union_IQeryable(result, soldiers_in_company); // thêm vào kết quả từ tìm kiếm nhóm
                    result = union_IQeryable(result, soldiers_in_pointDate); // thêm vào kết quả từ tìm kiếm ngày bắt đầu tính điểm
                    result = union_IQeryable(result, soldiers_in_servingDate); // thêm vào kết quả từ tìm kiếm ngày bắt đầu phục vụ
                    soldiers = result;
                    break;
            }

            return soldiers;
        }
        static private String[] filter_space_for_name(String searchString)
        {
            int first_space_index = searchString.IndexOf(" ");
            int last_space_index = searchString.LastIndexOf(" ");
            String searchstring_firstname;
            String searchstring_lastname;
            String lastname_and_middlename_string;
            String searchstring_middlename;
            if (first_space_index <= last_space_index && first_space_index >= 0) // khi có ít nhất họ và tên
            {
                searchstring_firstname = searchString.Substring(last_space_index + 1); // tên bắt đầu từ " " cuối cùng đến hết                
                searchstring_lastname = searchString.Substring(0, first_space_index); // họ bắt đầu từ đầu đến " " đầu tiên                
                if (first_space_index < last_space_index) // có tên lót
                {
                    lastname_and_middlename_string = searchString.Substring(0, last_space_index); // lấy ra họ và tên lót
                    searchstring_middlename = lastname_and_middlename_string.Substring(first_space_index + 1); // lấy ra tên lót
                }
                else
                    searchstring_middlename = null;
            }
            else
            {
                searchstring_firstname = searchString;
                searchstring_middlename = null;
                searchstring_lastname = null;
            }
            String[] result = { searchstring_lastname, searchstring_middlename, searchstring_firstname };
            return result;
        }
        static private IQueryable<Soldier> search_by_name(IQueryable<Soldier> soldiers, string[] string_to_search)
        {
            String lastname = string_to_search[0];
            String middleName = string_to_search[1];
            String firstName = string_to_search[2];
            if (!String.IsNullOrEmpty(string_to_search[0]))
                soldiers = soldiers.Where(s => s.lastName.Contains(lastname));
            if (!String.IsNullOrEmpty(string_to_search[1]))
                soldiers = soldiers.Where(s => s.middleName.Contains(middleName));
            if (!String.IsNullOrEmpty(string_to_search[2]))
                soldiers = soldiers.Where(s => s.firstName.Contains(firstName));
            return soldiers;
        }
        static private int[] filter_slash_for_date(String searchString)
        {
            int count_of_slash = searchString.Count(s => s == '/');
            int slash_first_position = searchString.IndexOf("/");
            int slash_last_position = searchString.LastIndexOf("/");
            String first;
            String midde;
            String first_middle;
            String last;
            int year, month, day;
            bool try_year, try_month, try_day;

            if (count_of_slash < 3) // date only have 2 slashes
            {
                if (slash_first_position <= slash_last_position && slash_first_position > 0) // if it has "/"
                {
                    first = searchString.Substring(0, slash_first_position); // letters before first "/"
                    last = searchString.Substring(slash_last_position + 1); // letters after last "/"
                    if (slash_first_position < slash_last_position) // if it has 2 "/"s
                    {
                        first_middle = searchString.Substring(0, slash_last_position); // take all letters before last "/"
                        midde = first_middle.Substring(slash_first_position + 1); // take all letters between 2 "/"
                    }
                    else
                        midde = "";
                }
                else // if there is no "/"
                {
                    try_year = int.TryParse(searchString, out year); // check searchstring if it is a year
                    first = "";
                    midde = "";
                    if (try_year)
                        last = year.ToString();
                    else
                        last = "";
                }
            }
            else // searchstring is not valid
            {
                first = "";
                midde = "";
                last = "";
            }

            try_year = int.TryParse(last, out year); // take year
            if (slash_first_position < slash_last_position && slash_first_position > 0) // if there is more than 1"/"
            {
                try_day = int.TryParse(first, out day); // take day
                try_month = int.TryParse(midde, out month); // take month
            }
            else
            {
                try_month = int.TryParse(first, out month); // take month
                day = 0;
                try_day = false;
            }
            int[] result = { day, month, year };
            return result;
        }
        static private IQueryable<Soldier> search_by_day(IQueryable<Soldier> soldiers, int type_of_day, int[] time)
        {
            // 0 -> servingdate
            // 1 -> pointdate
            int day = time[0];
            int month = time[1];
            int year = time[2];
            switch (type_of_day)
            {
                case 0:
                    soldiers = soldiers.Where(s => s.servingDate.Year == year);
                    if (month > 0)
                    {
                        soldiers = soldiers.Where(s => s.servingDate.Month == month);
                        if (day > 0)
                            soldiers = soldiers.Where(s => s.servingDate.Day == day);
                    }
                    break;
                default:
                    soldiers = soldiers.Where(s => s.pointDate.Year == year);
                    if (month > 0)
                    {
                        soldiers = soldiers.Where(s => s.pointDate.Month == month);
                        if (day > 0)
                            soldiers = soldiers.Where(s => s.pointDate.Day == day);
                    }
                    break;
            }
            return soldiers;
        }
        static private IQueryable<Soldier> search_by_company(IQueryable<Soldier> soldiers, string searchString)
        {
            soldiers = soldiers.Where(s => s.company.Contains(searchString));
            return soldiers;
        }
        static private IQueryable<Soldier> union_IQeryable(IQueryable<Soldier> start, IQueryable<Soldier> bemerged) // start can be null, but bemerged can't
        {
            if (bemerged.Count() != 0)
                if (start != null)
                    start.Union(bemerged);
                else
                    start = bemerged;
            return start;
        }
        
    }
}