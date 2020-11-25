namespace SqlCode
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var cd = new GetSqlSourceCode();
            //cd.SaveProceduresToFiles();
            cd.SaveFunctionsToFiles();
            //cd.SaveViewsToFiles();
        }
    }
}
