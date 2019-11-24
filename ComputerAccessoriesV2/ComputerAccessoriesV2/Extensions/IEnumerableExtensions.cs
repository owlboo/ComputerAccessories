using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, string propertyName, int selectedValue)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue(propertyName),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };
        }
        public static IEnumerable<SelectListItem> ToSelectListItemString<T>(this IEnumerable<T> items, string propertyName, string selectedValue)
        {
            if (selectedValue == null)
            {
                selectedValue = "";
            }
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue(propertyName),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue)
                   };
        }

        public static IEnumerable<SelectListItem> ToSelectListItemProvince<T>(this IEnumerable<T> items, string propertyName, int selectedValue)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue(propertyName),
                       Value = item.GetPropertyValue("ProvinceId"),
                       Selected = item.GetPropertyValue("ProvinceId").Equals(selectedValue)
                   };
        }

        //public static IEnumerable<SelectListItem> ToSelectListItemStringUser<T>(this IEnumerable<T> items, string selectedValue)
        //{
        //    if (selectedValue == null)
        //    {
        //        selectedValue = "";
        //    }
        //    return from item in items
        //           select new SelectListItem
        //           {
        //               Text = item.GetPropertyValue("Name"),
        //               Value = item.GetPropertyValue("Id"),
        //               Selected = item.GetPropertyValue("Id").Equals(selectedValue)
        //           };
        //}
        public static IEnumerable<SelectListItem> GetEnumSelectList<T>()
        {
            return (Enum.GetValues(typeof(T)).Cast<T>().Select(
                enu => new SelectListItem()
                {
                    Text = enu.ToString(),
                    Value = enu.ToString()
                })).ToList();
        }
    }
}
