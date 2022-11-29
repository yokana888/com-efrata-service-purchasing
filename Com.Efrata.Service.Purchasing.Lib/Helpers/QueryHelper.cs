using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Helpers
{
    public static class QueryHelper<TModel>
        //where TModel : IStandardEntity
    {
        public static IQueryable<TModel> ConfigureSearch(IQueryable<TModel> Query, List<string> SearchAttributes, string Keyword, bool ToLowerCase = false, string SearchWith = "Contains", bool WithAny = true)
        {
            /* Search with Keyword */
            if (Keyword != null)
            {
                string SearchQuery = String.Empty;
                foreach (string Attribute in SearchAttributes)
                {
                    if (WithAny && Attribute.Contains("."))
                    {
                        var Key = Attribute.Split(".");
                        SearchQuery = string.Concat(SearchQuery, Key[0], $".Any({Key[1]}.", SearchWith,"(@0)) OR ");
                    }
                    else
                    {
                        SearchQuery = string.Concat(SearchQuery, Attribute, ".", SearchWith, "(@0) OR ");
                    }
                }

                SearchQuery = SearchQuery.Remove(SearchQuery.Length - 4);

                if (ToLowerCase)
                {
                    SearchQuery = SearchQuery.Replace("." + SearchWith + "(@0)", ".ToLower()." + SearchWith + "(@0)");
                    Keyword = Keyword.ToLower();
                }

                Query = Query.Where(SearchQuery, Keyword);
            }
            return Query;
        }

        public static IQueryable<TModel> ConfigureFilter(IQueryable<TModel> Query, Dictionary<string, string> FilterDictionary)
        {
            if (FilterDictionary != null && !FilterDictionary.Count.Equals(0))
            {
                foreach (var f in FilterDictionary)
                {
                    string Key = f.Key;
                    object Value = f.Value;
                    bool ParsedValueBoolean;

                    string filterQuery = string.Concat(string.Empty, Key, " == @0");

                    if (Boolean.TryParse(Value.ToString(), out ParsedValueBoolean))
                    {
                        Query = Query.Where(filterQuery, ParsedValueBoolean);
                    }
                    else
                    {
                        Query = Query.Where(filterQuery, Value);
                    }
                }
            }
            return Query;
        }
        /// <summary>
        /// Helper for Filter With Condition that custom Defined,
        /// note : enum that failed to be parse as Description String it will be ignore
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="FilterModel"></param>
        /// <returns></returns>
        public static IQueryable<TModel> ConfigureFilter(IQueryable<TModel> Query, List<FilterViewModel> FilterModel)
        {
            if (FilterModel != null && !FilterModel.Count.Equals(0))
            {
                foreach (var f in FilterModel)
                {
                    string Key = f.Key;
                    object Value = f.Value;
                    bool ParsedValueBoolean;
                    string ConditionString = string.Empty;
                    //try parse condition
                    try
                    {
                        ConditionString = f.GetConditionOperator();
                    }catch(Exception ex)
                    {
                        ///if condition string cannot be parse then it will be Cancel for filter
                        continue;
                    }
                    string filterQuery = string.Concat(string.Empty, Key, " "+ConditionString+" @0");

                    if (Boolean.TryParse(Value.ToString(), out ParsedValueBoolean))
                    {
                        Query = Query.Where(filterQuery, ParsedValueBoolean);
                    }
                    else
                    {
                        Query = Query.Where(filterQuery, Value);
                    }
                }
            }
            return Query;
        }

        public static IQueryable<TModel> ConfigureOrder(IQueryable<TModel> Query, Dictionary<string, string> OrderDictionary)
        {
            /* Default Order */
            if (OrderDictionary.Count.Equals(0))
            {
                OrderDictionary.Add("LastModifiedUtc", "desc");

                Query = Query.OrderBy("LastModifiedUtc desc");
            }
            /* Custom Order */
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                try
                {
                    Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
                }
                catch (Exception)
                {
                    Query = Query.OrderBy(string.Concat(Key.Replace(".", ""), " ", OrderType));
                }
            }
            return Query;
        }

        /// <summary>
        /// [ColumnName: 1, Name: ColumnName] will produce "SELECT ColumnName, ColumnName as Name FROM TableName"
        /// </summary>
        public static IQueryable ConfigureSelect(IQueryable<TModel> Query, Dictionary<string, string> SelectDictionary)
        {
            /* Custom Select */
            if (SelectDictionary != null && !SelectDictionary.Count.Equals(0))
            {
                var listHeaderColumns = SelectDictionary.Where(d => !d.Key.Contains("."))
                    .Select(d => (d.Value == "1") ? d.Key : string.Concat(d.Value, " as ", d.Key));

                var listChildColumns = SelectDictionary
                    .Where(d => d.Key.Contains(".") && d.Value == "1")
                    .Select(s =>
                    {
                        var keys = s.Key.Split(".");
                        return new KeyValuePair<string, string>(keys[0], keys[1]);
                    })
                    .GroupBy(g => g.Key)
                    .Select(s => string.Concat(s.Key, ".Select(new(", string.Join(",", s.Select(ss => ss.Value)), ")) as ", s.Key));

                var listColumns = listHeaderColumns.Concat(listChildColumns);

                string selectedColumns = string.Join(", ", listColumns);

                var SelectedQuery = Query.Select(string.Concat("new(", selectedColumns, ")"));

                return SelectedQuery;
            }

            /* Default Select */
            return Query;
        }
    }
}