using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SqlCode.Database;
using SqlCode.Extensions;

namespace SqlCode
{
    public class GetSqlSourceCode
    {
        private const string ConnectionStringName = "MpcConnection";

        private const string DatabaseFolderPath = "C:\\MPC-Ones\\Databases\\Dash_Anonymise";

        private const string ProceduresFolderName = "004-Procedures";
        private const string ViewsFolderName = "003-Views";
        private const string FunctionsFolderName = "002-Functions";

        private const string CreateProcedureText1 = "CREATE PROCEDURE ";
        private const string CreateProcedureText2 = "CREATE PROC ";

        private const string CreateFunctionText = "CREATE FUNCTION ";

        private const string CreateViewText = "CREATE VIEW ";

        private readonly string _proceduresFolderPath;
        private readonly string _viewsFolderPath;
        private readonly string _functionsFolderPath;

        private readonly ApplicationDatabaseContext _context;

        public GetSqlSourceCode()
        {
            _context = new ApplicationDatabaseContext(ConnectionStringName);

            _proceduresFolderPath = $"{DatabaseFolderPath}\\{ProceduresFolderName}";
            _viewsFolderPath = $"{DatabaseFolderPath}\\{ViewsFolderName}";
            _functionsFolderPath = $"{DatabaseFolderPath}\\{FunctionsFolderName}";
        }

        public void SaveProceduresToFiles()
        {
            var existingFilesTmp = Directory.GetFiles(_proceduresFolderPath, "*.sql").ToList();
            var existingFiles = new List<string>();
            foreach (var filename in existingFilesTmp)
            {
                var fileInfo = new FileInfo(filename);
                existingFiles.Add(fileInfo.Name);
            }

            var sqlItems = _context.GetSqlSourceCode(null);
            foreach (var sqlItem in sqlItems)
            {
                if (existingFiles.Contains(sqlItem.FileName))
                {
                    continue;
                }

                if (sqlItem.Schema.ToLower() == "service")
                {
                    continue;
                }

                if (sqlItem.Text.Contains(CreateProcedureText1, StringComparison.OrdinalIgnoreCase) ||
                    sqlItem.Text.Contains(CreateProcedureText2, StringComparison.OrdinalIgnoreCase))
                {
                    ReplaceCreateProcedureText(sqlItem);
                    InsertProcedurePrefix(sqlItem);

                    var filePathName = Path.Combine(_proceduresFolderPath, sqlItem.FileName);
                    var fileInfo = new FileInfo(filePathName);

                    var fileStream = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fileStream.Position = 0;

                    fileStream.Position = 0;
                    Encoding utf8WithoutBom = new UTF8Encoding(true);
                    var encBytes = utf8WithoutBom.GetBytes(sqlItem.SqlTextWithPrefix);

                    fileStream.Write(encBytes, 0, encBytes.Count());
                    fileStream.SetLength(encBytes.Count());
                    fileStream.Close();
                }
            }
        }

        public void SaveFunctionsToFiles()
        {
            var existingFilesTmp = Directory.GetFiles(_functionsFolderPath, "*.sql").ToList();
            var existingFiles = new List<string>();
            foreach (var filename in existingFilesTmp)
            {
                var fileInfo = new FileInfo(filename);
                existingFiles.Add(fileInfo.Name);
            }

            var sqlItems = _context.GetSqlSourceCode(null);
            foreach (var sqlItem in sqlItems)
            {
                if (existingFiles.Contains(sqlItem.FileName))
                {
                    continue;
                }

                if (sqlItem.Schema.ToLower() == "service")
                {
                    continue;
                }

                if (sqlItem.Text.Contains(CreateFunctionText, StringComparison.OrdinalIgnoreCase))
                {
                    ReplaceCreateFunctionText(sqlItem);
                    InsertFunctionPrefix(sqlItem);

                    var filePathName = Path.Combine(_functionsFolderPath, sqlItem.FileName);
                    var fileInfo = new FileInfo(filePathName);

                    var fileStream = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fileStream.Position = 0;

                    fileStream.Position = 0;
                    Encoding utf8WithoutBom = new UTF8Encoding(true);
                    var encBytes = utf8WithoutBom.GetBytes(sqlItem.SqlTextWithPrefix);

                    fileStream.Write(encBytes, 0, encBytes.Count());
                    fileStream.SetLength(encBytes.Count());
                    fileStream.Close();
                }
            }
        }

        public void SaveViewsToFiles()
        {
            var existingFilesTmp = Directory.GetFiles(_viewsFolderPath, "*.sql").ToList();
            var existingFiles = new List<string>();
            foreach (var filename in existingFilesTmp)
            {
                var fileInfo = new FileInfo(filename);
                existingFiles.Add(fileInfo.Name);
            }

            var sqlItems = _context.GetSqlSourceCode(null);
            foreach (var sqlItem in sqlItems)
            {
                if (existingFiles.Contains(sqlItem.FileName))
                {
                    continue;
                }

                if (sqlItem.Schema.ToLower() == "service")
                {
                    continue;
                }

                if (sqlItem.Text.Contains(CreateViewText, StringComparison.OrdinalIgnoreCase))
                {
                    ReplaceCreateViewText(sqlItem);
                    InsertViewPrefix(sqlItem);

                    var filePathName = Path.Combine(_viewsFolderPath, sqlItem.FileName);
                    var fileInfo = new FileInfo(filePathName);

                    var fileStream = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fileStream.Position = 0;

                    fileStream.Position = 0;
                    Encoding utf8WithoutBom = new UTF8Encoding(true);
                    var encBytes = utf8WithoutBom.GetBytes(sqlItem.SqlTextWithPrefix);

                    fileStream.Write(encBytes, 0, encBytes.Count());
                    fileStream.SetLength(encBytes.Count());
                    fileStream.Close();
                }
            }
        }

        private void ReplaceCreateProcedureText(Models.SqlItem sqlItem)
        {
            sqlItem.Text = Regex.Replace(sqlItem.Text, CreateProcedureText2, CreateProcedureText1, RegexOptions.IgnoreCase);
            sqlItem.Text = Regex.Replace(sqlItem.Text, CreateProcedureText1, CreateProcedureText1, RegexOptions.IgnoreCase);
        }

        private void InsertProcedurePrefix(Models.SqlItem item)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(
                $"IF OBJECT_ID('[{item.Schema}].[{item.Name}]') IS NOT NULL\n" +
                $"    DROP PROCEDURE [{item.Schema}].[{item.Name}]\n" +
                $"GO");

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            stringBuilder.Append(item.Text);

            item.SqlTextWithPrefix = stringBuilder.ToString();
        }

        private void ReplaceCreateFunctionText(Models.SqlItem sqlItem)
        {
            sqlItem.Text = Regex.Replace(sqlItem.Text, CreateFunctionText, CreateFunctionText, RegexOptions.IgnoreCase);
        }

        private void InsertFunctionPrefix(Models.SqlItem item)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(
                $"IF OBJECT_ID('[{item.Schema}].[{item.Name}]') IS NOT NULL\n" +
                $"    DROP FUNCTION [{item.Schema}].[{item.Name}]\n" +
                $"GO");

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            stringBuilder.Append(item.Text);

            item.SqlTextWithPrefix = stringBuilder.ToString();
        }

        private void ReplaceCreateViewText(Models.SqlItem sqlItem)
        {
            sqlItem.Text = Regex.Replace(sqlItem.Text, CreateViewText, CreateViewText, RegexOptions.IgnoreCase);
        }

        private void InsertViewPrefix(Models.SqlItem item)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(
                $"IF OBJECT_ID('[{item.Schema}].[{item.Name}]') IS NOT NULL\n" +
                $"    DROP VIEW [{item.Schema}].[{item.Name}]\n" +
                $"GO");

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            stringBuilder.Append(item.Text);

            item.SqlTextWithPrefix = stringBuilder.ToString();
        }
    }
}
