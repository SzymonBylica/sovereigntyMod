using SovereigntyTK.Game.ActiveGameData;
using SovereigntyTK.UI.Controls;
using SovereigntyTK.UI.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SovereigntyTK.UI
{
    public class ControlProvinceCard: UIControl
    {
        ControlImage CardBGImage;
        private WorkingProvince CurrentProvince;

        ControlImage FlagImage;
        ControlImage ProvinceImage;

        ControlText NameText;
        ControlText DescriptionText;

        List<ControlImage> SpellImages;
        ControlImage PlagueIcon;

        ControlImage EconomyBG;
        ControlText EconomyText2;
        ControlText RevoltText;

        public ControlProvinceCard(GameBase Game)
            :base(Game)
        {
            CreateControls();
        }

        private void CreateControls()
        {
            CardBGImage = new ControlImage(Game);
            CardBGImage.MouseInputType = MouseInputTypes.None;
            AddChild(CardBGImage);

            FlagImage = new ControlImage(Game);
            AddChild(FlagImage);

            ProvinceImage = new ControlImage(Game);
            AddChild(ProvinceImage);

            NameText = new ControlText(Game);
            AddChild(NameText);
            NameText.FontName = "Trebuchet MS";
            NameText.Colour = Color.FromArgb(0, 0, 0);
            NameText.TextAnchor = AnchorPoints.Middle;

            EconomyBG = new ControlImage(Game);
            AddChild(EconomyBG);

            EconomyText2 = new ControlText(Game);
            EconomyBG.AddChild(EconomyText2);
            EconomyText2.FontName = "Trebuchet MS";
            EconomyText2.Colour = Color.FromArgb(72, 30, 27);
            EconomyText2.TextAnchor = AnchorPoints.Middle;
            EconomyText2.MouseInputType = MouseInputTypes.Forced;

            RevoltText = new ControlText(Game);
            AddChild(RevoltText);
            RevoltText.FontName = "Trebuchet MS";
            RevoltText.Colour = Color.FromArgb(0, 0, 0);
            RevoltText.TextAnchor = AnchorPoints.TopLeft;
            RevoltText.ScrollingEnabled = false;
            RevoltText.MouseInputType = MouseInputTypes.Forced;

            DescriptionText = new ControlText(Game);
            AddChild(DescriptionText);
            DescriptionText.FontName = "Trebuchet MS";
            DescriptionText.Colour = Color.FromArgb(0, 0, 0);
            DescriptionText.TextAnchor = AnchorPoints.TopLeft;
            DescriptionText.ScrollingEnabled = true;
            DescriptionText.AcceptMouseWheel = true;
            DescriptionText.MouseInputType = MouseInputTypes.Forced;

            foreach (UIControl Control in GetChildren())
            {
                if (Control == RevoltText) continue;
                Control.BubbleMouseovers = true;
            }

            SpellImages = new List<ControlImage>();
            for (int i = 0; i < 6; i++)
            {
                ControlImage SpellImage = new ControlImage(Game);
                SpellImages.Add(SpellImage);
                AddChild(SpellImage);
            }

            PlagueIcon = new ControlImage(Game);
            AddChild(PlagueIcon);
            PlagueIcon.SetImageFile("Data\\Images\\HUD\\Info\\plague_icon.png");
            PlagueIcon.SetTooltip(GameText.CreateLocalised("TTPLAGUE"));
        }

        private void SetControlPositions()
        {
            CardBGImage.SetBounds(0, 0, Sprite.Bounds.Width, Sprite.Bounds.Height, UIUnits.PixelScaled);

            FlagImage.SetBounds(Sprite.Bounds.Width * 0.751f, Sprite.Bounds.Height * 0.056f, Sprite.Bounds.Width * 0.143f, Sprite.Bounds.Height * 0.074f, UIUnits.PixelScaled);
            ProvinceImage.SetBounds(Sprite.Bounds.Width * 0.143f, Sprite.Bounds.Height * 0.153f, Sprite.Bounds.Width * 0.713f, Sprite.Bounds.Height * 0.282f, UIUnits.PixelScaled);

            NameText.SetBounds(Sprite.Bounds.Width * 0.102f, Sprite.Bounds.Height * 0.054f, Sprite.Bounds.Width * 0.645f, Sprite.Bounds.Height * 0.077f, UIUnits.PixelScaled);
            NameText.SetFontSize((int)(NameText.Sprite.Bounds.Height * 0.5f), UIUnits.PixelScaled);

            RevoltText.SetBounds(Sprite.Bounds.Width * 0.115f, Sprite.Bounds.Height * 0.515f, Sprite.Bounds.Width * 0.792f, Sprite.Bounds.Height * 0.050f, UIUnits.PixelScaled);

            DescriptionText.SetBounds(Sprite.Bounds.Width * 0.115f, Sprite.Bounds.Height * 0.565f, Sprite.Bounds.Width * 0.792f, Sprite.Bounds.Height * 0.383f, UIUnits.PixelScaled);
            DescriptionText.SetFontSize((int)(DescriptionText.Sprite.Bounds.Height * 0.075f), UIUnits.PixelScaled);
            RevoltText.SetFontSize((int)(DescriptionText.Sprite.Bounds.Height * 0.075f), UIUnits.PixelScaled);

            EconomyBG.SetBounds(Sprite.Bounds.Width * 0.05f, Sprite.Bounds.Width * 0.05f, Sprite.Bounds.Height * 0.060f, Sprite.Bounds.Height * 0.060f, UIUnits.PixelScaled);

            EconomyText2.SetBounds(0, 0, Sprite.Bounds.Height * 0.060f, Sprite.Bounds.Height * 0.060f, UIUnits.PixelScaled);
            EconomyText2.SetFontSize((int)(EconomyText2.Sprite.Bounds.Height * 0.75f), UIUnits.PixelScaled);

            for (int i = 0; i < 6; i++)
            {
                float XDist = 0.118f;
                float XPos = 0.143f + (i * XDist);

                SpellImages[i].SetBounds(Sprite.Bounds.Width * XPos, Sprite.Bounds.Height * 0.153f, Sprite.Bounds.Width * 0.118f, Sprite.Bounds.Height * 0.072f, UIUnits.PixelScaled);
            }

            PlagueIcon.SetBounds(Sprite.Bounds.Width * 0.143f, Sprite.Bounds.Height * 0.363f, Sprite.Bounds.Width * 0.118f, Sprite.Bounds.Height * 0.072f, UIUnits.PixelScaled);
        }

        public void SetProvince(WorkingProvince Province)
        {
            if (CurrentProvince != null) CurrentProvince.SpellEffects.SpellsChanged -= SpellEffects_SpellsChanged;
            Province.SpellEffects.SpellsChanged += SpellEffects_SpellsChanged;

            this.CurrentProvince = Province;

            switch (Province.OwnerRealm.Panel)
            {
                case SovereigntyTK.Game.Data.SpellSchools.Death:
                    {
                        CardBGImage.SetImageFile("Data\\Images\\HUD\\Info\\provincecard_death.png");
                        EconomyBG.SetImageFile("Data\\Images\\HUD\\Info\\economyvalue_death.png");
                        break;
                    }
                case SovereigntyTK.Game.Data.SpellSchools.Illusion:
                    {
                        CardBGImage.SetImageFile("Data\\Images\\HUD\\Info\\provincecard_illusion.png");
                        EconomyBG.SetImageFile("Data\\Images\\HUD\\Info\\economyvalue_illusion.png");
                        break;
                    }
                case SovereigntyTK.Game.Data.SpellSchools.Nature:
                    {
                        CardBGImage.SetImageFile("Data\\Images\\HUD\\Info\\provincecard_nature.png");
                        EconomyBG.SetImageFile("Data\\Images\\HUD\\Info\\economyvalue_nature.png");
                        break;
                    }
                case SovereigntyTK.Game.Data.SpellSchools.War:
                    {
                        CardBGImage.SetImageFile("Data\\Images\\HUD\\Info\\provincecard_war.png");
                        EconomyBG.SetImageFile("Data\\Images\\HUD\\Info\\economyvalue_war.png");
                        break;
                    }
            }

            FlagImage.SetImageFile("Data\\Images\\HUD\\flags\\" + Province.OwnerRealm.FlagFilename + ".png");
            ProvinceImage.SetImageFile("Data\\Images\\HUD\\landmarks\\" + Province.ImageFile + ".png");

            NameText.SetTextData(GameText.CreateLocalised(Province.DisplayName));

            string IconName = Province.CurrentEconomy + ".png";
            
            EconomyText2.Visible = true;

            EconomyText2.SetTextData(GameText.CreateLocalised("FORMAT_NUMBER", Province.CurrentEconomy));
            int Income = Province.Game.EconomyController.GetProvinceIncome(Province, Province.OwnerRealm.CapitolProvince, 1f);

            GameText Text = GameText.CreateLocalised("TT_BUILDING_ECON");
            Text.AddChildText(GameText.CreateLocalised("FORMAT_PROVINCEINCOME", Income, Province.BaseIncome.GetValue()));

            EconomyText2.SetTooltip(Text);

            RevoltText.SetTextData(GameText.CreateLocalised("REVOLT_CHANCE", Province.RevoltChance.GetValue()));
            RevoltText.SetTooltip(Province.RevoltChance.GetBreakdown());

            List<GameText> Description = new List<GameText>();
            Description.Add(GameText.CreateLocalised("FORMAT_NEWLINE"));
            GameText TerrainText = (GameText.CreateLocalised("TT_UNITDEPLOY"));
            TerrainText.AddChildText(GameText.CreateLocalised(Province.Terrain.DisplayName));
            Description.Add(TerrainText);
            Description.Add(GameText.CreateLocalised("FORMAT_NEWLINE"));
            Description.Add(GameText.CreateLocalised("FORMAT_PROVINCEINCOME", Income, Province.BaseIncome.GetValue()));
            Description.Add(GameText.CreateLocalised("FORMAT_NEWLINE"));
            Description.Add(GameText.CreateLocalised("FORMAT_NEWLINE"));

            if (Province.Description != "" && Province.Description != "0") Description.Add(GameText.CreateLocalised(Province.Description));

            DescriptionText.SetTextData(Description);

            PlagueIcon.Visible = Province.PlagueTurns > 0;

            UpdateSpells();
        }

        void SpellEffects_SpellsChanged()
        {
            UpdateSpells();
        }

        public void UpdateSpells()
        {
            for (int i = 0; i < 6; i++)
            {
                if (i >= CurrentProvince.SpellEffects.ActiveSpells.Count) SpellImages[i].Visible = false;
                else
                {
                    SpellEffect Spell = CurrentProvince.SpellEffects.ActiveSpells[i];

                    SpellImages[i].Visible = true;
                    SpellImages[i].SetImageFile("Data\\Images\\HUD\\spell\\" + Spell.SpellData.SchoolName + "\\" + Spell.SpellData.ImageFileName + ".png");
                    SpellImages[i].SetTooltip(GetSpellEffectTooltip(Spell));
                }
            }
        }

        public GameText GetSpellEffectTooltip(SpellEffect Effect)
        {
            GameText Text = GameText.CreateLocalised("TT_ACTIVESPELL");
            Text.AddChildText(GameText.CreateLocalised(Effect.SpellData.DisplayName));
            Text.AddChildText(GameText.CreateLocalised(Effect.SpellData.DisplayText));

            if (Effect.Timer >= 0) Text.AddChildText(GameText.CreateLocalised("FORMAT_NUMBER", Effect.Timer));
            else Text.AddChildText(GameText.CreateLocalised("SPELL_PERM"));

            Text.AddChildText(GameText.CreateLocalised(Effect.Caster.DisplayName));

            return Text;
        }

        protected override void ReclaculateBounds()
        {
            RectangleF OldBounds = Sprite.Bounds;

            base.ReclaculateBounds();

            if (OldBounds.Size == Sprite.Bounds.Size) return;
            if (CardBGImage == null) return;

            //if the size of the control has changed
            //the internal controls need to have their positions/sizes updated to reflect this
            SetControlPositions();
        }
    }
}
