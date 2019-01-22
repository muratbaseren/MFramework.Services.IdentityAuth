using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MFramework.Services.IdentityAuth.Constants
{
    public static class ConstantHelper
    {
        public static Dictionary<string, string> GetDisplayNamesDictionary<TClass>()
        {
            return typeof(TClass).GetFields()
                .Select(x =>
                {
                    DisplayAttribute displayAttr =
                        x.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;

                    return new Tuple<string, string>(x.GetValue(x).ToString(), displayAttr.Name);
                }).ToDictionary(x => x.Item1, y => y.Item2);
        }
    }
}