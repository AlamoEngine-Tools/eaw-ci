using System;
using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;

namespace EawXBuild.Configuration.FrontendAgnostic {
    public class CreateSteamWorkshopItemTaskBuilder : ITaskBuilder {
        private readonly CreateSteamWorkshopItemTask _task;

        public CreateSteamWorkshopItemTaskBuilder() {
            _task = new CreateSteamWorkshopItemTask(FacepunchSteamWorkshopAdapter.Instance, new FileSystem());
        }

        public ITaskBuilder With(string name, object value) {
            switch (name) {
                case "AppId":
                    _task.AppId = (uint) value;
                    break;
                case "Title":
                    _task.Title = (string) value;
                    break;
                case "Description":
                    _task.Description = (string) value;
                    break;
                case "ItemFolderPath":
                    _task.ItemFolderPath = (string) value;
                    break;
                case "Language":
                    _task.Language = (string) value;
                    break;
                case "Visibility":
                    Enum.TryParse((string) value, out WorkshopItemVisibility visibility);
                    _task.Visibility = visibility;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid configuration option: {name}");
            }

            return this;
        }

        public ITask Build() {
            return _task;
        }
    }
}