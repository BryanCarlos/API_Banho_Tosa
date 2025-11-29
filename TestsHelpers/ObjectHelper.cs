namespace TestsHelpers
{
    public static class ObjectHelper
    {
        public static TEntity SetProperty<TEntity>(this TEntity obj, string propertyName, object value)
        {
            var property = obj?.GetType().GetProperty(propertyName);

            if (property is null)
            {
                throw new ArgumentNullException(propertyName);
            }

            property.SetValue(obj, value);

            return obj;
        }
    }
}
