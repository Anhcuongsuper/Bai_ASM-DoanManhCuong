using AgianUWP.Entity;
using Form.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AgianUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        Member CurrentMember;


        public Login()
        {
            CurrentMember = new Member();
            this.InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentMember.email = this.Email.Text;
            this.CurrentMember.password = this.Password.Password;

            string jsonMember = JsonConvert.SerializeObject(this.CurrentMember);
            Debug.WriteLine(jsonMember);
            HttpClient httpClient = new HttpClient();
            var content = new StringContent(jsonMember, Encoding.UTF8, "application/json");
            //var result = httpClient.PostAsync("https://2-dot-backup-server-002.appspot.com/_api/v2/members/authentication", content).Result.Content.ReadAsStringAsync();
            var response = httpClient.PostAsync("https://2-dot-backup-server-002.appspot.com/_api/v2/members/authentication", content);
            var contents = await response.Result.Content.ReadAsStringAsync();
            Debug.WriteLine("Dữ liệu trả về : " + contents);
            if (response.Result.StatusCode == HttpStatusCode.Created)
            {

            }
            else
            {
                //thông báo lỗi 
                ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(contents);
                //Debug.WriteLine(errorResponse.error["firstName"]);
                if (errorResponse.error.Count > 0)
                {
                    foreach (var key in errorResponse.error.Keys)
                    {
                        var objectBykey = this.FindName(key);
                        var value = errorResponse.error[key];
                        if (objectBykey != null)
                        {

                            TextBlock textblock = objectBykey as TextBlock;
                            textblock.Text = "* " + value;
                            textblock.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }


    }
}