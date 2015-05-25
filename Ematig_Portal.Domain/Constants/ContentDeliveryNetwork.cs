namespace Ematig_Portal.Domain.Constants
{
    public static class ContentDeliveryNetwork
    {
        public static class Google
        {
            public const string Domain = "*.googleapis.com";
            public const string Domain1 = "*.gstatic.com";
        }

        public static class Microsoft
        {
            public const string Domain = "ajax.aspnetcdn.com";
            public const string ModernizrUrl = "//ajax.aspnetcdn.com/ajax/modernizr/modernizr-2.8.3.js";
            public const string BootstrapUrl = "//ajax.aspnetcdn.com/ajax/bootstrap/3.3.4/bootstrap.min.js";
            public const string RespondUrl = "//ajax.aspnetcdn.com/ajax/respond/1.4.2/respond.js";
        }
    }
}