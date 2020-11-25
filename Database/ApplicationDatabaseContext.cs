using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Matrix.MvcCore.Data;

namespace SqlCode.Database
{
    public class ApplicationDatabaseContext : BaseDbContext
    {
        public ApplicationDatabaseContext(string connectionString)
            : base(connectionString) { }

        public List<Models.SqlItem> GetSqlSourceCode(DateTime? lastUpdate)
        {
            var parameters = new List<SqlParameter>();

            var updateDate = new SqlParameter("@UPDDATE", SqlDbType.DateTime)
            {
                Value = lastUpdate
            };
            parameters.Add(updateDate);

            var dataTablet = ExecuteStoredProc("[dbo].[GetSourceCode]", parameters);
            if (dataTablet == null || dataTablet.Rows.Count == 0)
            {
                return null;
            }

            return dataTablet.AsEnumerable().Select(t => new Models.SqlItem
                {
                    FullName = t.Field<string>("Name"),
                    UpdDate = t.Field<DateTime>("UpdDate"),
                    Text = t.Field<string>("Code").Trim()
            })
                .ToList();
        }
    }
}
