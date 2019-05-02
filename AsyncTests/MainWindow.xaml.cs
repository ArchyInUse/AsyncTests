using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace AsyncTests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string BaseUrl = "http://docsmaster.net/ManualEntry/UnityDocs/";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if(InputBox.Text.Replace(" ", string.Empty) != string.Empty)
            {
                var ResponseBody = GetDataFromApi(InputBox.Text.Replace(" ", string.Empty));
                ClearData();
                InputBox.Text = "";
                await ResponseBody;

                await SortData(ResponseBody.Result);
            }
            else
            {
                await ClearData();
                StateBox.Text = "Error - Input cannot be empty.";
            }
        }


        // Returns null if no manual found, otherwise returns a ResponseBody
        private async Task<ResponseType> GetDataFromApi(string Entry)
        {
            HttpResponseMessage response;
            using (HttpClient Client = new HttpClient())
            {
                response = await Client.GetAsync(BaseUrl + Entry);
            }

            if (response.IsSuccessStatusCode)
            {
                StateBox.Text = "Success";

                return JsonConvert.DeserializeObject<ResponseType>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                StateBox.Text = "Error - Not found.";

                return null;
            }
        }

        private async Task SortData(ResponseType Response)
        {
            if (Response != null)
            {
                EntryNameBox.Text = Response.entryName;
                DescriptionBox.Text = Response.description;
                HyperL.NavigateUri = Response.fullReferenceLink;
                HyperL.Inlines.All(x => HyperL.Inlines.Remove(x));
                HyperL.Inlines.Add(Response.fullReferenceLink.ToString());
                ManualVerBox.Text = Response.manualVersion;
            }
            else
            {
                ClearData();
            }
        }

        public async Task ClearData()
        {
            EntryNameBox.Text = string.Empty;
            DescriptionBox.Text = string.Empty;
            HyperL.Inlines.All(x => HyperL.Inlines.Remove(x));
            HyperL.NavigateUri = null;
            ManualVerBox.Text = "";
        }
    }
}
