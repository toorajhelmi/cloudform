using System;
namespace Cloudform.Core.Deployers
{
    public class DeployingException : Exception
    {
        public const string PackageError = "Loading dependencies failed";
        public const string AzureLoginFailed = "Not able to login to the Azure subscrption";
        public const string CodePushFailed = "Encountered error while pushing code to the cloud";
        public const string QueueConnectionFailed = "Connecting to queue failed";
        public const string DeploymentinitializionFailed = "Deployment initializion failed";
        public const string DeploymentExecutionFailed = "Deployment execution failed";
        public const string ModuleInstallationFailed = "ModuleInstallationFailed";

        public DeployingException(string error, string what = null)
        {
            Error = error;
            What = what;
        }

        public string Error { get; set; }
        public string What { get; set; }
    }
}
