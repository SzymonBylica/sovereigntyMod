using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Threading;
using SovereigntyTK.UI.Controls;
using SovereigntyTK.UI.Text;
using SovereigntyTK.Game.Data;
using System.Linq;
using SovereigntyTK.Game;
using SovereigntyTK.Utility;
using SovereigntyTK.Game.Campaign;
using SovereigntyTK.Game.ActiveGameData;

namespace SovereigntyTK.UI
{
    public class TickerPanel: ScriptBase
    {
        List<GameText> TickerTexts;
        List<GameText> PendingTexts;
        ControlText Ticker;
        GameText CurrentTopText;
        float Timer;

        public bool IsOpen;

        public void FormLoaded(UIControl Control)
        {
            GameSystem.RegisterEvent(HandleCloseRequest, "CloseTicker");
            GameSystem.RegisterEvent(GameSystem_OnScriptEvent, "TickerMessage");
            GameSystem.OnViewportChanged += GameSystem_OnViewportChanged;

            Ticker = GameForm.GetControlByName<ControlText>("TickerText");
            Ticker.ScrollingEnabled = true;

            TickerTexts = new List<GameText>();
            PendingTexts = new List<GameText>();

            foreach (GameText Text in GameSystem.CurrentGame.TickerTexts)
            {
                TickerTexts.Add(Text);
            }

            CloseTicker();
        }

        private void HandleCloseRequest(string EventName, params object[] Args)
        {
            if (IsOpen) CloseTicker();
        }

        void GameSystem_OnViewportChanged()
        {
            GameForm.SetBounds(0, 30, 400, 30, UIUnits.Pixel);
            if (IsOpen) OpenTicker();
            else CloseTicker();
        }

        void GameSystem_OnScriptEvent(string EventName, params object[] Args)
        {
            if (EventName == "TickerMessage")
            {
                TickerMessage Msg = (TickerMessage)Args[0];

                if (TickerTexts.Count == 50) TickerTexts.RemoveAt(0);

                GameText Text = GameText.CreateLocalised("FORMAT_TICKER");
                Text.AddChildText(Msg.MessageText);

                TickerTexts.Add(Text);
                PendingTexts.Add(Text);

                if (IsOpen) OpenTicker();
                else CloseTicker();
            }
        }

        public override void Dispose()
        {
            GameSystem.UnregisterEvent(HandleCloseRequest, "CloseTicker");
            GameSystem.UnregisterEvent(GameSystem_OnScriptEvent, "TickerMessage");
            GameSystem.OnViewportChanged -= GameSystem_OnViewportChanged;
        }

        public void TickerToggleClick(UIControl Sender)
        {
            if (IsOpen) CloseTicker();
            else OpenTicker();

            string Img1 = "";
            string Img2 = "";
            string Img3 = "";

            if (IsOpen)
            {
                Img1 = "Data\\Images\\HUD\\Economy\\arrow_up_normal.png";
                Img2 = "Data\\Images\\HUD\\Economy\\arrow_up_mouseover.png";
                Img3 = "Data\\Images\\HUD\\Economy\\arrow_up_pressed.png";
            }
            else
            {
                Img1 = "Data\\Images\\HUD\\Economy\\arrow_down_normal.png";
                Img2 = "Data\\Images\\HUD\\Economy\\arrow_down_mouseover.png";
                Img3 = "Data\\Images\\HUD\\Economy\\arrow_down_pressed.png";
            }

            GameForm.GetControlByName<ControlButton>("ToggleButton").SetImageFiles(Img1, Img2, Img3);
        }

        public override void Update(float ElapsedTime)
        {
            if (!IsOpen)
            {
                if (PendingTexts.Count > 0)
                {
                    Timer += ElapsedTime;
                    if (Timer > 0.25f)
                    {
                        Timer -= 0.25f;
                        Ticker.Visible = true;
                        Ticker.SetTextData(PendingTexts[0]);
                        PendingTexts.RemoveAt(0);
                    }
                }
            }
        }


        public void OpenTicker()
        {
            PendingTexts.Clear();
            CurrentTopText = null;

            if (TickerTexts.Count == 0) Ticker.Visible = false;
            else
            {
                Ticker.Visible = true;
                GameForm.SetSize(400, 300, UIUnits.Pixel);
                Ticker.SetTextData(TickerTexts);
                Ticker.ScrollToBottom();
                IsOpen = true;
            }
        }

        public void CloseTicker()
        {
            if (TickerTexts.Count == 0) Ticker.Visible = false;
            else if (IsOpen)
            {
                Ticker.Visible = true;
                GameForm.SetSize(400, 30, UIUnits.Pixel);
                Ticker.SetTextData(TickerTexts[TickerTexts.Count - 1]);
                Ticker.ResetScroll();
                IsOpen = false;
                Update(0.25f);
            }
        }
    }
}
