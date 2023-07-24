using DotNetBrowser.Browser;
using DotNetBrowser.Dom;
using DotNetBrowser.Dom.Events;
using DotNetBrowser.Engine;
using DotNetBrowser.Input.Keyboard.Events;
using DotNetBrowser.Input.Keyboard;
using DotNetBrowser.Navigation.Events;
using DotNetBrowser.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using DotNetBrowser.Handlers;
using System.Diagnostics;
using System.Security.Policy;
using DotNetBrowser.Browser.Handlers;
using DotNetBrowser.Passwords.Handlers;
//using DotNetBrowser.Wpf;

namespace InstaBrowser
{
    public partial class Form1 : Form
    {
        private const string Url = "https://www.instagram.com/accounts/login/";
        //public static string hashTags = "#Love";
        private  IBrowser browser;
        private  IEngine engine;
        private bool _isLoggedIn;
        //private IBrowserView _browserView;

        string[] hashtagsArray = {
                "#love",
                "#instagood",
                "#photooftheday",
                "#fashion",
                "#beautiful",
                "#happy",
                "#cute",
                "#followme",
                "#tbt",
                "#like4like",
                "#follow",
                "#picoftheday"
            };
        static string GetRandomHashtag(string[] hashtags)
        {
            Random random = new Random();
            int index = random.Next(hashtags.Length);
            return hashtags[index];
        }
        public System.Windows.Forms.Form MainControl { get; set; }

        private delegate bool IsElementLoadedOnDOM(IDocument oDocument);
        public enum StepCompleted
        {
            Step_Init = 0,
            Step_ElementFound,
            Step_LoggedIn,
            Step_ClickedHashTag,

           

            Step_EndProcess
        }
        public static StepCompleted CurrentStep = StepCompleted.Step_Init;
        public Form1()
        {
            

            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized;
            //this.Size = new System.Drawing.Size(1366, 768);
            this.Size = new System.Drawing.Size(1280, 720);
            button1.Click += button1_Click;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            browser?.Dispose();
            engine?.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BrowserView browserView = new BrowserView
            {
                Dock = DockStyle.Fill
            };


            EngineOptions engineOptions = new EngineOptions.Builder
            {
                RenderingMode = RenderingMode.HardwareAccelerated,
                LicenseKey = "6P921J2OGCX0GBAKBUHTRJ2TGB173QCRFNHXHSV8XHI5TN6NNX8E7C2ICLLUUFN8CX4D"

            }.Build();
            engine = EngineFactory.Create(engineOptions);

            // Create the IBrowser instance.
            browser = engine.CreateBrowser();
           
            // Add the BrowserView control to the Form.
            Controls.Add(browserView);
            FormClosed += Form1_FormClosed;

            // Initialize the Windows Forms BrowserView control.
            browserView.InitializeFrom(browser);
            //browser.Navigation.LoadUrl(Url);
            //browser.Passwords.SavePasswordHandler =
            //     new Handler<DotNetBrowser.Passwords.Handlers.SavePasswordParameters, DotNetBrowser.Passwords.Handlers.SavePasswordResponse>(p =>
            //      {
            //         return DotNetBrowser.Passwords.Handlers.SavePasswordResponse.NeverSave;
            //      });
            //browser.Navigation.FrameLoadFinished += Navigation_FrameLoadFinished; 
            //browser.Navigation.FrameLoadFinished += Navigation_LoadFinished;
            Task.Run(() =>
            {
                //browser.MainFrame.Browser.CreatePopupHandler =
                //  new Handler<CreatePopupParameters, CreatePopupResponse>(p =>
                //  {

                //      browser.Navigation.LoadUrl(p.TargetUrl);


                //      return CreatePopupResponse.Suppress();
                //  });
                browser.Passwords.SavePasswordHandler =
                    new Handler<SavePasswordParameters, SavePasswordResponse>(p =>
                    {
                        return SavePasswordResponse.NeverSave;
                    });

                browser.Navigation.FrameLoadFinished += Navigation_FrameLoadFinished;
                browser.Navigation.FrameLoadFinished += Navigation_LoadFinished;
                browser.Navigation.LoadUrl(Url).Wait();
                //    // After the page is loaded successfully, we can configure the observer.

            });



        }
        private void Navigation_LoadFinished(object sender, FrameLoadFinishedEventArgs e)
        {
           
            if (e.Browser.MainFrame == null) { return; }
            if (e.Browser.MainFrame.IsMain)
            {
               
                IDocument document = e.Browser.MainFrame.Document;
                string URL = e.Browser.Url;
                //IElement oDiv;
                if ((URL == "https://www.instagram.com/accounts/onetap/?next=%2F"))
                {


                    WaitForElementToLoad(e, CurrentStep, x => (e.Browser.MainFrame.Document.GetElementByClassName("_ac8f") != null));
                    Thread.Sleep(500);
                    //oDiv = (IElement)e.Browser.MainFrame.Document.GetElementsByTagName("div").Where(s => s.InnerText.Trim() == "Not Now".ToLower()).FirstOrDefault();

                    if (e.Browser.MainFrame.Document.GetElementsByTagName("div").Where(s => s.InnerText.Trim() == "Not Now").FirstOrDefault() != null)
                    {
                        //e.Browser.MainFrame.Document.GetElementsByTagName("div").Where(s => s.InnerText.Trim() == "Not Now").FirstOrDefault().Click();
                        string script = "var element = document.querySelector('.x1i10hfl'); if (element) element.click();";
                        e.Browser.MainFrame.ExecuteJavaScript(script);
                        Thread.Sleep(500);
                        Application.DoEvents();


                    }

                    return;
                }

                if (URL == "https://www.instagram.com/?next=%2F")
                {
                    string hashTag = GetRandomHashtag(hashtagsArray);
                    if (e.Browser.MainFrame.Document.GetElementsByTagName("button").Where(s => s.InnerText.Trim() == "Not Now").FirstOrDefault() != null)
                    {
                        e.Browser.MainFrame.Document.GetElementsByTagName("button").Where(s => s.InnerText.Trim() == "Not Now").FirstOrDefault().Click();
                    }
                    Thread.Sleep(5000);
                    Application.DoEvents();
                    IElement element = e.Browser.MainFrame.Document.GetElementByClassName("x1n2onr6");
                    if (element != null)
                    {
                        IElement spanElement = element.GetElementsByTagName("a").FirstOrDefault(x => x.InnerText == "Search");
                        if (spanElement != null)
                        {
                            // Perform desired operations with the selected element
                            //spanElement.Click();
                            IElement parentDivElement = spanElement.Parent.GetElementByTagName("div");
                            parentDivElement.Click();
                        }
                    }
                    Thread.Sleep(2000);
                    WaitForElementToLoad(e, CurrentStep, x => (e.Browser.MainFrame.Document.GetElementByClassName("x9f619") != null));
                    IInputElement inputElement = (IInputElement)e.Browser.MainFrame.Document.GetElementsByTagName("input").Where(x => x.Attributes["placeholder"].Contains("Search")).FirstOrDefault();
                    if (inputElement != null)
                    {
                        //inputElement.Value = "Love";
                        EnterValue((IInputElement)e.Browser.MainFrame.Document.GetElementsByTagName("input").Where(x => x.Attributes["placeholder"].Contains("Search")).FirstOrDefault(), hashTag, e);
                        Thread.Sleep(500);


                    }
                    IElement firstTag;
                    Thread.Sleep(5000);
                    firstTag = e.Browser.MainFrame.Document.GetElementsByTagName("div").Where(x => x.Attributes["role"].Contains("none")).FirstOrDefault().GetElementsByTagName("a").FirstOrDefault().GetElementsByTagName("span").Where(a => (a.InnerText.Trim().ToLower() == hashTag.Trim().ToLower())).FirstOrDefault();

                    if (firstTag != null)
                    {
                        firstTag.Click();
                    }
                    Thread.Sleep(5000);
                    CurrentStep = StepCompleted.Step_ClickedHashTag;
                    //this code will work to
                    //foreach (IElement oDiv in e.Browser.MainFrame.Document.GetElementsByTagName("div").Where(x => x.Attributes["role"].Contains("none")))
                    //{
                    //    foreach (IElement oA in oDiv.GetElementsByTagName("a"))
                    //    {
                    //        foreach (IElement oSpan in oA.GetElementsByTagName("span"))
                    //        {
                    //            if (oSpan.InnerText.ToLower() == hashTags.ToLower())
                    //            {
                    //                oSpan.Click();
                    //            }
                    //        }
                    //    }
                    //}
                }
                //if (URL == $"https://www.instagram.com/explore/tags/{hashTags.Replace("#", "")}/" && CurrentStep == StepCompleted.Step_ClickedHashTag)
                if (URL == "https://www.instagram.com/?next=%2F" && CurrentStep == StepCompleted.Step_ClickedHashTag)
                {
                    Thread.Sleep(500);
                    e.Browser.MainFrame.Document.GetElementsByClassName("_aabd _aa8k  _al3l").FirstOrDefault().GetElementByTagName("a").Click();
                    Thread.Sleep(2000);
                    IElement _likebutton = (IElement)e.Browser.MainFrame.Document.GetElementsByClassName("_aamu _ae3_ _ae47 _ae48").FirstOrDefault().GetElementsByTagName("span").FirstOrDefault().GetElementsByTagName("div").Where(x => x.Attributes["role"].Contains("button")).FirstOrDefault();
                    if (_likebutton != null)
                    {
                        _likebutton.Click();
                    }                    
                    IElement nextButton = e.Browser.MainFrame.Document.GetElementByClassName("_abl-");

                    while (nextButton != null)
                    {
                        
                        nextButton.Click();
                        Thread.Sleep(3000);                        
                        WaitForElementToLoad(e, CurrentStep, x => (e.Browser.MainFrame.Document.GetElementByClassName("_ae65") != null));
                        IElement likebutton =  (IElement)e.Browser.MainFrame.Document.GetElementsByClassName("_aamu _ae3_ _ae47 _ae48").FirstOrDefault().GetElementsByTagName("span").FirstOrDefault().GetElementsByTagName("div").Where(x => x.Attributes["role"].Contains("button")).FirstOrDefault();
                        if(likebutton!= null)
                        {
                            likebutton.Click();
                        }
                        Thread.Sleep(2000);
                    }




                }
                //if (URL == "https://www.instagram.com/")
                //{
                //    if (e.Browser.MainFrame.Document.GetElementsByTagName("button").Where(s => s.InnerText.Trim() == "Not Now").FirstOrDefault() != null)
                //    {
                //        e.Browser.MainFrame.Document.GetElementsByTagName("button").Where(s => s.InnerText.Trim() == "Not Now").FirstOrDefault().Click();
                //    }
                //    Thread.Sleep(500);

                //    // string script = @"
                //    //var elements = document.querySelectorAll('span');
                //    //var targetElement;

                //    //elements.forEach(function(element) {
                //    //  if (element.innerText === 'Search') {
                //    //    targetElement = element;
                //    //    return;
                //    //  }
                //    //});

                //    //if (targetElement) {
                //    //  // Get the parent div or navigation link
                //    //  var parentElement = targetElement.closest('div.x9f619') || targetElement.closest('a._a6hd');

                //    //  if (parentElement) {
                //    //    // You have selected the desired element, do whatever you want with it
                //    //    console.log(parentElement);
                //    //    // Trigger the click event on the parent element
                //    //    targetElement.click();
                //    //  }
                //    //}";
                //    //This line of code is working fine to
                //    // string javascriptCode = @"const xpath = '//div[contains(@class, ""x9f619"")]//span[contains(text(), ""Search"")]'; const element = document.evaluate(xpath, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue; if (element) { var event = document.createEvent(""MouseEvent""); event.initEvent(""click"", true, true); element.dispatchEvent(event); }";
                //    string javascriptCode = @" const xpath = '//*[@aria-label=""Search""]'; const result = document.evaluate(xpath, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null); const element = result.singleNodeValue; if (element) {  var event = document.createEvent(""MouseEvent"");  event.initEvent(""click"", true, true); element.dispatchEvent(event);}";
                //    e.Browser.MainFrame.ExecuteJavaScript(javascriptCode);

                //}






            }




        }

        private void Navigation_FrameLoadFinished(object sender, FrameLoadFinishedEventArgs e)
        {

          
            try
            {
                string URL = e.ValidatedUrl;
                if ((URL == Url) && (_isLoggedIn == false))
                {
                    Thread.Sleep(6000);
                    Application.DoEvents();
                    Login(e.Browser.MainFrame.Document, e);
                    return;
                }
            }
            catch (Exception ex)
            {
               
            }
        }
        private void Login(IDocument document, FrameLoadFinishedEventArgs e)
        {
            try
            {
                string filePath = @"C:\Users\HP\Desktop\Password.txt";
                string user_name = "adnanakhtar5069";
                string password = "";
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string content = sr.ReadToEnd(); // Read the entire file content

                    // Assign the content to the Password element
                    password = content;
                }
               
                //ChainInputEventToElement(document.GetElementByName("username"), user_name, "change");
                //ChainInputEventToElement(document.GetElementByName("password"), password, "change");

                EnterValue((IInputElement)e.Browser.MainFrame.Document.GetElementByName("username"), user_name, e);
                Thread.Sleep(500);

                EnterValue((IInputElement)e.Browser.MainFrame.Document.GetElementByName("password"), password, e);
               
                Thread.Sleep(500);



                if (e.Browser.MainFrame.Document.GetElementsByTagName("button").Where(x => x.TextContent.Contains("Log in")).FirstOrDefault() != null)
                {
                    e.Browser.MainFrame.Document.GetElementsByTagName("button").Where(x => x.TextContent.Contains("Log in")).FirstOrDefault().Click();
                }
                //CurrentStep = StepCompleted.Step_LoggedIn;
                _isLoggedIn = true;
                //SharedFunction.StartWebBotActivity((DatasetManager.ActiveDataSetMgr)DataSetManager);
                //asa
            }
            catch (Exception ex)
            {
               
            }
        }
        private void ChainInputEventToElement(IElement element, string value, string myevent = "input")
        {
            try
            {
                BindEventToElement("focus", element);
                if (value != null)
                {
                    ((IInputElement)element).Value = value;
                    Thread.Sleep(500);
                }
                BindEventToElement(myevent, element);
                BindEventToElement("blur", element);
            }
            catch (Exception ex)
            {
            }
        }
        private void BindEventToElement(string eventName, IElement element)
        {
            try
            {
                EventType eventType = new EventType(eventName);
                var myEvent = browser.MainFrame.Document.CreateEvent(eventType, new EventParameters.Builder().Build());
                element.DispatchEvent(myEvent);
            }
            catch (Exception ex)
            {
                
            }
        }
        private void EnterValue(IInputElement oElement, string value, FrameLoadFinishedEventArgs e)
        {
            try
            {
                Thread.Sleep(500);

                oElement.Focus();
                KeyModifiers modifiers = null;
                IElement focusedElement = e.Browser.MainFrame.Document.FocusedElement;


                oElement.Focus();

               
                IKeyboard keyboard = e.Browser.Keyboard;
                KeyCode key;
                string keyChar;

                for (int iChar = 0, loopTo = value.Length - 1; iChar <= loopTo; iChar++)
                {
                 
                    key = (KeyCode)System.Convert.ToInt32(value[iChar]); 
                    keyChar = value[iChar].ToString();

                    if (IsSpecialCharacter(Convert.ToChar(keyChar)) && keyChar =="#") 
                    { 
                       SimulateKey(keyboard, KeyCode.Oem3, keyChar, new KeyModifiers { ShiftDown = true });
                    } 
                    else if (IsSpecialCharacter(Convert.ToChar(keyChar)) && keyChar =="$") 
                    { 
                       SimulateKey(keyboard, KeyCode.Oem4, keyChar, new KeyModifiers { ShiftDown = true });
                    }
                    else
                    {
                        SimulateKey(keyboard, key, keyChar);
                    }


                    System.Threading.Thread.Sleep(50);
                }

              
            }
            catch (Exception ex)
            {
                //oLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        private bool IsSpecialCharacter(Char keyChar)
        {
            return Char.IsSymbol(keyChar) || Char.IsPunctuation(keyChar);
        }
        private static void SimulateKey(IKeyboard keyboard, KeyCode key, string keyChar,
                                       KeyModifiers modifiers = null)
        {
            modifiers = modifiers ?? new KeyModifiers();
            KeyPressedEventArgs keyDownEventArgs = new KeyPressedEventArgs
            {
                KeyChar = keyChar,
                VirtualKey = key,
                Modifiers = modifiers
            };

            KeyTypedEventArgs keyPressEventArgs = new KeyTypedEventArgs
            {
                KeyChar = keyChar,
                VirtualKey = key,
                Modifiers = modifiers
            };
            KeyReleasedEventArgs keyUpEventArgs = new KeyReleasedEventArgs
            {
                VirtualKey = key,
                Modifiers = modifiers
            };

            keyboard.KeyPressed.Raise(keyDownEventArgs);
            keyboard.KeyTyped.Raise(keyPressEventArgs);
            keyboard.KeyReleased.Raise(keyUpEventArgs);
        }
        //private void EnterValue(IInputElement oElement, string value, FrameLoadFinishedEventArgs e)
        //{
        //    try
        //    {
        //        Thread.Sleep(500);

        //        oElement.Focus();

        //        IElement focusedElement = e.Browser.MainFrame.Document.FocusedElement;


        //        oElement.Focus();


        //        IKeyboard keyboard = e.Browser.Keyboard;
        //        KeyCode key;
        //        string keyChar;

        //        KeyPressedEventArgs keyDownEventArgs;
        //        KeyTypedEventArgs keyPressEventArgs;
        //        KeyReleasedEventArgs keyUpEventArgs;

        //        for (int iChar = 0, loopTo = value.Length - 1; iChar <= loopTo; iChar++)
        //        {

        //            //key = (KeyCode)System.Convert.ToInt32(value[iChar]);
        //            key = (KeyCode)(value[iChar]);
        //            keyChar = value[iChar].ToString();

        //            //DebugWriteLine("Sending..." + keyChar);

        //            keyDownEventArgs = new KeyPressedEventArgs
        //            {
        //                KeyChar = keyChar,
        //                VirtualKey = key
        //            };

        //            keyPressEventArgs = new KeyTypedEventArgs
        //            {
        //                KeyChar = keyChar,
        //                VirtualKey = key
        //            };
        //            keyUpEventArgs = new KeyReleasedEventArgs
        //            {
        //                VirtualKey = key
        //            };


        //            keyboard.KeyPressed.Raise(keyDownEventArgs);
        //            keyboard.KeyTyped.Raise(keyPressEventArgs);
        //            keyboard.KeyReleased.Raise(keyUpEventArgs);

        //            System.Threading.Thread.Sleep(50);
        //        }

        //        key = KeyCode.Tab;
        //        keyChar = "";

        //        keyDownEventArgs = new KeyPressedEventArgs
        //        {
        //            KeyChar = keyChar,
        //            VirtualKey = key
        //        };

        //        keyPressEventArgs = new KeyTypedEventArgs
        //        {
        //            KeyChar = keyChar,
        //            VirtualKey = key
        //        };
        //        keyUpEventArgs = new KeyReleasedEventArgs
        //        {
        //            VirtualKey = key
        //        };

        //        keyboard.KeyPressed.Raise(keyDownEventArgs);
        //        keyboard.KeyTyped.Raise(keyPressEventArgs);
        //        keyboard.KeyReleased.Raise(keyUpEventArgs);

        //    }
        //    catch (Exception ex)
        //    {
        //        //oLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
        //    }
        //}
        private bool WaitForElementToLoad(FrameLoadFinishedEventArgs e, StepCompleted currentStep, IsElementLoadedOnDOM isElementLoadedOnDOM)
        {

            int iTick = 0;
            try
            {
                while (CurrentStep == currentStep)
                {
                    //if (MainControlDisposing) { return false; };
                    if (isElementLoadedOnDOM(e.Browser.MainFrame.Document))
                    {
                        CurrentStep = StepCompleted.Step_ElementFound;
                        return true;
                    }
                    iTick++;
                    Thread.Sleep(500);

                    if (iTick > 20)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //oLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            return false;
        }

    }
}
