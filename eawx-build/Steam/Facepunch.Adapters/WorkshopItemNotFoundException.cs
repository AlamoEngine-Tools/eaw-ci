using System;

namespace EawXBuild.Steam.Facepunch.Adapters {
    [Serializable]
    public class WorkshopItemNotFoundException : Exception {
        public WorkshopItemNotFoundException(string message) : base(message) {
        }
    }
}