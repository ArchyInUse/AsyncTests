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
                ClearData();
                StateBox.Text = "Error - Input cannot be empty.";
            }
        }


        // Returns null if no manual found, otherwise returns a ResponseBody
        private async Task<ResponseType> GetDataFromApi(string Entry)
        {
            string responseBody;
            using (HttpClient Client = new HttpClient())
            {
                try
                {
                    responseBody = await Client.GetStringAsync(BaseUrl + Entry);
                }
                catch(Exception)
                {
                    StateBox.Text = "Error - Not found.";
                    responseBody = null;
                }
            }

            if(responseBody != null)
            {
                StateBox.Text = "Success";

                return JsonConvert.DeserializeObject<ResponseType>(responseBody);
            }

            return null;
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

        public void ClearData()
        {
            EntryNameBox.Text = string.Empty;
            DescriptionBox.Text = string.Empty;
            HyperL.Inlines.All(x => HyperL.Inlines.Remove(x));
            HyperL.NavigateUri = null;
            ManualVerBox.Text = "";
        }
    }
}
