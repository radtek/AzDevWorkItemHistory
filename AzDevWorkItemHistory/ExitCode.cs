using System;
using Microsoft.VisualStudio.Services.Common;

namespace WorkItemHistory
{
    public class ExitCode
    {
        public static ExitCode Success = new ExitCode(0, "Success");
        public static ExitCode ParseError = new ExitCode(5, "Error parsing program arguments.");
        public static ExitCode GeneralException(Exception e) => new ExitCode(-1, e.ToString());
        public static ExitCode DuplicateUri(Uri uri) => new ExitCode(2, $"This URI ({uri}) already exists. You need to 'logout' with that URI if you wish to update the credentials for that URI.");
        public static ExitCode NeedToLogin(Uri uri) => new ExitCode(3, $"You'll need to login to {uri} first to perform this operation.");
        public static ExitCode Unauthorized(VssUnauthorizedException exception, Uri azureUri) => new ExitCode(4, $"You need to re-login to {azureUri}. Your access token as either been revoked or expired. Full exception below.{Environment.NewLine}{Environment.NewLine}{exception}");

        private ExitCode(int value, string message)
        {
            Value = value;
            Message = message;
        }

        public int Value { get; }
        public string Message { get; }
    }
}