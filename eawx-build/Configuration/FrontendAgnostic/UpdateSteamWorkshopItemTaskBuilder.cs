using System;
using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;
using EawXBuild.Tasks.Steam;

namespace EawXBuild.Configuration.FrontendAgnostic {
    public class UpdateSteamWorkshopItemTaskBuilder : ITaskBuilder {
        private readonly UpdateSteamWorkshopItemTask _task;
        private readonly WorkshopItemChangeSet _changeSet;

        public UpdateSteamWorkshopItemTaskBuilder() {
            var fileSystem = new FileSystem();
            _changeSet = new WorkshopItemChangeSet(fileSystem);
            _task = new UpdateSteamWorkshopItemTask(FacepunchSteamWorkshopAdapter.Instance) {
                ChangeSet = _changeSet
            };
        }

        public ITaskBuilder With(string name, object value) {
            switch (name) {
                case "AppId":
                    _task.AppId = Convert.ToUInt32(value);
                    break;
                case "ItemId":
                    _task.ItemId = Convert.ToUInt64(value);
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
                    if (value != null)
                        _changeSet.Visibility = (WorkshopItemVisibility) value;
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