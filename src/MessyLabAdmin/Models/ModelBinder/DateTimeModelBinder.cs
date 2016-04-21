using Microsoft.AspNet.Mvc.Internal;
using Microsoft.AspNet.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MessyLabAdmin.Models.ModelBinder
{
    public class DateTimeModelBinder : IModelBinder
    {

        private const string format = "dd.MM.yyyy HH:mm:ss";
        private readonly CultureInfo enUS = new CultureInfo("en-US");

        public Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        {
            var key = bindingContext.ModelName;
            var val = bindingContext.ValueProvider.GetValue(key);
            if (val == ValueProviderResult.None)
            {
                return ModelBindingResult.NoResultAsync;
            }

            if (bindingContext.ModelType == typeof(DateTime) || Nullable.GetUnderlyingType(bindingContext.ModelType) == typeof(DateTime))
            {
                DateTime model;
                if(DateTime.TryParseExact(val.FirstValue, format, enUS, DateTimeStyles.None, out model))
                {
                    return Task.FromResult(ModelBindingResult.Success(key, model));
                }
            }

            return ModelBindingResult.NoResultAsync;

        }

    }
}

