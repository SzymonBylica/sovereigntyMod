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
        const int MaxEntries = 50;
        const int WrapChars = 46;
        const string WrapIndent = "   ";

        static readonly Color NormalColour = Color.FromArgb(255, 238, 161);
        static readonly Color BattleColour = Color.FromArgb(255, 255, 255);

        class TickerEntry
        {
            public GameText Text;
            public bool IsBattle;
        }

        class TickerRow
        {
            public string Text;
            public Color Colour;
        }

        // Ticker messages also arrive from AI turn threads, so the handler only queues entries;
        // controls are touched from Update/UI callbacks on the render thread.
        readonly object EntryLock = new object();
        List<TickerEntry> TickerTexts;
        List<TickerEntry> PendingTexts;
        ControlText Ticker;
        ControlList<TickerRow> LogList;
        bool LogDirty;
        float Timer;

        public bool IsOpen;

        public void FormLoaded(UIControl Control)
        {
            GameSystem.RegisterEvent(HandleCloseRequest, "CloseTicker");
            GameSystem.RegisterEvent(GameSystem_OnScriptEvent, "TickerMessage");
            GameSystem.OnViewportChanged += GameSystem_OnViewportChanged;
            GameSystem.CurrentGame.OnPlayerTurnStarted += HandlePlayerTurnStarted;

            Ticker = GameForm.GetControlByName<ControlText>("TickerText");

            // One row per line so each message can have its own colour (the text renderer has no colour tags).
            LogList = new ControlList<TickerRow>(GameSystem);
            GameForm.AddChild(LogList);
            LogList.ManualUpdate = true;
            LogList.HighlightEnabled = false;
            LogList.SelectionEnabled = false;
            LogList.OnListItemUpdated += LogList_OnListItemUpdated;
            LogList.SetVisibleItems(18);
            LogList.SetBounds(8, 4, 360, 292, UIUnits.Pixel);
            LogList.Visible = false;

            TickerTexts = new List<TickerEntry>();
            PendingTexts = new List<TickerEntry>();

            foreach (GameText Text in GameSystem.CurrentGame.TickerTexts)
            {
                TickerTexts.Add(new TickerEntry { Text = Text, IsBattle = IsBattleText(Text) });
            }

            ShowCollapsed();
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
            if (EventName != "TickerMessage") return;

            TickerMessage Msg = (TickerMessage)Args[0];

            // Suppress AI-vs-AI treaty chatter (NAP / defence / alliance spam).
            if (IsForeignTreatyMessage(Msg)) return;

            GameText Text = GameText.CreateLocalised("FORMAT_TICKER");
            Text.AddChildText(Msg.MessageText);
            TickerEntry Entry = new TickerEntry { Text = Text, IsBattle = IsBattleText(Msg.MessageText) };

            lock (EntryLock)
            {
                if (TickerTexts.Count == MaxEntries) TickerTexts.RemoveAt(0);
                TickerTexts.Add(Entry);

                if (IsOpen) LogDirty = true;
                else PendingTexts.Add(Entry);
            }
        }

        // Battle reports ("X attacks Y", "X defeats Y") are hardcoded English literals in the engine.
        private bool IsBattleText(GameText Text)
        {
            if (Text == null) return false;

            string Resolved = Text.GetActualText(GameSystem);
            return Resolved.Contains(" attacks ") || Resolved.Contains(" defeats ");
        }

        private bool IsForeignTreatyMessage(TickerMessage Msg)
        {
            if (Msg.MessageType != TickerMessageType.Diplomacy) return false;
            if (Msg.MessageText == null) return false;
            if (Msg.MessageText.TextName != "NEW_TREATY" && Msg.MessageText.TextName != "END_TREATY") return false;

            WorkingRealm Player = GameSystem.CurrentGame.PlayerRealm;
            if (Player == null) return true;

            string PlayerName = GameText.CreateLocalised(Player.DisplayName).GetActualText(GameSystem);
            string FullText = Msg.MessageText.GetActualText(GameSystem);
            return FullText.IndexOf(PlayerName, System.StringComparison.OrdinalIgnoreCase) < 0;
        }

        public override void Dispose()
        {
            GameSystem.UnregisterEvent(HandleCloseRequest, "CloseTicker");
            GameSystem.UnregisterEvent(GameSystem_OnScriptEvent, "TickerMessage");
            GameSystem.OnViewportChanged -= GameSystem_OnViewportChanged;
            if (GameSystem.CurrentGame != null)
            {
                GameSystem.CurrentGame.OnPlayerTurnStarted -= HandlePlayerTurnStarted;
            }
        }

        void HandlePlayerTurnStarted(WorkingRealm Realm)
        {
            if (Realm == null || Realm != GameSystem.CurrentGame.PlayerRealm) return;

            // Match TopPanel / save detail: displayed turn is TurnNumber + 1.
            int Turn = GameSystem.CurrentGame.TurnController.TurnNumber + 1;
            GameSystem.FireEvent("TickerMessage", new TickerMessage(
                GameText.CreateFromLiteral("------------------------"), TickerMessageType.Default, 1));
            GameSystem.FireEvent("TickerMessage", new TickerMessage(
                GameText.CreateLocalised("TICKER_TURN_START", Turn), TickerMessageType.Default, 1));
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
            if (Ticker == null) return;

            if (IsOpen)
            {
                if (LogDirty) RebuildLog();
                return;
            }

            TickerEntry Next = null;
            lock (EntryLock)
            {
                if (PendingTexts.Count > 0)
                {
                    Timer += ElapsedTime;
                    if (Timer > 0.25f)
                    {
                        Timer -= 0.25f;
                        Next = PendingTexts[0];
                        PendingTexts.RemoveAt(0);
                    }
                }
            }

            if (Next != null) ShowEntry(Next);
        }

        private void ShowEntry(TickerEntry Entry)
        {
            Ticker.Visible = true;
            Ticker.Colour = Entry.IsBattle ? BattleColour : NormalColour;
            Ticker.SetTextData(Entry.Text);
        }

        public void OpenTicker()
        {
            lock (EntryLock)
            {
                PendingTexts.Clear();
                if (TickerTexts.Count == 0)
                {
                    Ticker.Visible = false;
                    return;
                }
                IsOpen = true;
            }

            GameForm.SetSize(400, 300, UIUnits.Pixel);
            Ticker.Visible = false;
            LogList.Visible = true;
            RebuildLog();
        }

        public void CloseTicker()
        {
            if (IsOpen) ShowCollapsed();
        }

        // Also used on load, where IsOpen starts false: the old CloseTicker() then never set any text,
        // leaving the bar blank with its scrollbar parts drawn at their default (stretched) size.
        private void ShowCollapsed()
        {
            TickerEntry Last = null;
            lock (EntryLock)
            {
                IsOpen = false;
                if (TickerTexts.Count > 0) Last = TickerTexts[TickerTexts.Count - 1];
            }

            GameForm.SetSize(400, 30, UIUnits.Pixel);
            LogList.Visible = false;

            if (Last == null) Ticker.Visible = false;
            else ShowEntry(Last);
        }

        private void RebuildLog()
        {
            List<TickerEntry> Snapshot;
            lock (EntryLock)
            {
                LogDirty = false;
                Snapshot = TickerTexts.ToList();
            }

            LogList.ClearItems();
            foreach (TickerEntry Entry in Snapshot)
            {
                Color Colour = Entry.IsBattle ? BattleColour : NormalColour;
                foreach (string Line in WrapText(Entry.Text.GetActualText(GameSystem)))
                {
                    LogList.AddItem(new TickerRow { Text = Line, Colour = Colour });
                }
            }
            LogList.ScrollToBottom();
        }

        void LogList_OnListItemUpdated(ListItem<TickerRow> Item)
        {
            ControlText Line = new ControlText(GameSystem);
            Line.SetBounds(0, 0, Item.Container.Sprite.Bounds.Width, Item.Container.Sprite.Bounds.Height, UIUnits.PixelScaled);
            Line.SetFontSize((int)(Item.Container.Sprite.Bounds.Height * 0.75f), UIUnits.PixelScaled);
            Line.FontName = "Trebuchet MS";
            Line.Bold = true;
            Line.TextAnchor = AnchorPoints.Left;
            Line.Colour = Item.Data.Colour;
            Line.SetTextData(GameText.CreateFromLiteral(Item.Data.Text));
            Item.Container.AddChild(Line);
        }

        // Rows are one line tall, so long messages are pre-wrapped (approximate width of the 12px bold font).
        private static List<string> WrapText(string Text)
        {
            List<string> Lines = new List<string>();
            foreach (string Part in Text.Split('\n'))
            {
                string Remaining = Part.Trim();
                if (Remaining.Length == 0) continue;

                while (Remaining.Length > WrapChars)
                {
                    int Cut = Remaining.LastIndexOf(' ', WrapChars);
                    if (Cut <= WrapIndent.Length) Cut = WrapChars;

                    Lines.Add(Remaining.Substring(0, Cut).TrimEnd());
                    Remaining = WrapIndent + Remaining.Substring(Cut).TrimStart();
                }
                Lines.Add(Remaining);
            }
            return Lines;
        }
    }
}
