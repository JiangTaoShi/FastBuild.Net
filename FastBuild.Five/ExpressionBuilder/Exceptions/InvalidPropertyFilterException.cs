using System;

namespace FastBuild.Five.ExpressionBuilder
{

    /// <summary>
    /// 异常:无效的属性
    /// </summary>
    public class InvalidPropertyFilterException : Exception
    {

        public string PropertyName { get; set; }
        public InvalidPropertyFilterException()
        {
        }
        public InvalidPropertyFilterException(string message) : base(message)
        {
        }

        public InvalidPropertyFilterException(string propertyName, Exception innerException) : base(propertyName, innerException)
        {
            PropertyName = propertyName;
        }

        public override string Message => GetMessage(PropertyName);

        private string GetMessage(string propertyName)
        {
            string result = $"无法在Entity中找到要匹配的过滤器属性\"{propertyName}\"";
            return result;
        }


    }
}