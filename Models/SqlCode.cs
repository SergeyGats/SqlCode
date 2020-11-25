using System;

namespace SqlCode.Models
{
    public class SqlItem
    {
        public string FullName { get; set; }
        public DateTime UpdDate { get; set; }
        public string Text { get; set; }

        public string Schema => FullName.Split('.')[0];
        public string Name => FullName.Split('.')[1];
        public string FileName => $"{FullName}.sql";
        public string SqlTextWithPrefix { get; set; }
    }
}
