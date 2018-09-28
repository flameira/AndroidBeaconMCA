namespace AndroidBeacon.ViewModels
{
    using System;
    using Models;
    using Services;
    using Xamarin.Forms;

    public class ItemDetailViewModel : BaseViewModel
    {
        private bool contectLoaded;
        private readonly ILogService logService;
        private readonly bool screenLoaded;

        public ItemDetailViewModel(Item item = null)
        {
            Title = item?.Text;
            Item = item;

            logService = DependencyService.Get<ILogService>();
            LoadContent();
            screenLoaded = true;
        }

        public string FileContent { get; private set; }
        public HtmlWebViewSource FileContentAsHtml { get; private set; }
        public Item Item { get; set; }


        public async void LoadContent()
        {
            if (contectLoaded) return;

            FileContentAsHtml = new HtmlWebViewSource();
            FileContentAsHtml.Html = GetFileContentAsHtml("Loading...");
            OnPropertyChanged("FileContentAsHtml");

            if (screenLoaded)
            {
                FileContent = await logService.GetLogContentAsync(Item.Description);
                FileContentAsHtml.Html = GetFileContentAsHtml(FileContent);
                OnPropertyChanged("FileContentAsHtml");

                contectLoaded = true;
            }
        }

        private string GetFileContentAsHtml(string content)
        {
            var html = content.Replace(Environment.NewLine, "<br/>");
            return "<html>" +
                   "<header><style>body{font-family: Helvetica, Arial, Sans-Serif; font-size: 10px }</style></header>" +
                   "<body><p>" + html + "</p></body>" + "</html>";
        }
    }
}