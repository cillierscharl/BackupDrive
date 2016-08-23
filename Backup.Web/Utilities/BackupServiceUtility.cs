
namespace Backup.MVC
{
    public static class BackupServiceUtility
    {
        private static Backup.Web.BackupService.BackupServiceClient client;
        public static Backup.Web.BackupService.BackupServiceClient GetServiceClient()
        {
            if (client == null || client.State == System.ServiceModel.CommunicationState.Faulted)
            {
                client = new Backup.Web.BackupService.BackupServiceClient();
                return client;
            }
            return client;
        }
    }
}
