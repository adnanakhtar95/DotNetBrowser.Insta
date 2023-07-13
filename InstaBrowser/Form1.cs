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

namespace InstaBrowser
{
    public partial class Form1 : Form
    {
        private const string Url = "https://www.instagram.com/accounts/login/";
        private  IBrowser browser;
        private  IEngine engine;
        private bool _isLoggedIn;
        public Form1()
        {
            

            InitializeComponent();
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
            browser.Navigation.LoadUrl(Url);
            browser.Navigation.FrameLoadFinished += Navigation_FrameLoadFinished;
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
    }
}
