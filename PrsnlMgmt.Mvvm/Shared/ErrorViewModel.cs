using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrsnlMgmt.Mvvm.Shared
{
    public class ErrorViewModel : BindableBase
    {
        public RelayCommand ContinueCommand { get; private set; }
        public event Action Done;

        public ErrorViewModel(Exception ex, string message, string srcFilePath, int srcLineNum, string srcMemberName, string srcTypeName)
        {
            Exception = ex;
            Message = message;
            SourceFilePath = srcFilePath;
            SourceLineNumber = srcLineNum;
            SourceMemberName = srcMemberName;
            SourceTypeName = srcTypeName;
            ContinueCommand = new RelayCommand(() => Done?.Invoke());
        }

        public Exception Exception { get; private set; }
        public string Message { get; private set; }

        public string SourceFilePath { get; private set; }
        public int SourceLineNumber { get; private set; }

        public string SourceTypeName { get; private set; }
        public string SourceMemberName { get; private set; }

        public string Summary { 
            get {
                StringBuilder sb = new StringBuilder();
                if (string.IsNullOrEmpty(Message))
                    sb.AppendLine("An error has occurred.");
                else
                    sb.AppendLine(Message);
                sb.AppendLine("\nat\n");
                sb.AppendLine($"Source File Name:   {(!string.IsNullOrEmpty(SourceFilePath) ? Path.GetFileName(SourceFilePath) : "[NOT AVAILABLE]")}");
                sb.AppendLine($"Source Type Name:   {(!string.IsNullOrEmpty(SourceTypeName) ? SourceTypeName : "[NOT AVAILABLE]")}");
                sb.AppendLine($"Source Member Name: {(!string.IsNullOrEmpty(SourceMemberName) ? SourceMemberName : "[NOT AVAILABLE]")}");
                sb.AppendLine($"Source Line Number: {(SourceLineNumber > 0 ? SourceLineNumber.ToString() : "[NOT AVAILABLE]")}");
                sb.AppendLine();
                if(Exception != null)
                {
                    sb.AppendLine($"Exception Type:     {Exception?.GetType().FullName}");
                    sb.AppendLine($"Message: {Exception.Message}");
                }

                return sb.ToString();
            } 
        }
    }
}
