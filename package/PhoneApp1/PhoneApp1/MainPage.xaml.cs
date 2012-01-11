using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Facebook;

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

var client = new FacebookClient("access_token");

client.GetCompleted += (sender, e) =>
{
    var me = e.GetResultData<JsonObject>();
    string firstName = (string)me["first_name"];
    string lastName = (string)me["last_name"];
    string email = (string)me["email"];
};

client.GetAsync("me");

        }

    }
}