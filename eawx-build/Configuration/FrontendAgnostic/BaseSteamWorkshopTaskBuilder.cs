using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Steam;
using EawXBuild.Tasks.Steam;

namespace EawXBuild.Configuration.FrontendAgnostic
{
    public abstract class BaseSteamWorkshopTaskBuilder : ITaskBuilder
    {
        protected readonly IWorkshopItemChangeSet ChangeSet;
        protected SteamWorkshopTask Task;

        protected BaseSteamWorkshopTaskBuilder()
        {
            FileSystem fileSystem = new FileSystem();
            ChangeSet = new WorkshopItemChangeSet(fileSystem);
            Task ??= CreateTaskWithChangeSet(ChangeSet);
        }

        public virtual ITaskBuilder With(string name, object value)
        {
            switch (name)
            {
                case "AppId":
                    Task.AppId = Convert.ToUInt32(value);
                    break;
                case "Title":
                    ChangeSet.Title = (string) value;
                    break;
                case "DescriptionFilePath":
                    ChangeSet.DescriptionFilePath = (string) value;
                    break;
                case "ItemFolderPath":
                    ChangeSet.ItemFolderPath = (string) value;
                    break;
                case "Language":
                    ChangeSet.Language = (string) value;
                    break;
                case "Visibility":
                    if (value != null)
                        ChangeSet.Visibility = (WorkshopItemVisibility) value;
                    break;
                case "Tags":
                    ChangeSet.Tags = (HashSet<string>) value;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid configuration option: {name}");
            }

            return this;
        }

        public ITask Build()
        {
            return Task;
        }

        protected abstract SteamWorkshopTask CreateTaskWithChangeSet(IWorkshopItemChangeSet changeSet);
    }
}