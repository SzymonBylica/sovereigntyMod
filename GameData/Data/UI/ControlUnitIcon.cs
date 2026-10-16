using SovereigntyTK.Game.ActiveGameData;
using SovereigntyTK.Game.Data;
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
    public class ControlUnitIcon: UIControl
    {
        ControlImage UnitImage;


        ControlImage ClassImage;
        ControlImage HealthBG;
        ControlImage MoraleBG;
        ControlImage HealthFG;
        ControlImage MoraleFG;
        ControlImage AttritionImage;
        ControlText UnknownText;

        public WorkingUnit Unit;

        public ControlUnitIcon(GameBase Game)
            :base(Game)
        {
            CreateControls();
        }

        private void CreateControls()
        {
            UnitImage = new ControlImage(Game);
            UnitImage.MouseInputType = MouseInputTypes.None;
            AddChild(UnitImage);

            ClassImage = new ControlImage(Game);
            ClassImage.MouseInputType = MouseInputTypes.None;
            ClassImage.SetAnchor(AnchorPoints.TopLeft);
            AddChild(ClassImage);

            HealthBG = new ControlImage(Game);
            HealthBG.MouseInputType = MouseInputTypes.None;
            HealthBG.SetAnchor(AnchorPoints.BottomLeft);
            AddChild(HealthBG);

            MoraleBG = new ControlImage(Game);
            MoraleBG.MouseInputType = MouseInputTypes.None;
            MoraleBG.SetAnchor(AnchorPoints.BottomLeft);
            AddChild(MoraleBG);

            HealthFG = new ControlImage(Game);
            HealthFG.MouseInputType = MouseInputTypes.None;
            HealthFG.SetAnchor(AnchorPoints.BottomLeft);
            HealthFG.SetImageFile("Data\\Images\\Combat\\health_10.png");
            AddChild(HealthFG);

            MoraleFG = new ControlImage(Game);
            MoraleFG.MouseInputType = MouseInputTypes.None;
            MoraleFG.SetAnchor(AnchorPoints.BottomLeft);
            MoraleFG.SetImageFile("Data\\Images\\Combat\\morale_10.png");
            AddChild(MoraleFG);

            AttritionImage = new ControlImage(Game);
            AttritionImage.MouseInputType = MouseInputTypes.None;
            AttritionImage.SetAnchor(AnchorPoints.TopLeft);
            AttritionImage.SetImageFile("Data\\Images\\HUD\\Info\\attrition_skull.png");
            AttritionImage.Visible = false;
            AddChild(AttritionImage);

            UnknownText = new ControlText(Game);
            UnknownText.MouseInputType = MouseInputTypes.None;
            UnknownText.FontName = "Trebuchet MS";
            UnknownText.TextAnchor = AnchorPoints.Middle;
            UnknownText.Bold = true;
            UnknownText.Colour = Color.FromArgb(225, 225, 225);
            UnknownText.Visible = false;
            UnknownText.SetTextData(GameText.CreateFromLiteral("?"));
            AddChild(UnknownText);
        }

        private void SetControlPositions()
        {
            UnitImage.SetBounds(0, 0, Sprite.Bounds.Width, Sprite.Bounds.Height, UIUnits.PixelScaled);

            ClassImage.SetBounds(0, 0, Sprite.Bounds.Width * 0.25f, Sprite.Bounds.Height * 0.25f, UIUnits.PixelScaled);

            HealthBG.SetBounds(0, 0, Sprite.Bounds.Width, Sprite.Bounds.Height * 0.1f, UIUnits.PixelScaled);
            HealthFG.SetBounds(0, 0, Sprite.Bounds.Width, Sprite.Bounds.Height * 0.1f, UIUnits.PixelScaled);

            MoraleBG.SetBounds(0, Sprite.Bounds.Height * 0.1f, Sprite.Bounds.Width, Sprite.Bounds.Height * 0.1f, UIUnits.PixelScaled);
            MoraleFG.SetBounds(0, Sprite.Bounds.Height * 0.1f, Sprite.Bounds.Width, Sprite.Bounds.Height * 0.1f, UIUnits.PixelScaled);
            AttritionImage.SetBounds(Sprite.Bounds.Width * 0.72f, 0, Sprite.Bounds.Width * 0.28f, Sprite.Bounds.Height * 0.28f, UIUnits.PixelScaled);
            UnknownText.SetBounds(0, 0, Sprite.Bounds.Width, Sprite.Bounds.Height, UIUnits.PixelScaled);
            UnknownText.SetFontSize((int)(Sprite.Bounds.Height * 0.6f), UIUnits.PixelScaled);
        }

        public void SetUnknownUnit(string TooltipText = null)
        {
            this.Unit = null;

            UnitImage.Visible = false;
            ClassImage.Visible = false;
            HealthBG.Visible = false;
            HealthFG.Visible = false;
            MoraleBG.Visible = false;
            MoraleFG.Visible = false;
            AttritionImage.Visible = false;
            UnknownText.Visible = true;
            if (string.IsNullOrEmpty(TooltipText)) SetTooltip((GameText)null);
            else SetTooltip(GameText.CreateFromLiteral(TooltipText));
        }

        public void SetUnit(WorkingUnit Unit)
        {
            this.Unit = Unit;
            UnitImage.Visible = true;
            UnknownText.Visible = false;

            if (Unit is WorkingHero)
            {
                string HeroArt = "";

                if (Unit.Game.PlayerRealm.HeroTypeID == 1) HeroArt = (Unit as WorkingHero).HeroArt1;
                else HeroArt = (Unit as WorkingHero).HeroArt2;

                UnitImage.SetImageFile("Data\\Images\\units\\Hero\\" + HeroArt + ".jpg");

                ClassImage.Visible = false;
                HealthBG.Visible = false;
                HealthFG.Visible = false;
                MoraleBG.Visible = false;
                MoraleFG.Visible = false;
                AttritionImage.Visible = false;
                SetTooltip((GameText)null);
            }
            else
            {
                UnitImage.SetImageFile("Data\\Images\\Units\\" + Unit.ImageFile + ".jpg");

                ClassImage.Visible = true;
                HealthBG.Visible = true;
                HealthFG.Visible = true;
                MoraleBG.Visible = true;
                MoraleFG.Visible = true;

                switch (Unit.Class)
                {
                    case UnitClasses.Fort:
                        {
                            ClassImage.SetImageFile("Data\\Images\\HUD\\icons_siege.png");
                            break;
                        }
                    case UnitClasses.Archer:
                        {
                            ClassImage.SetImageFile("Data\\Images\\HUD\\icons_archer.png");
                            break;
                        }
                    case UnitClasses.Cavalry:
                        {
                            ClassImage.SetImageFile("Data\\Images\\HUD\\icons_cavalry.png");
                            break;
                        }
                    case UnitClasses.Infantry:
                        {
                            ClassImage.SetImageFile("Data\\Images\\HUD\\icons_infantry.png");
                            break;
                        }
                    case UnitClasses.Irregular:
                        {
                            ClassImage.SetImageFile("Data\\Images\\HUD\\icons_irregular.png");
                            break;
                        }
                    case UnitClasses.Siege:
                        {
                            ClassImage.SetImageFile("Data\\Images\\HUD\\icons_siege.png");
                            break;
                        }
                    case UnitClasses.Naval:
                        {
                            ClassImage.SetImageFile("Data\\Images\\HUD\\icons_naval.png");
                            break;
                        }
                }

                float HealthPct = (float)Unit.Health * 0.01f;
                float MoralePct = (float)Unit.Morale * 0.01f;

                HealthFG.SetWidth(Sprite.Bounds.Width * HealthPct, UIUnits.PixelScaled);
                MoraleFG.SetWidth(Sprite.Bounds.Width * MoralePct, UIUnits.PixelScaled);

                AttritionImage.Visible = Unit.HadAttritionThisTurn;
                if (Unit.HadAttritionThisTurn && string.IsNullOrEmpty(Unit.AttritionTooltipText) == false)
                {
                    SetTooltip(GameText.CreateFromLiteral(Unit.AttritionTooltipText));
                }
                else
                {
                    SetTooltip((GameText)null);
                }
            }

            if (Unit.Selected) UnitImage.Sprite.SetAlpha(1);
            else UnitImage.Sprite.SetAlpha(0.5f);
        }

        protected override void ReclaculateBounds()
        {
            RectangleF OldBounds = Sprite.Bounds;

            base.ReclaculateBounds();

            if (OldBounds.Size == Sprite.Bounds.Size) return;
            if (UnitImage == null) return;

            //if the size of the control has changed
            //the internal controls need to have their positions/sizes updated to reflect this
            SetControlPositions();
        }
    }
}
