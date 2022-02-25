using System;

namespace FastBuild.Five.ExpressionBuilder
{
    [Serializable]
    internal class InvalidTypeCastingFilterException : Exception
    {
        public string PropertyName { get; set; }
        public object Value { get; }
        public Type CastingType { get; }

        public InvalidTypeCastingFilterException()
        {
        }

        public InvalidTypeCastingFilterException(string message) : base(message)
        {
        }

        public InvalidTypeCastingFilterException(string propertyName,
            object value,
            Type castingType,
            Exception innerException) : base(propertyName, innerException)
        {
            PropertyName = propertyName;
            Value = value;
            CastingType = castingType;
        }

        public override string Message => GetMessage(PropertyName);

        private string GetMessage(string propertyName)
        {
            string result = $"过滤器对象的属性类型与Entity对应的属性类型无法相互转换:{Value.GetType().Name}->{CastingType.Name}。 请检查属性\"{propertyName}\"";
            return result;
        }
    }
}