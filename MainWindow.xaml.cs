using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace Ogusers_bumping_threads
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string textfile;
        string urlfile;
        List<string> textlist = new List<string>();
        List<string> urllist = new List<string>();
        Boolean pause = false;
        List<string> usedlist;
        public MainWindow()
        {
            InitializeComponent();
        }
        

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            dlg.Filter = "Text Files (*.txt)|*.txt";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                textfile = dlg.FileName;
                button1.Content = dlg.SafeFileName;
                textlist.Clear();

                using (var rd = new StreamReader(dlg.FileName))
                {
                    while (!rd.EndOfStream)
                    {
                        var splits = rd.ReadLine();
                        textlist.Add(splits);
                    }
                }
                Console.WriteLine("Content 1:");
                foreach (var element in textlist)
                    Console.WriteLine(element);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            dlg.Filter = "Text files (*.txt)|*.txt";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                textfile = dlg.FileName;
                button2.Content = dlg.SafeFileName;
                urllist.Clear();

                using (var rd = new StreamReader(dlg.FileName))
                {
                    while (!rd.EndOfStream)
                    {
                        var splits = rd.ReadLine();
                        urllist.Add(splits);
                    }
                }
                Console.WriteLine("Content 2:");
                foreach (var element in urllist)
                    Console.WriteLine(element);
            }
        }

        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            if (chromeDriver != null)
                chromeDriver.Quit();
        }
        string URL = "https://ogusers.com";
        ChromeDriver chromeDriver;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            button1_Copy2.IsEnabled = false;
            button1_Copy1.IsEnabled = false;
            statuslabel.Content = "Started";
            Console.Out.WriteLine(button1.Content);
            Console.Out.WriteLine(button2.Content);
            int delay = 30;
            if (Int32.TryParse(delaytxtBox.Text, out delay))
            {
                if (delay < 60)
                {
                    delay = 60;
                }
            }
            else
            {
                delay = 60;
            }
            if (button1.Content.Equals("Load Text") || button2.Content.Equals("Load Urls"))
            {
                MessageBox.Show("Please select valid text and url file");
            }
            else
            {

                var USER = usernameTextBox.Text;
                string PASSWORD = passwordTextBox.Password;
                string factorAuth = factorAuthButn.Text;
                string toShowCheck = hidetxtBox.Text;
                if (chromeDriver == null)
                {

                    Task.Run(() =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            statustext.Text = "Loading driver";
                        });
                        var option = new ChromeOptions();
                        var driverService = ChromeDriverService.CreateDefaultService();
                        driverService.HideCommandPromptWindow = true;

                        if (!toShowCheck.Equals("show"))
                        {
                            option.AddArgument("--headless");
                        }
                        try
                        {
                            chromeDriver = new ChromeDriver(driverService, option);
                        }
                        catch
                        {
                            MessageBox.Show("Error while loading chromedriver");
                            Console.Out.WriteLine("Exception! Chrome driver not loaded");
                        }
                        Dispatcher.Invoke(() =>
                        {
                            statustext.Text = "Driver loaded" + "\n" + statustext.Text + "\n";
                        });
                        if (!pause)
                        {
                            try
                            {
                                chromeDriver.Navigate().GoToUrl(URL);
                                System.Threading.Thread.Sleep(10000);
                                chromeDriver.FindElementByXPath("(//a[@class=\"guestnav\" and text()=\"Login\"])[1]").Click();
                                System.Threading.Thread.Sleep(4000);


                                Dispatcher.Invoke(() =>
                                {
                                    statustext.Text = "Sending " + USER + " " + PASSWORD + "\n" + statustext.Text + "\n\n";
                                });
                                Console.Out.WriteLine("Sending username and pass");
                                try
                                {
                                    chromeDriver.FindElementByXPath("(//input[@name=\"username\" ])[2]").SendKeys(USER);
                                    chromeDriver.FindElementByXPath("(//input[@name=\"password\" ])[2]").SendKeys(PASSWORD);
                                    if (!factorAuth.Equals(string.Empty))
                                    {
                                        chromeDriver.FindElementByXPath("(//input[@name=\"2facode\" ])[2]").SendKeys(factorAuth);
                                    }
                                    chromeDriver.FindElementByXPath("(//input[@name=\"submit\" ])[2]").Click();
                                }
                                catch
                                {
                                    Console.Out.WriteLine("Exception while sending keys");
                                }
                                Dispatcher.Invoke(() =>
                                {
                                    statustext.Text = "Logged in" + "\n" + statustext.Text + "\n\n";
                                });

                                System.Threading.Thread.Sleep(2000);
                                usedlist = new List<string>();
                                while (true)
                                {
                                    if (pause)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        foreach (var uRL in urllist)
                                        {
                                            try
                                            {
                                                chromeDriver.Navigate().GoToUrl(uRL);
                                                if (chromeDriver.PageSource.Contains("404 not"))
                                                {
                                                    throw new Exception("Not loaded");
                                                }
                                            }
                                            catch
                                            {
                                                System.Threading.Thread.Sleep(2000);
                                                chromeDriver.Navigate().GoToUrl(uRL);
                                                Dispatcher.Invoke(() =>
                                                {
                                                    statustext.Text = uRL + " Not found" + "\n" + statustext.Text + "\n\n";
                                                });
                                            }
                                            /*
                                            Dispatcher.Invoke(() =>
                                            {
                                                statustext.Text = "Getting" + "\n" + statustext.Text + "\n";
                                            });*/
                                            System.Threading.Thread.Sleep(2000);
                                            if (usedlist.Count == textlist.Count)
                                            {
                                                usedlist.Clear();
                                            }
                                            string next_text;
                                            try
                                            {
                                                Random rnd = new Random();
                                                while (true)
                                                {
                                                    int r = rnd.Next(textlist.Count);
                                                    next_text = textlist[r];
                                                    if (!usedlist.Contains(next_text))
                                                    {
                                                        next_text = textlist[r];
                                                        break;
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                next_text = textlist[0];
                                            }
                                            try
                                            {
                                                chromeDriver.FindElementByXPath("//textarea[@id=\"message\"]").SendKeys(next_text);
                                                System.Threading.Thread.Sleep(10000);
                                                chromeDriver.FindElementByXPath("//input[@id=\"quick_reply_submit\"]").Click();
                                                labelvalue.Content = "sum";
                                                Dispatcher.Invoke(() =>
                                                {
                                                    statustext.Text = "Currently posted \"" + next_text + "\" on " + uRL + "\n" + statustext.Text + "\n\n";
                                                });
                                                System.Threading.Thread.Sleep(20000);
                                            }
                                            catch
                                            {
                                                Console.Out.WriteLine("Process pasued");
                                                goto Outer;
                                            }


                                        }
                                        Dispatcher.Invoke(() =>
                                        {
                                            statustext.Text = "Sleeping for " + delay + " minutes" + "\n" + statustext.Text + "\n";
                                        });
                                        print_wait(delay);
                                        Outer:
                                        continue;

                                    }
                                }
                            }
                            catch
                            {
                                Console.Out.WriteLine("Prcess paused");
                            }
                        }
                    });
                }

            }
        }

        private void print_wait(int delay)
        {
            int nxt = delay * 60;
            while (nxt > 0)
            {
                nxt -= 1;
                Dispatcher.Invoke(() =>
                {
                    statustext.Text = statustext + "\n" + "NEXT RUN IN :: " + nxt;
                });
                System.Threading.Thread.Sleep(1000);
            }

            Dispatcher.Invoke(() =>
            {
                statustext.Text = statustext + "\n" + "***********************";
            });

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void hidetxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            button1_Copy2.IsEnabled = true;
            button1_Copy1.IsEnabled = true;

            if (chromeDriver != null)
            {
                chromeDriver.Quit();
                chromeDriver = null;
            }
            statuslabel.Content = "Stopped";
            pause = true;
            statustext.Text = "Posting paused" + "\n" + statustext.Text + "\n\n";

        }

        private void passwordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

}
