using LSPD_First_Response.Mod.API;
using Rage;
using CommonDataFramework.API;
using CommonDataFramework.Modules.VehicleDatabase;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace MobileTerminal
{
    public class Main : Plugin
    {
        private static IServiceProvider _serviceProvider;

        private static IServiceProvider CreateProvider()
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            var collection = new ServiceCollection()
                .AddSingleton(jsonOptions);

            return collection.BuildServiceProvider();

        }

        public override void Initialize()
        {
            _serviceProvider = CreateProvider();

            Functions.OnOnDutyStateChanged += OnOnDuty;
            Game.LogTrivial("MobileTerminal: Hello!");
        }

        public override void Finally()
        {
            Game.LogTrivial("MobileTerminal: Goodbye!");
        }

        private void CleanUp()
        {
            Game.LogTrivial("MobileTerminal: Cleaned up.");
        }

        private void OnOnDuty(bool isOnDuty)
        {
            GameFiber.WaitUntil(CDFFunctions.IsPluginReady, 60000);

            if (!isOnDuty)
            {
                CleanUp();
                return;
            }

            Game.LogTrivial("MobileTerminal: On duty.");
            Game.DisplayNotification("MobileTerminal: Hello!");

            if (!LoadedPlugin.Exists("PolicingRedefined"))
            {
                Game.LogTrivial("MobileTerminal: PolicingRedefined not found. Cleaning up.");
                Game.DisplayNotification("MobileTerminal: PolicingRedefined not found. Exiting.");
                CleanUp();
                return;
            }

            if (!LoadedPlugin.Exists("CommonDataFramework"))
            {
                Game.LogTrivial("MobileTerminal: CommonDataFramework not found. Cleaning up.");
                Game.DisplayNotification("MobileTerminal: CommonDataFramework not found. Exiting.");
                CleanUp();
                return;
            }


            Events.OnPulloverStarted += OnPulloverStarted;
        }

        private void OnPulloverStarted(LHandle handle)
        {
            Game.LogTrivial("Pulled over.");
            var ped = Functions.GetPulloverSuspect(handle);
            var vehicle = ped.LastVehicle;
            Game.DisplayNotification(vehicle.GetVehicleData().PrimaryColor);
            Game.DisplayNotification(World.GetStreetName(Game.LocalPlayer.Character.Position));
        }
    }
}