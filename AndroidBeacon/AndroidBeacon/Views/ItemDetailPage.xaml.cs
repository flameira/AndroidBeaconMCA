using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AndroidBeacon.Models;
using AndroidBeacon.ViewModels;

namespace AndroidBeacon.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage(Item item)
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel (item);
            
            Title = item.Description;
        }

        protected override void OnAppearing ()
        {
            base.OnAppearing ();

            (BindingContext as ItemDetailViewModel).LoadContent ();
        }
        
    }
}