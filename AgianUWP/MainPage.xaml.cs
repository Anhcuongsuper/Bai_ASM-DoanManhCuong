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
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AgianUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static string API_GET_UPLOAD_URL;
        private string currentUploadUrl;
        private Member currentMember;
        private StorageFile photo;


        public static string API_GET_UPLOAD_URL1 { get => API_GET_UPLOAD_URL; set => API_GET_UPLOAD_URL = value; }

        public MainPage()
        {
            this.currentMember = new Member();
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }





        private async void doSubmit_Click(object sender, RoutedEventArgs e)
        {

            /*//register
            this.currentMember.firstName = this.FirstName.Text;
            this.currentMember.lastName = this.LastName.Text;
            this.currentMember.email = this.Email.Text;
            this.currentMember.phone = this.Phone.Text;
            this.currentMember.password = this.PassWord.Password;
            this.currentMember.introduction = this.Introduction.Text;
            this.currentMember.avatar = this.AvatarUrl.Text;
            this.currentMember.address = this.Address.Text;*/

            string jsonMember = JsonConvert.SerializeObject(this.currentMember);


            HttpClient httpClient = new HttpClient();
            var content = new StringContent(jsonMember, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(ServiceUrl.ServiceUrl.MEMBER_REGISTER, content);
            var contents = await response.Result.Content.ReadAsStringAsync();
            Debug.WriteLine(contents);
            if (response.Result.StatusCode == HttpStatusCode.Created)
            {
                Debug.WriteLine("Đã tạo thành công ! :d");
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            this.currentMember.gender = Int32.Parse(radio.Tag.ToString());

        }

        private void BirthdayPicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.currentMember.birthday = BirthdayPicker.Date.Value.ToString("yyyy-MM-dd");
        }

        private void doReset_Click(object sender, RoutedEventArgs e)
        {
            FirstName.Text = "";
            LastName.Text = "";
            Email.Text = "";
            Address.Text = "";
            Phone.Text = "";
            Introduction.Text = "";
            AvatarUrl.Text = "";
            PassWord.Password = "";

        }

        private async void Choose_Image(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

            this.photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (this.photo == null)
            {
                // User cancelled photo capture
                return;
            }
            HttpClient httpClient = new HttpClient();
            currentUploadUrl = await httpClient.GetStringAsync("https://2-dot-backup-server-002.appspot.com/get-upload-token");
            Debug.WriteLine("Upload url:" + currentUploadUrl);
            HttpUploadFile(currentUploadUrl, "myFile", "image/png");

        }

        public async void HttpUploadFile(string url, string paramName, string contentType)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";

            Stream rs = await wr.GetRequestStreamAsync();
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string header = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", paramName, "path_file", contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            // write file.
            Stream fileStream = await this.photo.OpenStreamForReadAsync();
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);

            WebResponse wresp = null;
            try
            {
                wresp = await wr.GetResponseAsync();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                //Debug.WriteLine(string.Format("File uploaded, server response is: @{0}@", reader2.ReadToEnd()));
                //string imgUrl = reader2.ReadToEnd();
                //Uri u = new Uri(reader2.ReadToEnd(), UriKind.Absolute);
                //Debug.WriteLine(u.AbsoluteUri);
                //ImageUrl.Text = u.AbsoluteUri;
                //MyAvatar.Source = new BitmapImage(u);
                //Debug.WriteLine(reader2.ReadToEnd());
                string imageUrl = reader2.ReadToEnd();
                Avatar.Source = new BitmapImage(new Uri(imageUrl, UriKind.Absolute));
                AvatarUrl.Text = imageUrl;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error uploading file", ex.StackTrace);
                Debug.WriteLine("Error uploading file", ex.InnerException);
                if (wresp != null)
                {
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }

        private static CameraCaptureUIMode GetPhoto()
        {
            return CameraCaptureUIMode.Photo;
        }


    }
}