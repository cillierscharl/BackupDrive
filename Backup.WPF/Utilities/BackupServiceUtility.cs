
namespace Backup.WPF.Utilities
{
    public static class BackupServiceUtility
    {
        private static BackupService.BackupServiceClient client;
        public static BackupService.BackupServiceClient GetServiceClient()
        {
            if (client == null || client.State == System.ServiceModel.CommunicationState.Faulted)
            {
                client = new BackupService.BackupServiceClient();
                return client;
            }
            return client;
        }
    }
}
