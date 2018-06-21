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
            chromeDriver.Quit();
        }
        string URL = "https://ogusers.com";
        ChromeDriver chromeDriver;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
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

                  //  if (hidetxtBox.Text != "show")
                  //  {
                        option.AddArgument("--headless");
                    //}
                    try
                    {
                        chromeDriver = new ChromeDriver(driverService, option);
                    }
                    catch
                    {
                        Console.Out.WriteLine("Exception");
                    }
                    Dispatcher.Invoke(() =>
                    {
                        statustext.Text = statustext.Text+"\n"+"Driver loaded";
                    });
                    chromeDriver.Navigate().GoToUrl(URL);
                    System.Threading.Thread.Sleep(10000);
                    chromeDriver.FindElementByXPath("(//a[@class=\"guestnav\" and text()=\"Login\"])[1]").Click();
                    System.Threading.Thread.Sleep(2000);
                    var USER = usernameTextBox.Text;
                    string PASSWORD = passwordTextBox.Text;
                    chromeDriver.FindElementByXPath("(//input[@name=\"username\" ])[2]").SendKeys(USER);
                    chromeDriver.FindElementByXPath("(//input[@name=\"password\" ])[2]").SendKeys(PASSWORD);
                    chromeDriver.FindElementByXPath("(//input[@name=\"submit\" ])[2]").Click();

                    statustext.Text = statustext.Text + "\n" + "Logged in";

                    System.Threading.Thread.Sleep(2000);
                    List<string> usedlist = new List<string>();
                    while (true)
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
                                statustext.Text = statustext.Text + "\n" + uRL + " Not found";
                            }
                            statustext.Text = statustext.Text + "\n" + "Getting";
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
                                    }
                                }


                            }
                            catch
                            {
                                next_text = textlist[0];
                            }
                            chromeDriver.FindElementByXPath("//textarea[@id=\"message\"]").SendKeys(next_text);
                            System.Threading.Thread.Sleep(10000);
                            chromeDriver.FindElementByXPath("//input[@id=\"quick_reply_submit\"]").Click();
                            System.Threading.Thread.Sleep(2000);

                        }
                        statustext.Text = statustext.Text + "\n" + "Sleeping for 2 hours";
                    }
                });

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }

}
