using System;
using System.Collections.Generic;
using EawXBuild.Core;
using NLua;
using NLua.Exceptions;

namespace EawXBuild.Configuration.Lua.v1
{
    public class LuaJob
    {
        private readonly IJob _job;

        public LuaJob(IJob job)
        {
            _job = job;
        }

        public void tasks(LuaTable allTasks)
        {
            foreach (KeyValuePair<object?, object?>? keyValuePair in allTasks)
            {
                var task = BuildTask(keyValuePair);
                _job.AddTask(task);
            }
        }

        private ITask BuildTask(KeyValuePair<object?, object?>? keyValuePair)
        {
            var taskTable = GetTaskTableOrThrow(keyValuePair);
            var luaTask = GetTaskOrThrow(taskTable);
            var task = luaTask.Task;
            task.Name = GetTaskNameOrThrow(taskTable);
            return task;
        }

        private static LuaTable GetTaskTableOrThrow(KeyValuePair<object?, object?>? keyValuePair)
        {
            return keyValuePair?.Value as LuaTable
                    ?? throw new LuaScriptException("Task table must be array based", "LuaJob.tasks");
        }

        private static string GetTaskNameOrThrow(LuaTable taskTable)
        {
            return taskTable["name"] as string
                    ?? throw new LuaScriptException("'name' must not be nil", "LuaJob.tasks"); ;
        }

        private static ILuaTask GetTaskOrThrow(LuaTable taskTable)
        {
            return taskTable["action"] as ILuaTask
                    ?? throw new LuaScriptException("'action' must be a task", "LuaJob.tasks"); ;
        }
    }
}