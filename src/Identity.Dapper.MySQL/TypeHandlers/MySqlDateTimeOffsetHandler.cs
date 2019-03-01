using Dapper;
using System;
using System.Data;

namespace Identity.Dapper.MySQL.TypeHandlers
{
    public class MySqlDateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset?>
    {
        // assume UTC in and out

        public override DateTimeOffset? Parse(object value)
        {
            if (value == null || value == DBNull.Value) return null;

            var result = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
            return new DateTimeOffset(result);
        }

        public override void SetValue(IDbDataParameter parameter, DateTimeOffset? value)
        {
            parameter.DbType = DbType.DateTime;
            parameter.Value = value.HasValue ? value.Value.UtcDateTime : (object)DBNull.Value;
        }
    }
}
