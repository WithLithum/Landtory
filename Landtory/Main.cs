﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GTA;
using NativeFunctionHook;
using Landtory.Engine.API.Common;
using Landtory.Engine.API.Handle;
using NativeFunctionHook.value;
using Landtory.Engine.Plugin;

namespace Landtory
{
    class Main : Script
    {
        bool SirenDriver;
        NArrowCheckpoint CheckArrow;
        Engine.API.Logger logger = new Engine.API.Logger();
        Blip stationBlip;
        string InfoDraw;
        public Main()
        {
            this.GUID = new Guid("2A0A940D-154B-4513-9FB7-7E12DBB4D8B8");
            this.BindKey(System.Windows.Forms.Keys.L, false, false, true, Load_Mod);
            this.PerFrameDrawing += Main_PerFrameDrawing;
            this.BindScriptCommand("DrawTargetText", Notified_Render);
        }

        private void Main_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.DrawText(InfoDraw, 200, 3, Color.Aqua);
        }

        void Load_Mod()
        {
            try
            {
                Vector3 vector = new Vector3(79.2884f, -713.946f, 4.95886f);
                logger.Log("Initilazing Checkpoint", "Main");
                CheckArrow = new NArrowCheckpoint(vector, new RGBColor(255, 215, 0));
                World.UnlockAllIslands();
                stationBlip = Blip.AddBlip(vector);
                stationBlip.Icon = BlipIcon.Building_PoliceStation;
                
                logger.Log("Initilazing Mod Functions", "Main");
                CheckArrow.CheckpointTriggered += new NArrowCheckpoint.CheckpointTriggeredHandler(Main_Tick);
                this.BindKey(System.Windows.Forms.Keys.End, SirenSwitchDriver);
                this.GUID = new Guid("2AA8D642-C91F-4688-A32C-7D27F588014A");
                InfoDraw = NLanguage.GetLangStr("OnDuty");
                Wait(3000);
                InfoDraw = NLanguage.GetLangStr("NameDraw");

                Controller.LoadAllPlugins();
                if(Controller.plugins.Count != 0)
                {
                    Controller.ExecuteAllPluginMethod("OnInitialized");
                }
            }
            catch (Exception ex)
            {
                logger.Log("Fatal Error During Initilazing Mod, Caused by: " + ex.Message + Environment.NewLine + ex.ToString(), Engine.API.Logger.LogLevel.Fatal);
                Game.DisplayText("Fatal Error of Landtory! Check Landtory.log for more info.");
            }
        }
        void Main_Tick(object sender, EventArgs e)
        {
            logger.Log("On Duty checkpoint arrived, sending signal", "Main");
            SendScriptCommand(new Guid("983110F2-7F23-4248-8B5C-315641351FEB"), "ON_DUTY");
            this.Interval = 0;
        }

        void Notified_Render(Script sender, ObjectCollection parameter)
        {
            InfoDraw = Programming.TransferInfo.Render;
            Wait(3000);
            InfoDraw = NLanguage.GetLangStr("NameDraw");
        }

        void SirenSwitchDriver()
        {
            if (Exists(Player.Character.CurrentVehicle) == false)
            {
                logger.Log("Siren without Driver switch failed: No Vehicle", "Main");
                NGame.PrintSubtitle(NLanguage.GetLangStr("SirenDriverNoVehicle"));
                return;
            }
            if (SirenDriver)
            {
                Player.Character.CurrentVehicle.AllowSirenWithoutDriver = false;
                SirenDriver = false;
                NGame.PrintSubtitle(NLanguage.GetLangStr("SirenWithoutDriverOff"));
                logger.Log("Siren without Driver OFF", "Main");
            }
            else
            {
                Player.Character.CurrentVehicle.AllowSirenWithoutDriver = true;
                SirenDriver = true;
                NGame.PrintSubtitle(NLanguage.GetLangStr("SirenWithoutDriverOn"));
                logger.Log("Siren without Driver ON", "Main");
            }
        }
    }
}
