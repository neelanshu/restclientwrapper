﻿using System.Web;
using System.Web.Mvc;

namespace RestaurantsNearMe.MVC.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}