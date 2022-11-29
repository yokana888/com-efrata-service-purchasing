using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Helpers
{
    public class FilterViewModel
    {
        public string Key { get; set; }
        public ConditionValue Condition { get; set; } = ConditionValue.UNDEFINED;
        //public string ConditionName { get; set; }
        public object Value { get; set; }
        /// <summary>
        /// Convert jsonString to class FilterViewModel using NewtonSoft.Json
        /// it safe thread it will be ignore if it cannot be parse
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="conditionType"></param>
        public void SetFilterViewModel(string jsonString, ConditionType conditionType= ConditionType.ENUM_INT)
        {
            try
            {
                //temp object 
                object tempModel = new { };
                FilterViewModel model = new FilterViewModel();
                switch(conditionType)
                {
                    case ConditionType.ENUM_INT:
                        model = JsonConvert.DeserializeObject<FilterViewModel>(jsonString);
                        break;
                    case ConditionType.DESCRIPTION:
                        tempModel = JsonConvert.DeserializeObject<object>(jsonString);
                        try
                        {
                            var listTypeObject = GetListPropertyFromObject(tempModel);
                            model = ConvertListPropertyToClass(tempModel, listTypeObject,conditionType);
                        }
                        catch(Exception ex)
                        {
                            break;
                        }
                        break;
                    case ConditionType.STRING:
                        tempModel = JsonConvert.DeserializeObject<object>(jsonString);
                        try
                        {
                            var listTypeObjectString = GetListPropertyFromObject(tempModel);
                            model = ConvertListPropertyToClass(tempModel, listTypeObjectString, conditionType);
                        }
                        catch(Exception ex)
                        {
                            break;
                        }
                        break;
                }
                this.Key = model.Key;
                this.Condition = model.Condition;
                this.Value = model.Value;
            }
            catch (Exception ex)
            {
                //return new FilterViewModel();
            }
        }
        public FilterViewModel()
        {

        }
        public FilterViewModel(FilterViewModel model)
        {
            this.Key = model.Key;
            this.Condition = model.Condition;
            this.Value = model.Value;
        }
        //public FilterViewModel()
        //{

        //}

        private static string GetEnumDescription(ConditionValue value)
        {
            // Get the Description attribute value for the enum value
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        private static ConditionValue GetEnumUsingDescription(string description)
        {
            foreach (var field in typeof(ConditionValue).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (ConditionValue)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (ConditionValue)field.GetValue(null);
                }
            }

            //throw new ArgumentException("Not found.", nameof(description));
            // Or return default(T);
            return ConditionValue.UNDEFINED;
        }
        private static IList<PropertyInfo> GetListPropertyFromObject(object obj)
        {
            Type typeObject = obj.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(typeObject.GetProperties());
            return props;
        }
        private static FilterViewModel ConvertListPropertyToClass(object obj, IList<PropertyInfo> listTypeObject, ConditionType conditionType)
        {
            FilterViewModel model = new FilterViewModel();
            string keyString = listTypeObject.GetType().GetProperty("Key").GetValue(obj, null).ToString();
            var valueObject = listTypeObject.GetType().GetProperty("Value").GetValue(obj, null);
            string conditionString = listTypeObject.GetType().GetProperty("Condition").GetValue(obj, null).ToString();
            ConditionValue conditionValue = ConditionValue.UNDEFINED;
            switch (conditionType)
            {
                case ConditionType.DESCRIPTION:
                    conditionValue = GetEnumUsingDescription(conditionString);
                    break;
                case ConditionType.STRING:
                    conditionValue = (ConditionValue)Enum.Parse(model.GetType(), conditionString);
                    break;
                default:
                    break;
            }
            model.Key = keyString;
            model.Value = valueObject;
            model.Condition = conditionValue;
            return model;
        }
        /// <summary>
        /// this not thread safe 
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static List<FilterViewModel> ConvertJsonAsList(string jsonString, ConditionType conditionType = ConditionType.ENUM_INT)
        {
            List<object> listObject = JsonConvert.DeserializeObject<List<object>>(jsonString,new JsonSerializerSettings { 
            Formatting = Formatting.None
            });
            List<FilterViewModel> listFilter = new List<FilterViewModel>();
            listObject.ForEach(obj =>
            {
                object parseObject = JsonConvert.DeserializeObject<object>(obj.ToString());
                var model = new FilterViewModel();
                model.SetFilterViewModel(parseObject.ToString(), conditionType);
                //listFilter.Add(new FilterViewModel().SetFilterViewModel(obj.ToString(), conditionType));
                listFilter.Add(model);
            });
            return listFilter;

        }
        /// <summary>
        /// Get Enum Condition as String Operator
        /// </summary>
        /// <returns></returns>
        public string GetConditionOperator()
        {
            return GetEnumDescription(Condition);
        }
    }
    public enum ConditionValue
    {
        UNDEFINED = 0,
        [Description("")]
        EMPTY = 1,
        [Description("==")]
        EQUALS = 2,
        [Description("!=")]
        NOT_EQUALS = 3,
        [Description(">")]
        GREATER_THAN = 4,
        [Description("<")]
        LESS_THAN = 5,
        [Description(">=")]
        GREATER_AND_EQUAL_THAN = 6,
        [Description("<=")]
        LESS_AND_EQUAL_THAN = 7
    }
    public enum ConditionType 
    {
        ENUM_INT = 0,
        STRING= 1,
        DESCRIPTION = 2
    }
}
