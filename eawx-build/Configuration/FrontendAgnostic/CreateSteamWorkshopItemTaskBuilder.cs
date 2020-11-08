using System;
using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;

namespace EawXBuild.Configuration.FrontendAgnostic {
    public class CreateSteamWorkshopItemTaskBuilder : ITaskBuilder {
        private readonly CreateSteamWorkshopItemTask _task;
        private WorkshopItemChangeSet _changeSet;

        public CreateSteamWorkshopItemTaskBuilder() {
            var fileSystem = new FileSystem();
            _changeSet = new WorkshopItemChangeSet(fileSystem);
            _task = new CreateSteamWorkshopItemTask(FacepunchSteamWorkshopAdapter.Instance) {
                ChangeSet = _changeSet
            };
        }

        public ITaskBuilder With(string name, object value) {
            switch (name) {
                case "AppId":
                    _task.AppId = (uint) value;
                    break;
                case "Title":
                    _changeSet.Title = (string) value;
                    break;
                case "DescriptionFilePath":
                    _changeSet.DescriptionFilePath = (string) value;
                    break;
                case "ItemFolderPath":
                    _changeSet.ItemFolderPath = (string) value;
                    break;
                case "Language":
                    _changeSet.Language = (string) value;
                    break;
                case "Visibility":
                    Enum.TryParse((string) value, out WorkshopItemVisibility visibility);
                    _changeSet.Visibility = visibility;
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