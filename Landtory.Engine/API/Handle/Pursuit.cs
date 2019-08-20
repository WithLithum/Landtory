﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTA;
using AdvancedHookManaged;

namespace Landtory.Process
{
    class Pursuit
    {
        private Ped target;
        private APed Atarget;
        private Blip targetB;
        private List<Ped> officers;
        private List<APed> Aofficers;
        private Player Plyr = Game.LocalPlayer;
        private Timer time = new Timer();
        public Pursuit(Ped suspect)
        {
            target = suspect;
            Atarget = TypeConverter.ConvertToAPed(target);
            targetB = suspect.AttachBlip();
            targetB.Friendly = false;
            targetB.Name = "Suspect";
            if (target.isInVehicle())
            {
                target.Task.CruiseWithVehicle(target.CurrentVehicle, 50, false);
                target.Task.AlwaysKeepTask = true;
                target.SayAmbientSpeech("GENERIC_FUCK_OFF");
            }
            else
            {
                target.SetPathfinding(false, true, false);
                target.Task.FleeFromChar(Plyr.Character, true);
            }
            time.Tick += new EventHandler(time_Tick);
            time.Interval = 100;
        }

        void time_Tick(object sender, EventArgs e)
        {
            if (target.Exists() == false || target == null)
            {
                
            }
        }

        public void AttachBackup()
        {
            Ped officer = World.CreatePed(Model.BasicCopModel, Plyr.Character.Position.Around(30f));
            APed Aofficer = TypeConverter.ConvertToAPed(officer);
            Vehicle copcar = World.CreateVehicle(Model.BasicPoliceCarModel, Plyr.Character.Position.Around(29f));
            AVehicle Acopcar = TypeConverter.ConvertToAVehicle(copcar);
            Acopcar.AddCopBlip();
            Acopcar.SetAsPoliceVehicle(true);
            copcar.SirenActive = true;
            officer.WarpIntoVehicle(copcar, VehicleSeat.Driver);
            officer.Task.ClearAllImmediately();
            AttachOfficerInVehicle(officer);
        }

        /// <summary>
        /// Attach a officer in pursuit.
        /// </summary>
        /// <param name="officer">Officer.</param>
        /// <exception cref="GTA.NonExistingObjectException"></exception>
        public void AttachOfficerOnFoot(Ped officer)
        {
            if (officer.Exists() == false)
            {
                throw new GTA.NonExistingObjectException("That officer ped you called dosen't exist or not created.");
            }
            if (officer.isInVehicle() == true)
            {
                throw new ArgumentException("The officer ped you called is in a vehicle.");
            }
            officer.Task.ClearAllImmediately();
            APed Aofficer = TypeConverter.ConvertToAPed(officer);
            officer.SayAmbientSpeech("CHASE_SOLO");
            Aofficer.TaskCombatBustPed(Atarget);
            officers.Add(officer);
            Aofficers.Add(Aofficer);
        }
        public void AttachOfficerInVehicle(Ped officer)
        {
            if (officer.Exists() == false)
            {
                throw new GTA.NonExistingObjectException("That officer ped you called dosen't exist or not created.");
            }
            if (officer.isInVehicle() == false)
            {
                throw new ArgumentException("The officer ped you called is on foot.");
            }
            officer.Task.ClearAllImmediately();
            officer.CurrentVehicle.SirenActive = true;
            APed Aofficer = TypeConverter.ConvertToAPed(officer);
            officer.SayAmbientSpeech("CHASE_SOLO");
            AVehicle car = TypeConverter.ConvertToAVehicle(officer.CurrentVehicle);
            car.AddCopBlip();
            car.SetAsPoliceVehicle(true);
            if (target.isInVehicle())
            {
                Aofficer.TaskCombatPersueInCarSubtask(Atarget);
                officer.SayAmbientSpeech("PULL_OVER_WARNING");
            }
            else
            {
                officer.SayAmbientSpeech("MEGAPHONE_FOOT_PURSUIT");
                officer.Task.LeaveVehicle();
                Aofficer.TaskCombatBustPed(Atarget);
            }
            officers.Add(officer);
            Aofficers.Add(Aofficer);
        }
        public void AttachNooseTeamBackup()
        {
            Ped SwatDriver = World.CreatePed(Plyr.Character.Position.Around(30f));
            Ped SwatOfficer = World.CreatePed(Plyr.Character.Position.Around(30f));
            Vehicle SwatVehicle = World.CreateVehicle("NSTOCKADE", Plyr.Character.Position.Around(30f));
            SwatDriver.WarpIntoVehicle(SwatVehicle, VehicleSeat.Driver);
            SwatOfficer.WarpIntoVehicle(SwatVehicle, VehicleSeat.RightFront);
            SwatOfficer.WillDoDrivebys = true;
            SwatVehicle.SirenActive = true;
        }

        private void Kill()
        {
            foreach(Ped officer in officers)
            {
                if (officer.isInVehicle())
                {
                    officer.Task.ClearAll();
                    officer.Task.CruiseWithVehicle(officer.CurrentVehicle, 15f, true);
                    officer.NoLongerNeeded();
                }
                if (officer.isInVehicle() == false)
                {
                    officer.Task.ClearAll();
                    officer.Task.WanderAround();
                    officer.NoLongerNeeded();
                }
            }
        }
    }
}
