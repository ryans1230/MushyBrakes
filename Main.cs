using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Threading.Tasks;

namespace HotBrakes
{
    public class Main : BaseScript
    {
        protected Vehicle lastCar;
        protected int hotBrakes = 0;
        protected bool notified = false;
        protected bool hotNotify = false;

        [Tick]
        internal async Task HeatBrakes()
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;

            if (Entity.Exists(veh) && (veh.Model.IsCar || veh.Model.IsBike) && veh.Driver == Game.PlayerPed)
            {
                if (lastCar == veh)
                {
                    if (veh.Speed > 5f && veh.CurrentGear != 0 && Game.IsControlPressed(0, Control.VehicleBrake))
                    {
                        if (hotBrakes > 10000)
                        {
                            Screen.DisplayHelpTextThisFrame("~r~Your brakes are disabled due to being too hot.");
                            Game.DisableControlThisFrame(0, Control.VehicleBrake);
                        }
                        else if (hotBrakes > 5000)
                        {
                            if (hotBrakes % 2 == 0)
                            {
                                if (!hotNotify)
                                {
                                    Screen.ShowNotification("~y~Your brakes are ~r~REALLY ~y~getting hot!");
                                    notified = true;
                                    hotNotify = true;
                                }
                                Game.DisableControlThisFrame(0, Control.VehicleBrake);
                            }
                        }
                        else if (hotBrakes > 3500)
                        {
                            if (hotBrakes % 5 == 0)
                            {
                                Game.DisableControlThisFrame(0, Control.VehicleBrake);
                            }
                        }
                        else if (hotBrakes > 2500 && hotBrakes % 10 == 0)
                        {
                            if (!notified)
                            {
                                Screen.ShowNotification("~y~Your brakes are getting hot!");
                                notified = true;
                            }
                            Game.DisableControlThisFrame(0, Control.VehicleBrake);
                        }

                        hotBrakes += 10;
                        veh.AreBrakeLightsOn = true;
                    }
                    if (veh.Mods[VehicleModType.Brakes].Index > 0 && hotBrakes > 0)
                    {
                        hotBrakes -= 2;
                    }
                }
                else
                {
                    // Got in a new car
                    hotBrakes = 0;
                    lastCar = veh;
                    notified = false;
                    hotNotify = false;
                }
            }

            // Cooldown the brakes slowly
            if (hotBrakes > 0)
            {
                if (new Random(100).Next() < 34)
                {
                    hotBrakes -= 4;
                }
                hotBrakes -= 1;
            } else
            {
                // Brakes are completely cold. Renotify them.
                notified = false;
                hotNotify = false;
            }
        }
    }
}
