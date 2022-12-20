namespace FairPlaySocial.Common.Global
{
    public static class ThemeConfiguration
    {
        public static class Divisions
        {
            public static string DefaultCss { get; set; } = "mb-3";
        }

        public static class Labels
        {
            public static string DefaultCss { get; set; } = "form-label";
        }
        public static class Buttons
        {
            public static string PrimaryButtonCss { get; set; } = "btn btn-primary";
            public static string SecondaryButtonCss { get; set; } = "btn btn-secondary";
        }
        public static class Selects
        {
            public static string DefaultCss { get; set; } = "form-select";
        }

        public static class GenericControls
        {
            public static string DefaultCss { get; set; } = "form-control";
        }

        public static class Icons
        {
            public static string TenantsDefaultCss { get; set; } = "bi bi-people-fill";
            public static string ChatDefaultCss { get; set; } = "bi bi-chat-dots-fill";
            public static string AddReservationDefaultCss { get; set; } = "bi bi-building";
            public static string NavigateBack { get; set; } = "bi bi-arrow-left-circle-fill";
        }

        public static class Images
        {
            public static string ThumbnailDefaultCss { get; set; } = "img-thumbnail";
        }

        public static class Chats
        {
            public static string ChatItemSection { get; set; } = "chat-item-section";
            public static string ChatSender { get; set; } = "sender";
            public static string ChatTime { get; set; } = "time";
            public static string ChatContent { get; set; } = "content";
            public static string ChatActionsBar { get; set; } = "actions-bar";
            public static string PostReplies { get; set; } = "post-replies";
        }
    }
}
